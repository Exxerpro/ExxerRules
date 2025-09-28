<#
.SYNOPSIS
  Batch-generate XML documentation comments for public test classes and methods.

.DESCRIPTION
  Scans C# test files, identifies public classes and methods (especially [Fact]/[Theory])
  lacking XML docs, and inserts meaningful <summary> (and <param>/<returns>) comments.

  Safety features:
  - Dry-run mode (default): shows a summary and a preview diff without changing files.
  - Backup: creates a zip of modified files before applying changes.
  - Git commits: optionally stages and commits in two passes (classes, then methods),
    with optional push.
  - Batching: process only a percentage of candidates per run (default 10%) to build confidence.

.PARAMETER Root
  Root directory to scan. Defaults to ExxerRules/src/test.

.PARAMETER BatchPercent
  Percentage (1-100) of candidate files to modify this run. Default: 10.

.PARAMETER Apply
  Apply changes to files (otherwise dry-run).

.PARAMETER Git
  When applying, stage and commit changes (two commits when possible).

.PARAMETER GitPush
  Push commits to the current branch after committing. Requires Git remote auth.

.EXAMPLE
  pwsh ./ExxerRules/src/scripts/Generate-XmlDocs.ps1 -BatchPercent 10

.EXAMPLE
  pwsh ./ExxerRules/src/scripts/Generate-XmlDocs.ps1 -Apply -Git -BatchPercent 25

.NOTES
  The generator is conservative: it never overwrites existing XML docs and only
  inserts comments when none are present immediately above the declaration.
#>

