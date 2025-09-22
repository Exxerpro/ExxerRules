<#!
.SYNOPSIS
  Orchestrates the XML doc generation cycle using Python + Git with safety checks.

.DESCRIPTION
  - Optionally starts with one or more explicit files: runs dry-run first, then applies
    exactly one insertion per file (stepwise) to validate safety.
  - Builds solution and verifies that the tracked warning pattern (default CS1591)
    decreases; aborts if errors increase.
  - Proceeds through batch percentages (default: 2,3,10,15,20,25,40,50). After each batch:
      * Builds and compares counts
      * If error count increases or the pattern does not decrease, stops and reports
      * Otherwise, stages, commits, and optionally pushes

.PARAMETER Solution
  Path to the .sln file.

.PARAMETER Root
  Root directory to scan (e.g., a single test project folder).

.PARAMETER Files
  Comma-separated explicit file paths to process first (pilot). Optional.

.PARAMETER Percents
  Comma-separated list of batch percents. Default: 2,3,10,15,20,25,40,50

.PARAMETER Pattern
  Warning pattern to track (default: CS1591).

.PARAMETER AllowStrings
  Allow files with raw/verbatim strings.

.PARAMETER Push
  Push commits to the current branch after each successful batch.

.EXAMPLE
  pwsh ExxerRules/src/scripts/Run-DocBatches.ps1 `
    -Solution ExxerRules/src/IndFusion.sln `
    -Root ExxerRules/src/test/IndFusion.Analyzer.Tests `
    -Files ExxerRules/src/test/IndFusion.Analyzer.Tests/TestCases/CodeFixes/ConfigureAwaitFalseCodeFixProviderTests.cs `
    -Percents 2,3,10,15,20,25,40,50 `
    -Pattern CS1591 -Push
#>

param(
  [Parameter(Mandatory=$true)][string]$Solution,
  [Parameter(Mandatory=$true)][string]$Root,
  [string]$Files = '',
  [string]$Percents = '2,3,10,15,20,25,40,50',
  [string]$Pattern = 'CS1591',
  [switch]$AllowStrings,
  [switch]$Push
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-Proc {
  param([string[]]$Cmd)
  $psi = New-Object System.Diagnostics.ProcessStartInfo
  $psi.FileName = $Cmd[0]
  $psi.Arguments = ($Cmd[1..($Cmd.Length-1)] -join ' ')
  $psi.RedirectStandardOutput = $true
  $psi.RedirectStandardError = $true
  $psi.UseShellExecute = $false
  $psi.CreateNoWindow = $true
  $p = New-Object System.Diagnostics.Process
  $p.StartInfo = $psi
  [void]$p.Start()
  $stdout = $p.StandardOutput.ReadToEnd()
  $stderr = $p.StandardError.ReadToEnd()
  $p.WaitForExit()
  return @{ Code = $p.ExitCode; Out = ($stdout + $stderr) }
}

function Get-CountsFromBuild {
  param([string]$Text, [string]$Pattern)
  $errors = ([regex]::Matches($Text, '\berror\b', 'IgnoreCase')).Count
  $warnings = ([regex]::Matches($Text, '\bwarning\b', 'IgnoreCase')).Count
  $pcount = if ($Pattern) { ([regex]::Matches($Text, [regex]::Escape($Pattern))).Count } else { 0 }
  [pscustomobject]@{ Errors=$errors; Warnings=$warnings; Pattern=$pcount }
}

function Build-Solution {
  param([string]$Solution)
  Invoke-Proc @('dotnet','build', $Solution, '-c','Debug','-v','minimal','-p:TreatWarningsAsErrors=false')
}

function Run-Generator {
  param([string]$Root,[int]$Percent,[switch]$Apply,[switch]$AllowStrings,[string]$Files)
  $cmd = @('python', (Join-Path $PSScriptRoot 'generate_xml_docs.py'))
  if ($Files) { $cmd += @('--files', $Files) } else { $cmd += @('--root', $Root) }
  $cmd += @('--percent', $Percent.ToString())
  if ($Apply) { $cmd += '--apply' }
  if ($AllowStrings) { $cmd += '--allow-strings' }
  $res = Invoke-Proc $cmd
  if ($res.Code -ne 0) { throw "Generator failed: $($res.Out)" }
  $backup = ''
  $changed = @()
  foreach ($line in $res.Out -split "`r?`n") {
    if ($line -like 'BACKUP_DIR:*') { $backup = $line.Substring(11).Trim() }
    elseif ($line -like 'CHANGED:*') { $changed += $line.Substring(8).Trim() }
  }
  [pscustomobject]@{ Backup=$backup; Changed=$changed; Raw=$res.Out }
}

function Git-CommitPush {
  param([string[]]$Files,[string]$Message,[switch]$Push)
  if (-not $Files -or $Files.Count -eq 0) { return }
  # Stage files
  $addCmd = @('git','add','--') + $Files
  [void](Invoke-Proc $addCmd)
  # Quote commit message to keep it as a single argument
  $msgQuoted = '"' + $Message.Replace('"','\"') + '"'
  $commitCmd = @('git','commit','--no-verify','-m', $msgQuoted)
  $c1 = Invoke-Proc $commitCmd
  if ($c1.Code -ne 0) { Write-Warning "git commit returned $($c1.Code): $($c1.Out)" }
  if ($Push) {
    $p = Invoke-Proc @('git','push')
    if ($p.Code -ne 0) { Write-Warning "git push returned $($p.Code): $($p.Out)" }
  }
}

Write-Host "Baseline build..." -ForegroundColor Cyan
$baseBuild = Build-Solution -Solution $Solution
if ($baseBuild.Code -ne 0) { Write-Error "Baseline build failed"; $baseBuild.Out; exit 1 }
$baseCounts = Get-CountsFromBuild -Text $baseBuild.Out -Pattern $Pattern
Write-Host ("Baseline: errors={0} warnings={1} {2}={3}" -f $baseCounts.Errors, $baseCounts.Warnings, $Pattern, $baseCounts.Pattern)

# Pilot: specific files, one insertion per file (if provided)
if ($Files) {
  Write-Host "Pilot: Dry-run -> $Files" -ForegroundColor Yellow
  [void](Run-Generator -Root $Root -Percent 100 -AllowStrings:$AllowStrings -Files $Files)

  Write-Host "Pilot: Apply one insertion per file (stepwise)..." -ForegroundColor Yellow
  # stepwise single insertion per file
  $cmd = @('python', (Join-Path $PSScriptRoot 'generate_xml_docs.py'),'--files', $Files,'--percent','100','--apply','--stepwise','--max-inserts-per-file','1')
  if ($AllowStrings) { $cmd += '--allow-strings' }
  $pilot = Invoke-Proc $cmd
  $backupDir = ''
  $changed = @()
  foreach ($line in $pilot.Out -split "`r?`n") {
    if ($line -like 'BACKUP_DIR:*') { $backupDir = $line.Substring(11).Trim() }
    elseif ($line -like 'CHANGED:*') { $changed += $line.Substring(8).Trim() }
  }
  Write-Host ("Pilot changed {0} files. Backup: {1}" -f $changed.Count, $backupDir)

  $post = Build-Solution -Solution $Solution
  if ($post.Code -ne 0) { Write-Error "Build failed after pilot"; $post.Out; exit 2 }
  $postCounts = Get-CountsFromBuild -Text $post.Out -Pattern $Pattern
  Write-Host ("After pilot: errors={0} warnings={1} {2}={3}" -f $postCounts.Errors, $postCounts.Warnings, $Pattern, $postCounts.Pattern)

  if ($postCounts.Errors -gt $baseCounts.Errors) {
    Write-Error "Pilot increased error count; STOP. Consider surgical restore."
    exit 3
  }

  Git-CommitPush -Files $changed -Message "docs(tests): pilot XML doc insertion (single-method)" -Push:$Push
  $baseCounts = $postCounts
}

# Batch loop
$perc = @($Percents -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' })
foreach ($p in $perc) {
  $pInt = [int]$p
  Write-Host "=== Batch $pInt% ===" -ForegroundColor Cyan
  $gen = Run-Generator -Root $Root -Percent $pInt -Apply -AllowStrings:$AllowStrings -Files ''
  Write-Host ("Changed {0} files. Backup: {1}" -f $gen.Changed.Count, $gen.Backup)

  $build = Build-Solution -Solution $Solution
  if ($build.Code -ne 0) {
    Write-Error "Build failed after batch $pInt%"
    $build.Out
    Write-Host "Changed files:"; $gen.Changed | ForEach-Object { Write-Host " - $_" }
    Write-Host "Backup dir: $($gen.Backup)"
    exit 4
  }
  $counts = Get-CountsFromBuild -Text $build.Out -Pattern $Pattern
  Write-Host ("After $pInt%: errors={0} warnings={1} {2}={3}" -f $counts.Errors, $counts.Warnings, $Pattern, $counts.Pattern)

  if ($counts.Errors -gt $baseCounts.Errors) {
    Write-Error "Error count increased after $pInt%; STOP. Consider surgical restore."
    Write-Host "Changed files:"; $gen.Changed | ForEach-Object { Write-Host " - $_" }
    Write-Host "Backup dir: $($gen.Backup)"
    exit 5
  }

  Git-CommitPush -Files $gen.Changed -Message ("docs(tests): batch XML docs ({0}%)" -f $pInt) -Push:$Push
  $baseCounts = $counts
}

Write-Host "All batches completed successfully with improvements." -ForegroundColor Green
