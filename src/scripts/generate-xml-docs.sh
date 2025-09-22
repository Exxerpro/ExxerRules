#!/usr/bin/env bash
set -euo pipefail

# Batch-generate XML docs for public classes/methods in test files.
# Cross-platform bash version (no PowerShell/Python required).

ROOT="$(cd "$(dirname "$0")"/.. && pwd)/test"
BATCH_PERCENT=10
APPLY=0
DO_GIT=0
DO_PUSH=0

usage() {
  cat <<USAGE
Usage: $(basename "$0") [options]
  -r, --root DIR        Root to scan (default: $ROOT)
  -p, --percent N       Batch percent 1-100 (default: $BATCH_PERCENT)
  -a, --apply           Apply changes (default: dry-run)
  -g, --git             Stage and commit changes in two passes
  --push                Push after commits
  -h, --help            Show this help
USAGE
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    -r|--root) ROOT="$2"; shift 2;;
    -p|--percent) BATCH_PERCENT="$2"; shift 2;;
    -a|--apply) APPLY=1; shift;;
    -g|--git) DO_GIT=1; shift;;
    --push) DO_PUSH=1; shift;;
    -h|--help) usage; exit 0;;
    *) echo "Unknown arg: $1"; usage; exit 1;;
  esac
done

if ! [[ "$BATCH_PERCENT" =~ ^[0-9]+$ ]] || (( BATCH_PERCENT < 1 || BATCH_PERCENT > 100 )); then
  echo "Invalid percent: $BATCH_PERCENT" >&2; exit 1
fi

mapfile -t FILES < <(find "$ROOT" -type f -name "*.cs" \
  ! -path "*/bin/*" ! -path "*/obj/*" \
  ! -name "GlobalUsings.cs" ! -name "AssemblyInfo.cs" ! -name "*.g.cs" | sort)