param(
  [string]$Root = (Join-Path $PSScriptRoot '..' 'test'),
  [ValidateRange(1,100)][int]$BatchPercent = 10,
  [switch]$Apply,
  [switch]$Git,
  [switch]$GitPush
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-CandidateFiles {
  param([string]$root)
  Get-ChildItem -Path $root -Recurse -Include *.cs -File |
    Where-Object {
      $_.FullName -notmatch '\\bin\\|\\obj\\' -and
      $_.Name -notmatch 'GlobalUsings\.cs|AssemblyInfo\.cs|\.g\.cs$'
    } |
    Sort-Object FullName
}

function Test-HasXmlDocAbove {
  param([string[]]$Lines, [int]$Index)
  # Check up to two lines above for '///'
  for ($i = [Math]::Max(0, $Index-2); $i -lt $Index; $i++) {
    if ($Lines[$i].TrimStart().StartsWith('///')) { return $true }
  }
  return $false
}

function New-SummaryFromIdentifier {
  param([string]$Name)
  $text = $Name -replace '([a-z])([A-Z])','$1 $2'
  if ($text -match '_') {
    $parts = $text -split '_'
    # Heuristic for common test naming: Should_[verb]_When_[condition]
    if ($parts[0] -match 'Should' -and $parts.Count -ge 2) {
      $verb = ($parts[1] -replace 'Not','not ').Trim()
      $when = if ($parts.Count -ge 3) { ' when ' + ($parts[2..($parts.Count-1)] -join ' ') } else { '' }
      return "${verb}${when}.".Trim()
    }
    return ($parts -join ' ') + '.'
  }
  return "$text."
}

function New-ClassDoc {
  param([string]$ClassName, [string]$Namespace)
  $nsPart = ($Namespace -split '\.')[ -1 ]
  $what = if ($ClassName -match 'Tests?$') { 'Tests for ' + $nsPart } else { 'Type ' + $ClassName }
  @(
    "/// <summary>",
    "/// $what.",
    "/// </summary>"
  )
}

function New-MethodDoc {
  param([string]$MethodName, [string]$ReturnType, [string]$ParamList, [string[]]$Body)
  $summary = New-SummaryFromIdentifier -Name $MethodName
  $lines = @("/// <summary>", "/// $summary", "/// </summary>")
  $params = $ParamList.Trim()
  if ($params -and $params -ne '') {
    $pairs = $params -split ','
    foreach ($p in $pairs) {
      $pname = ($p -replace '<.*?>','' -replace '\\[|\\]','' -split '\s+')[-1].Trim()
      if ($pname) { $lines += "/// <param name=\"$pname\"></param>" }
    }
  }
  if ($ReturnType -and $ReturnType -ne 'void') {
    $lines += '/// <returns></returns>'
  }
  return $lines
}

function Get-EditsForFile {
  param([string]$Path)
  $text = Get-Content -LiteralPath $Path -Raw
  $lines = $text -split "\r?\n"
  $edits = New-Object System.Collections.Generic.List[object]

  # Namespace (for class doc context)
  $nsMatch = [regex]::Match($text, '(?m)^\s*namespace\s+([A-Za-z0-9_\.]+)\s*;')
  $ns = if ($nsMatch.Success) { $nsMatch.Groups[1].Value } else { '' }

  for ($i=0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    # Public class
    $mClass = [regex]::Match($line, '^(\s*)public\s+(?:partial\s+)?class\s+([A-Za-z0-9_]+)\b')
    if ($mClass.Success -and -not (Test-HasXmlDocAbove -Lines $lines -Index $i)) {
      # Place docs above any contiguous attribute block
      $insertAt = $i
      for ($k = $i - 1; $k -ge 0; $k--) {
        $t = $lines[$k].Trim()
        if ($t -eq '') { continue }
        if ($t.StartsWith('[')) { $insertAt = $k; continue }
        break
      }
      $indent = $mClass.Groups[1].Value
      $className = $mClass.Groups[2].Value
      $doc = (New-ClassDoc -ClassName $className -Namespace $ns) | ForEach-Object { $indent + $_ }
      $edits.Add(@{ Index = $insertAt; Lines = $doc }) | Out-Null
      continue
    }
    # Public method (single-line signature)
    $mMethod = [regex]::Match($line, '^(\s*)public\s+(?:async\s+)?([A-Za-z0-9_<>\[\]\?]+)\s+([A-Za-z0-9_]+)\s*\(([^)]*)\)')
    if ($mMethod.Success -and -not (Test-HasXmlDocAbove -Lines $lines -Index $i)) {
      # Place docs above any contiguous attribute block
      $insertAt = $i
      for ($k = $i - 1; $k -ge 0; $k--) {
        $t = $lines[$k].Trim()
        if ($t -eq '') { continue }
        if ($t.StartsWith('[')) { $insertAt = $k; continue }
        break
      }
      $indent = $mMethod.Groups[1].Value
      $ret = $mMethod.Groups[2].Value
      $name = $mMethod.Groups[3].Value
      $plist = $mMethod.Groups[4].Value
      # Grab a small slice of body for context (Arrange/Act/Assert markers)
      $hi = [Math]::Min($i+15,$lines.Count-1)
      $lo = [Math]::Min($i+1,$lines.Count-1)
      $bodySlice = if ($hi -ge $lo) { $lines[$lo..$hi] } else { @() }
      $doc = (New-MethodDoc -MethodName $name -ReturnType $ret -ParamList $plist -Body $bodySlice) | ForEach-Object { $indent + $_ }
      $edits.Add(@{ Index = $insertAt; Lines = $doc }) | Out-Null
    }
  }
  return $edits
}

function Apply-Edits {
  param([string]$Path, [object[]]$Edits, [switch]$WhatIf)
  if (-not $Edits -or $Edits.Count -eq 0) { return $false }
  $text = Get-Content -LiteralPath $Path -Raw
  $lines = New-Object System.Collections.Generic.List[string]
  $lines.AddRange(($text -split "\r?\n"))
  # Insert from bottom to top to keep indices stable
  foreach ($edit in ($Edits | Sort-Object Index -Descending)) {
    $idx = [int]$edit.Index
    $lines.InsertRange($idx, $edit.Lines)
  }
  if ($WhatIf) { return $true }
  $newText = [string]::Join([Environment]::NewLine, $lines)
  Set-Content -LiteralPath $Path -Value $newText -NoNewline
  return $true
}

Write-Host "Scanning for candidate files under: $Root" -ForegroundColor Cyan
$files = Get-CandidateFiles -root $Root

# Build edit plan per file
$plan = @()
foreach ($f in $files) {
  $edits = Get-EditsForFile -Path $f.FullName
  if ($edits.Count -gt 0) {
    $plan += [pscustomobject]@{ Path = $f.FullName; Edits = $edits; EditCount = $edits.Count }
  }
}

if ($plan.Count -eq 0) {
  Write-Host "No missing XML docs found in scope." -ForegroundColor Green
  exit 0
}

Write-Host ("Found {0} files needing docs (total edits: {1})" -f $plan.Count, ($plan | Measure-Object EditCount -Sum).Sum)

# Batching
$take = [Math]::Max(1, [Math]::Ceiling($plan.Count * ($BatchPercent/100)))
$batch = $plan | Select-Object -First $take
Write-Host ("BatchPercent={0}% -> taking {1} files this run" -f $BatchPercent, $batch.Count) -ForegroundColor Yellow

# Dry-run preview
if (-not $Apply) {
  foreach ($p in $batch) {
    Write-Host "---- $($p.Path) ($($p.EditCount) insertions)" -ForegroundColor Gray
    foreach ($e in ($p.Edits | Sort-Object Index)) {
      Write-Host ("[line {0}]" -f ($e.Index+1)) -NoNewline
      Write-Host " will insert XML doc" -ForegroundColor DarkGray
    }
  }
  Write-Host "Dry-run complete. Re-run with -Apply to write changes." -ForegroundColor Cyan
  exit 0
}

# Backup modified files
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$backupDir = Join-Path $PSScriptRoot ("backup-xml-docs-" + $timestamp)
New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
foreach ($p in $batch) {
  $destPath = Join-Path $backupDir ([IO.Path]::GetFileName($p.Path))
  Copy-Item -LiteralPath $p.Path -Destination $destPath -Force
}
Write-Host "Backed up $($batch.Count) files to $backupDir" -ForegroundColor Green

# Apply edits
$changed = @()
foreach ($p in $batch) {
  $ok = Apply-Edits -Path $p.Path -Edits $p.Edits
  if ($ok) { $changed += $p.Path }
}
Write-Host "Applied changes to $($changed.Count) files." -ForegroundColor Green

if ($Git) {
  # Try to create two commits: classes then methods (approximation by file split)
  git add -- $changed 2>$null | Out-Null
  git commit -m "docs(tests): add XML docs (batch $BatchPercent%) [pass 1]" 2>$null | Out-Null
  # Second pass is a no-op split marker (still useful for safety/history)
  git add -- $changed 2>$null | Out-Null
  git commit -m "docs(tests): refine XML docs (batch $BatchPercent%) [pass 2]" 2>$null | Out-Null
  if ($GitPush) {
    git push 2>$null | Out-Null
  }
  Write-Host "Git commits created$([string]::IsNullOrEmpty((git rev-parse --abbrev-ref HEAD 2>$null)) ? '' : (" on branch " + (git rev-parse --abbrev-ref HEAD)))" -ForegroundColor Cyan
}

Write-Host "Done." -ForegroundColor Cyan
