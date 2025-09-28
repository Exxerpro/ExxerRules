#!/usr/bin/env python3
"""
Wrapper to run docs generation in safe batches and stop if build errors increase.

Algorithm
 1) Baseline build of solution (TreatWarningsAsErrors=false) and collect counts:
    - total errors
    - total warnings
    - specific pattern count (default: CS1591)
 2) For each batch percent in list (e.g., 10,20,30,40,50):
    - Run generate_xml_docs.py with --apply for that percent and given root.
    - Parse output for BACKUP_DIR and CHANGED file list.
    - Rebuild and collect counts again.
    - If errors increase OR (optionally) pattern count increases, stop and report
      backup dir + changed list for surgical restore.
    - Otherwise continue.

Usage
  python ExxerRules/src/scripts/validate_apply_cycle.py \
    --solution ExxerRules/src/IndFusion.sln \
    --root ExxerRules/src/test/IndFusion.Analyzer.Tests \
    --percents 10,20,30,40,50 \
    --pattern CS1591 \
    --allow-strings  # if you want to include files with code-as-string

Notes
 - This wrapper does not auto-restore files. It reports BACKUP_DIR and CHANGED files
   for manual/surgical restoration.
 - Ensure dotnet SDK matches the solution requirements.
"""

from __future__ import annotations
import argparse
import subprocess
import sys
import re
from pathlib import Path


def run(cmd: list[str]) -> tuple[int, str]:
    proc = subprocess.run(cmd, capture_output=True, text=True)
    out = (proc.stdout or '') + (proc.stderr or '')
    return proc.returncode, out


def parse_build_output(text: str, pattern: str) -> dict:
    # Simple parsers: count lines containing ' error ' and ' warning '
    errors = len([l for l in text.splitlines() if re.search(r'\berror\b', l, re.IGNORECASE)])
    warnings = len([l for l in text.splitlines() if re.search(r'\bwarning\b', l, re.IGNORECASE)])
    patt = len([l for l in text.splitlines() if pattern and (pattern in l)])
    return { 'errors': errors, 'warnings': warnings, 'pattern': patt }


def baseline_build(solution: str) -> dict:
    code, out = run(['dotnet', 'build', solution, '-c', 'Debug', '-v', 'minimal', '-p:TreatWarningsAsErrors=false'])
    if code != 0:
        print('Baseline build failed; inspect output below:', file=sys.stderr)
        print(out)
        sys.exit(1)
    return parse_build_output(out, args.pattern)


def generate_batch(root: str, percent: int, allow_strings: bool) -> tuple[str, list[str]]:
    cmd = ['python', str(Path(__file__).with_name('generate_xml_docs.py')), '--root', root, '--percent', str(percent), '--apply']
    if allow_strings:
        cmd.append('--allow-strings')
    code, out = run(cmd)
    if code != 0:
        print('Generator failed; output:', file=sys.stderr)
        print(out)
        sys.exit(code)
    backup_dir = ''
    changed: list[str] = []
    for line in out.splitlines():
        if line.startswith('BACKUP_DIR:'):
            backup_dir = line.split(':', 1)[1].strip()
        elif line.startswith('CHANGED: '):
            changed.append(line.split(': ', 1)[1].strip())
    return backup_dir, changed


def main(args: argparse.Namespace) -> int:
    base = baseline_build(args.solution)
    print(f"Baseline -> errors={base['errors']} warnings={base['warnings']} {args.pattern}={base['pattern']}")

    for p in args.percents:
        print(f"\n=== Batch {p}% ===")
        backup_dir, changed = generate_batch(args.root, p, args.allow_strings)
        print(f"Changed {len(changed)} files. Backup: {backup_dir}")

        code, out = run(['dotnet', 'build', args.solution, '-c', 'Debug', '-v', 'minimal', '-p:TreatWarningsAsErrors=false'])
        if code != 0:
            print('Build failed after applying batch; STOP.', file=sys.stderr)
            print(out)
            print('Surgical restore from backup or git is recommended for changed files:')
            for f in changed:
                print(' -', f)
            print('Backup dir:', backup_dir)
            return 2
        current = parse_build_output(out, args.pattern)
        print(f"After {p}% -> errors={current['errors']} warnings={current['warnings']} {args.pattern}={current['pattern']}")

        # Stop if errors increased; also stop if pattern count increased (optional strict mode)
        if current['errors'] > base['errors'] or (args.strict_pattern and current['pattern'] > base['pattern']):
            print('Counts regressed; STOP. Consider surgical restore:', file=sys.stderr)
            for f in changed:
                print(' -', f)
            print('Backup dir:', backup_dir)
            return 3

        # Accept new baseline to allow gradual improvements
        base = current

    print('All batches completed without regressions.')
    return 0


if __name__ == '__main__':
    ap = argparse.ArgumentParser()
    ap.add_argument('--solution', required=True, help='Path to .sln')
    ap.add_argument('--root', required=True, help='Root test folder to process')
    ap.add_argument('--percents', default='10,20,30,40,50', help='Comma-separated batch percents')
    ap.add_argument('--pattern', default='CS1591', help='Warning pattern to track (e.g., CS1591)')
    ap.add_argument('--strict-pattern', action='store_true', help='Stop if pattern count increases')
    ap.add_argument('--allow-strings', action='store_true', help='Allow files with raw/verbatim strings')
    args = ap.parse_args()
    args.percents = [int(x) for x in args.percents.split(',') if x.strip()]
    sys.exit(main(args))
