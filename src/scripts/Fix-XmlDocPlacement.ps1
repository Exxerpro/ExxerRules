<#
.SYNOPSIS
  Repairs malformed XML doc insertions like ': BaseType/// <summary>' by moving the doc block above the class line.

.DESCRIPTION
  Scans C# files under a root (default: ExxerRules/src/test) and detects cases where XML doc markers
  (/// <summary> ... ) were appended to the end of a class declaration line (typically after ': BaseType').
  It extracts that doc block and inserts it on separate lines immediately above the class line with correct indentation.

.PARAMETER Root
  Root directory to scan. Defaults to ExxerRules/src/test.

.PARAMETER Apply
  Apply changes. Otherwise runs in dry-run mode and reports candidates only.

.EXAMPLE
  pwsh ./ExxerRules/src/scripts/Fix-XmlDocPlacement.ps1

.EXAMPLE
  pwsh ./ExxerRules/src/scripts/Fix-XmlDocPlacement.ps1 -Apply
#>

param(
  [string]$Root = (Join-Path $PSScriptRoot '..' 'test'),
  [switch]$Apply
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-CandidateFiles {
  param([string]$root)
  Get-ChildItem -Path $root -Recurse -Include *.cs -File |
    Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' } |
    Sort-Object FullName
}

function Find-MalformedBlocks {
  param([string]$Path)
  $text = Get-Content -LiteralPath $Path -Raw
  $lines = $text -split "\r?\n"
  $results = @()
  $classPattern = '^(\s*)(public\s+(?:partial\s+)?class\s+\w+.*)$'
  for ($i=0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match $classPattern) {
      # If we see '///' anywhere on this or immediate following lines with a preceding colon prefix pattern, mark it
      $hasInlineDoc = $false
      $docStartIndex = -1
      for ($j=$i; $j -le [Math]::Min($i+3, $lines.Count-1); $j++) {
        if ($lines[$j] -match '///\s*<summary>') { $hasInlineDoc = $true; $docStartIndex = $j; break }
      }
      if ($hasInlineDoc -and $docStartIndex -ge 0) {
        # If the summary marker is not at the beginning of line (after whitespace), consider malformed
        $trimmed = $lines[$docStartIndex].TrimStart()
        if (-not $trimmed.StartsWith('///')) {
          $results += [pscustomobject]@{ ClassLine = $i; DocStart = $docStartIndex }
        }
      }
    }
  }
  return $results
}

function Repair-File {
  param([string]$Path)
  $text = Get-Content -LiteralPath $Path -Raw
  $lines = New-Object System.Collections.Generic.List[string]
  $lines.AddRange(($text -split "\r?\n"))
  $repaired = $false
  $classPattern = '^(\s*)(public\s+(?:partial\s+)?class\s+\w+.*)$'

  for ($i=0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match $classPattern) {
      $indent = $Matches[1]
      # Locate doc start within next few lines
      $docStart = -1
      for ($j=$i; $j -le [Math]::Min($i+5, $lines.Count-1); $j++) {
        if ($lines[$j] -match '///\s*<summary>') { $docStart = $j; break }
      }
      if ($docStart -ge 0) {
        # Only fix if the summary line is not aligned at start (malformed append)
        $t = $lines[$docStart].TrimStart()
        if (-not $t.StartsWith('///')) {
          # Collect doc lines
          $docLines = New-Object System.Collections.Generic.List[string]
          for ($k=$docStart; $k -lt $lines.Count; $k++) {
            $tline = $lines[$k].TrimStart()
            if ($tline.StartsWith('///')) { $docLines.Add($indent + $tline); $lines[$k] = ($lines[$k] -replace '.*///','').TrimStart(); continue }
            break
          }
          # Clean empty tails left by removal on affected lines
          for ($k=$docStart; $k -ge $i; $k--) {
            $lines[$k] = $lines[$k].TrimEnd()
          }
          # Insert the doc above class line
          if ($docLines.Count -gt 0) {
            $lines.InsertRange($i, $docLines)
            $repaired = $true
            # Advance index to skip over inserted block
            $i += $docLines.Count
          }
        }
      }
    }
  }
  if ($repaired) {
    $newText = [string]::Join([Environment]::NewLine, $lines)
    Set-Content -LiteralPath $Path -Value $newText -NoNewline
  }
  return $repaired
}

Write-Host "Scanning for malformed inline XML docs under: $Root" -ForegroundColor Cyan
$files = Get-CandidateFiles -root $Root
$candidates = @()
foreach ($f in $files) {
  $hits = Find-MalformedBlocks -Path $f.FullName
  if ($hits.Count -gt 0) { $candidates += $f.FullName }
}

if ($candidates.Count -eq 0) {
  Write-Host "No malformed inline XML docs found." -ForegroundColor Green
  exit 0
}

Write-Host ("Found {0} files with malformed inline XML docs" -f $candidates.Count) -ForegroundColor Yellow
foreach ($p in $candidates) { Write-Host " - $p" }

if (-not $Apply) {
  Write-Host "Dry-run complete. Re-run with -Apply to repair." -ForegroundColor Cyan
  exit 0
}

# Backup
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$backupDir = Join-Path $PSScriptRoot ("backup-fix-placement-" + $timestamp)
New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
foreach ($p in $candidates) {
  Copy-Item -LiteralPath $p -Destination (Join-Path $backupDir ([IO.Path]::GetFileName($p))) -Force
}

$fixed=0
foreach ($p in $candidates) {
  if (Repair-File -Path $p) { $fixed++ }
}

Write-Host "Repaired $fixed of $($candidates.Count) files. Backups at $backupDir" -ForegroundColor Green
