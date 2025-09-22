#!/usr/bin/env python3
"""
Batch-generate XML documentation for public test classes and methods.

Goals
 - Add <summary> above public classes and public methods (incl. async), placing
   docs above any contiguous attribute block.
 - Handle multi-line method signatures (collect until matching ')').
 - Never overwrite existing XML docs; skip already documented members.
 - Safety: dry run by default, per‑run backup of modified files, percentage batching,
   optional git stage/commit/push.
 - Heuristics to avoid inserting inside code-as-string: by default, skip files that
   contain raw (triple-quote) or verbatim (@" ... ") string markers. Can be overridden.

Usage examples
  Preview (10%):
    python ExxerRules/src/scripts/generate_xml_docs.py --root ExxerRules/src/test --percent 10

  Apply + commits (10%):
    python ExxerRules/src/scripts/generate_xml_docs.py --root ExxerRules/src/test --percent 10 --apply --git

  Apply allowing files with code-as-strings:
    python ExxerRules/src/scripts/generate_xml_docs.py --root ExxerRules/src/test --percent 10 --apply --allow-strings
"""

from __future__ import annotations
import argparse
import dataclasses
import datetime as _dt
import os
import re
import shutil
import subprocess
import sys
from typing import List, Tuple


CLASS_RE = re.compile(r"^(\s*)public\s+(?:partial\s+)?class\s+(?P<name>[A-Za-z_][A-Za-z0-9_]*)\b")
# Start of a method signature (may be multi-line)
METHOD_START_RE = re.compile(
    r"^(\s*)public\s+(?:async\s+)?[A-Za-z0-9_<>,\[\]\.\?\s]+\s+(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*\("
)

ATTRIBUTE_RE = re.compile(r"^\s*\[")
XML_SUMMARY_OPEN_RE = re.compile(r"^\s*///\s*<summary>")
HAS_XML_ABOVE_RE = re.compile(r"^\s*///")


@dataclasses.dataclass
class Edit:
    index: int
    lines: List[str]


def find_files(root: str) -> List[str]:
    files: List[str] = []
    for dirpath, _, filenames in os.walk(root):
        if any(x in dirpath for x in (os.sep + 'bin' + os.sep, os.sep + 'obj' + os.sep)):
            continue
        for fn in filenames:
            if not fn.endswith('.cs'):
                continue
            if fn in ('GlobalUsings.cs', 'AssemblyInfo.cs') or fn.endswith('.g.cs'):
                continue
            files.append(os.path.join(dirpath, fn))
    files.sort()
    return files


def has_raw_or_verbatim_strings(text: str) -> bool:
    # Very conservative: skip if raw (""") or verbatim (@") appears anywhere
    return ('@"' in text) or ('"""' in text)


def has_xml_doc_above(lines: List[str], idx: int) -> bool:
    # Check up to two lines above for an XML doc marker
    i = max(0, idx - 1)
    if HAS_XML_ABOVE_RE.match(lines[i] if i < len(lines) else ''):
        return True
    j = max(0, idx - 2)
    return HAS_XML_ABOVE_RE.match(lines[j] if j < len(lines) else '') is not None


def previous_attribute_block_start(lines: List[str], idx: int) -> int:
    insert_at = idx
    k = idx - 1
    while k >= 0:
        t = lines[k].strip()
        if t == '':
            k -= 1
            continue
        if ATTRIBUTE_RE.match(lines[k]):
            insert_at = k
            k -= 1
            continue
        break
    return insert_at


def build_method_signature(lines: List[str], start_idx: int) -> Tuple[str, int]:
    # Concatenate lines from start_idx until we find a closing ')' balancing parentheses
    sig = []
    depth = 0
    end_idx = start_idx
    for i in range(start_idx, min(len(lines), start_idx + 20)):
        part = lines[i].rstrip('\n')
        sig.append(part)
        depth += part.count('(')
        depth -= part.count(')')
        end_idx = i
        if depth <= 0:
            break
    signature = ' '.join(s.strip() for s in sig)
    return signature, end_idx