has_xmldoc_above() {
  local file="$1"; local lineno="$2"
  local prev1 prev2
  prev1=$(sed -n "$((lineno-1))p" "$file" 2>/dev/null || true)
  prev2=$(sed -n "$((lineno-2))p" "$file" 2>/dev/null || true)
  [[ "${prev1#*[![:space:]]}" == "" ]] && prev1="$prev2" # allow one blank
  if [[ "$prev1" =~ ^[[:space:]]*/// ]]; then return 0; fi
  if [[ "$prev2" =~ ^[[:space:]]*/// ]]; then return 0; fi
  return 1
}

needs_edits() {
  local file="$1"; local count=0
  local re_class='^[[:space:]]*public[[:space:]]+(partial[[:space:]]+)?class[[:space:]]+[A-Za-z0-9_]+'
  local re_method='^[[:space:]]*public[[:space:]]+(async[[:space:]]+)?[A-Za-z0-9_<>\[\]\?]+[[:space:]]+[A-Za-z0-9_]+\s*\([^)]*\)'
  local i=0
  local in_raw=0 in_ver=0
  while IFS= read -r line; do
    # crude raw string tracking (""" toggles)
    if [[ "$line" == *'"""'* ]]; then
      local cnt
      cnt=$(echo "$line" | grep -o '"""' | wc -l | tr -d ' ')
      if (( cnt % 2 == 1 )); then in_raw=$((1-in_raw)); fi
    fi
    # crude verbatim string tracking: start on = @" and robust end when an odd number of quotes before ;
    if (( in_ver == 0 && in_raw == 0 )) && echo "$line" | grep -qE '=\s*@"'; then in_ver=1; fi
    if (( in_ver == 1 && in_raw == 0 )); then
      # Check for line ending with ..."; (possibly spaces) and odd trailing quotes
      t=$(echo "$line" | sed -E 's/[[:space:]]+$//')
      if [[ "${t: -1}" == ";" ]]; then
        presemi="${t%?}"
        # count trailing quotes
        qcount=0
        while [[ -n "$presemi" && ${presemi: -1} == '"' ]]; do
          qcount=$((qcount+1))
          presemi="${presemi%?}"
        done
        if (( qcount % 2 == 1 )); then in_ver=0; fi
      fi
    fi
    ((i++))
    if echo "$line" | grep -Eq "$re_class" || echo "$line" | grep -Eq "$re_method"; then
      if ! has_xmldoc_above "$file" "$i"; then ((count++)); fi
    fi
  done < "$file"
  echo "$count"
}

declare -a PLAN
total_edits=0
for f in "${FILES[@]}"; do
  c=$(needs_edits "$f")
  if (( c > 0 )); then PLAN+=("$f:$c"); total_edits=$((total_edits+c)); fi
done

if (( ${#PLAN[@]} == 0 )); then
  echo "No missing XML docs found in $ROOT"; exit 0
fi

echo "Found ${#PLAN[@]} files needing docs (total edits: $total_edits)"
take=$(( ( ${#PLAN[@]} * BATCH_PERCENT + 99 ) / 100 ))
(( take < 1 )) && take=1
echo "BatchPercent=$BATCH_PERCENT% -> taking $take files this run"

declare -a BATCH
for ((i=0; i<take; i++)); do BATCH+=("${PLAN[$i]}"); done

insert_docs() {
  local file="$1"; local tmp="$file.tmp.$$"
  local re_ns='^[[:space:]]*namespace[[:space:]]+([A-Za-z0-9_\.]+)[[:space:]]*;'
  local ns
  ns=$(grep -Eo "$re_ns" "$file" | head -1 | sed -E 's/^\s*namespace\s+([^;]+).*/\1/')
  : > "$tmp"
  local prev1="" prev2="" line indent name ret params
  local re_class='^([[:space:]]*)public[[:space:]]+(partial[[:space:]]+)?class[[:space:]]+([A-Za-z0-9_]+)'
  local re_method='^([[:space:]]*)public[[:space:]]+(async[[:space:]]+)?([A-Za-z0-9_<>\[\]\?]+)[[:space:]]+([A-Za-z0-9_]+)\s*\(([^)]*)\)'
  local in_raw=0 in_ver=0
  while IFS= read -r line; do
    # crude raw string tracking (""" toggles)
    if [[ "$line" == *'"""'* ]]; then
      local cnt
      cnt=$(echo "$line" | grep -o '"""' | wc -l | tr -d ' ')
      if (( cnt % 2 == 1 )); then in_raw=$((1-in_raw)); fi
    fi
    # crude verbatim string tracking: start on = @" and close when an odd number of quotes appear before ;
    if (( in_ver == 0 && in_raw == 0 )) && echo "$line" | grep -qE '=\s*@"'; then in_ver=1; fi
    if (( in_ver == 1 && in_raw == 0 )); then
      t=$(echo "$line" | sed -E 's/[[:space:]]+$//')
      if [ "${t: -1}" = ";" ]; then
        presemi="${t%?}"
        qcount=0
        while [ -n "$presemi" ] && [ "${presemi: -1}" = '"' ]; do
          qcount=$((qcount+1))
          presemi="${presemi%?}"
        done
        if (( qcount % 2 == 1 )); then in_ver=0; fi
      fi
    fi
    if (( in_raw == 0 && in_ver == 0 )) && echo "$line" | grep -Eq "$re_class"; then
      indent=$(echo "$line" | sed -E "s/$re_class/\\1/" | tr -d '\r')
      name=$(echo "$line" | sed -E "s/$re_class/\\3/" | tr -d '\r')
      if ! [[ "$prev1" =~ ^[[:space:]]*/// || "$prev2" =~ ^[[:space:]]*/// ]]; then
        echo "${indent}/// <summary>" >> "$tmp"
        local ns_tail
        ns_tail="${ns##*.}"
        if [[ "$name" =~ Tests?$ ]]; then
          echo "${indent}/// Tests for ${ns_tail}." >> "$tmp"
        else
          echo "${indent}/// Type ${name}." >> "$tmp"
        fi
        echo "${indent}/// </summary>" >> "$tmp"
      fi
      echo "$line" >> "$tmp"
    elif (( in_raw == 0 && in_ver == 0 )) && echo "$line" | grep -Eq "$re_method"; then
      # recompute groups using sed/awk because grep -E doesn't expose captures
      indent=$(echo "$line" | sed -E "s/$re_method/\1/" | tr -d '\r')
      ret=$(echo "$line" | sed -E "s/$re_method/\3/" | tr -d '\r')
      name=$(echo "$line" | sed -E "s/$re_method/\4/" | tr -d '\r')
      params=$(echo "$line" | sed -E "s/$re_method/\5/" | tr -d '\r')
      if ! [[ "$prev1" =~ ^[[:space:]]*/// || "$prev2" =~ ^[[:space:]]*/// ]]; then
        # Summary from method name
        local sum="$name"; sum="${sum//_/ }"
        echo "${indent}/// <summary>" >> "$tmp"
        echo "${indent}/// ${sum}." >> "$tmp"
        echo "${indent}/// </summary>" >> "$tmp"
        if [[ -n "$params" ]]; then
          IFS=',' read -ra parts <<< "$params"
          for p in "${parts[@]}"; do
            p=$(echo "$p" | sed -E 's/<[^>]+>//g; s/\[|\]//g')
            pname=$(echo "$p" | awk '{print $NF}')
            [[ -n "$pname" ]] && echo "${indent}/// <param name=\"$pname\"></param>" >> "$tmp"
          done
        fi
        if [[ "$ret" != "void" ]]; then
          echo "${indent}/// <returns></returns>" >> "$tmp"
        fi
      fi
      echo "$line" >> "$tmp"
    else
      echo "$line" >> "$tmp"
    fi
    prev2="$prev1"; prev1="$line"
  done < "$file"
  mv "$tmp" "$file"
}

if (( APPLY == 0 )); then
  for entry in "${BATCH[@]}"; do
    f="${entry%%:*}"; c="${entry##*:}"
    echo "---- $f ($c insertions)"
  done
  echo "Dry-run complete. Re-run with -a to apply."
  exit 0
fi

backup_dir="$(cd "$(dirname "$0")" && pwd)/backup-xml-docs-$(date +%Y%m%d-%H%M%S)"
mkdir -p "$backup_dir"
changed=()
for entry in "${BATCH[@]}"; do
  f="${entry%%:*}"; cp "$f" "$backup_dir"/
  insert_docs "$f"
  changed+=("$f")
done
echo "Backed up ${#BATCH[@]} files to $backup_dir"
echo "Applied changes to ${#changed[@]} files."

if (( DO_GIT == 1 )); then
  git add -- "${changed[@]}" || true
  git commit -m "docs(tests): add XML docs (batch ${BATCH_PERCENT}%) [pass 1]" || true
  git add -- "${changed[@]}" || true
  git commit -m "docs(tests): refine XML docs (batch ${BATCH_PERCENT}%) [pass 2]" || true
  if (( DO_PUSH == 1 )); then git push || true; fi
fi

echo "Done."