def summarize_method(name: str) -> str:
    # Convert snake/camel test style names to a sentence-ish summary
    if '_' in name:
        parts = name.split('_')
        out = ' '.join(parts)
    else:
        out = re.sub(r'([a-z])([A-Z])', r'\1 \2', name)
    if not out.endswith('.'):
        out += '.'
    return out


def class_doc(class_name: str, indent: str) -> List[str]:
    return [
        f"{indent}/// <summary>",
        f"{indent}/// Tests for {class_name}.",
        f"{indent}/// </summary>",
    ]


def method_doc(method_name: str, indent: str, return_type: str, params_str: str) -> List[str]:
    lines = [
        f"{indent}/// <summary>",
        f"{indent}/// {summarize_method(method_name)}",
        f"{indent}/// </summary>",
    ]
    if params_str.strip():
        # naive split on commas; trim to last token per param
        for p in params_str.split(','):
            p = re.sub(r'<[^>]+>', '', p)
            p = p.strip()
            if not p:
                continue
            name = p.split()[-1]
            # strip trailing commas/array brackets
            name = name.strip(',').strip()
            if name:
                lines.append(f"{indent}/// <param name=\"{name}\"></param>")
    if return_type.strip() and return_type.strip() != 'void':
        lines.append(f"{indent}/// <returns></returns>")
    return lines


def plan_edits(path: str, text: str, allow_strings: bool) -> List[Edit]:
    if (not allow_strings) and has_raw_or_verbatim_strings(text):
        return []
    lines = text.splitlines()
    edits: List[Edit] = []
    i = 0
    while i < len(lines):
        line = lines[i]
        # Classes
        m = CLASS_RE.match(line)
        if m and not has_xml_doc_above(lines, i):
            insert_at = previous_attribute_block_start(lines, i)
            indent = m.group(1)
            name = m.group('name')
            edits.append(Edit(insert_at, class_doc(name, indent)))
            i += 1
            continue

        # Methods (support multi-line parameters)
        mm = METHOD_START_RE.match(line)
        if mm and not has_xml_doc_above(lines, i):
            signature, end_idx = build_method_signature(lines, i)
            # Extract return type and method name from signature
            mm2 = re.match(r"^\s*public\s+(?:async\s+)?(?P<ret>[A-Za-z0-9_<>,\[\]\.\?\s]+?)\s+(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*\((?P<params>[^)]*)\)", signature)
            if mm2:
                indent = mm.group(1)
                ret = mm2.group('ret').strip()
                name = mm2.group('name').strip()
                params = mm2.group('params') or ''
                insert_at = previous_attribute_block_start(lines, i)
                edits.append(Edit(insert_at, method_doc(name, indent, ret, params)))
                i = end_idx + 1
                continue
        i += 1
    return edits


def apply_edits(path: str, text: str, edits: List[Edit]) -> str:
    if not edits:
        return text
    lines = text.splitlines()
    # insert bottom-up
    for e in sorted(edits, key=lambda x: x.index, reverse=True):
        lines[e.index:e.index] = e.lines
    return "\n".join(lines)


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument('--root', default=os.path.join(os.path.dirname(__file__), '..', 'test'))
    ap.add_argument('--files', help='Comma-separated explicit file paths to process (overrides --root scan)')
    ap.add_argument('--percent', type=int, default=10)
    ap.add_argument('--apply', action='store_true')
    ap.add_argument('--git', action='store_true')
    ap.add_argument('--push', action='store_true')
    ap.add_argument('--allow-strings', action='store_true', help='Do not skip files with raw/verbatim strings')
    ap.add_argument('--stepwise', action='store_true', help='Apply one insertion at a time per file, re-reading after each to avoid drift')
    ap.add_argument('--max-inserts-per-file', type=int, default=10, help='When --stepwise is set, limit inserts per file (default 10). Use 1 to test a single method insert.')
    args = ap.parse_args()

    files: List[str]
    if args.files:
        files = [s.strip() for s in args.files.split(',') if s.strip()]
    else:
        files = find_files(args.root)
    plan_list = []
    total_inserts = 0
    for p in files:
        try:
            with open(p, 'r', encoding='utf-8', errors='replace') as fh:
                tx = fh.read()
        except Exception:
            continue
        edits = plan_edits(p, tx, allow_strings=args.allow_strings)
        if edits:
            plan_list.append((p, edits))
            total_inserts += sum(len(e.lines) for e in edits)

    if not plan_list:
        print('No candidates found.')
        return 0

    take = max(1, (len(plan_list) * args.percent + 99) // 100)
    batch = plan_list[:take]
    print(f'Found {len(plan_list)} files needing docs (total insert lines: {total_inserts})')
    print(f'Batch percent={args.percent}% -> taking {len(batch)} files')

    if not args.apply:
        for p, edits in batch:
            print(f' - {p} ({sum(len(e.lines) for e in edits)} lines to insert)')
        print('Dry-run complete. Re-run with --apply to write changes.')
        return 0

    ts = _dt.datetime.now().strftime('%Y%m%d-%H%M%S')
    backup_dir = os.path.join(os.path.dirname(__file__), f'backup-xml-docs-py-{ts}')
    os.makedirs(backup_dir, exist_ok=True)
    changed = []
    for p, edits in batch:
        try:
            if not args.stepwise:
                with open(p, 'r', encoding='utf-8', errors='replace') as fh:
                    tx = fh.read()
                new_tx = apply_edits(p, tx, edits)
                if new_tx != tx:
                    shutil.copy2(p, os.path.join(backup_dir, os.path.basename(p)))
                    with open(p, 'w', encoding='utf-8', newline='') as fh:
                        fh.write(new_tx)
                    changed.append(p)
            else:
                # Stepwise: apply one insertion at a time with re-scan to avoid index drift
                # Copy backup once when first change happens
                backed_up = False
                max_iters = max(1, args.max_inserts_per_file)
                iters = 0
                while iters < max_iters:
                    iters += 1
                    with open(p, 'r', encoding='utf-8', errors='replace') as fh:
                        current = fh.read()
                    step_edits = plan_edits(p, current, allow_strings=args.allow_strings)
                    if not step_edits:
                        break
                    # Choose the earliest insertion to minimize interaction
                    step = sorted(step_edits, key=lambda e: e.index)[0]
                    # Apply only that step
                    new_current = apply_edits(p, current, [step])
                    if new_current == current:
                        break
                    if not backed_up:
                        shutil.copy2(p, os.path.join(backup_dir, os.path.basename(p)))
                        backed_up = True
                    with open(p, 'w', encoding='utf-8', newline='') as fh:
                        fh.write(new_current)
                if backed_up:
                    changed.append(p)
        except Exception as e:
            print(f'WARN: failed to update {p}: {e}', file=sys.stderr)

    print(f'BACKUP_DIR: {backup_dir}')
    print(f'Backed up {len(changed)} files to {backup_dir}')
    print(f'Applied changes to {len(changed)} files.')
    for p in changed:
        print(f'CHANGED: {p}')

    if args.git and changed:
        def _run(cmd: List[str]):
            try:
                subprocess.run(cmd, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            except Exception:
                pass
        _run(['git', 'add', '--'] + changed)
        _run(['git', 'commit', '-m', f'docs(tests): add XML docs (batch {args.percent}%) [pass 1]'])
        _run(['git', 'add', '--'] + changed)
        _run(['git', 'commit', '-m', f'docs(tests): refine XML docs (batch {args.percent}%) [pass 2]'])
        if args.push:
            _run(['git', 'push'])
        print('Git commits created.')

    return 0


if __name__ == '__main__':
    sys.exit(main())
