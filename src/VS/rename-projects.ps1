#requires -Version 7.0
param(
  [string]$Solution = 'IndFusion.sln',
  [string]$MapPath = 'VS/rename-map.json',
  [switch]$WhatIf,
  [switch]$UpdateSolution,
  [switch]$UpdateNamespaces
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Read-Map([string]$path) {
  if (-not (Test-Path $path)) { throw "Map file not found: $path" }
  $json = Get-Content -Raw -Path $path | ConvertFrom-Json
  return @($json)
}

function Get-Csproj([string]$dir) {
  $files = @(Get-ChildItem -Path $dir -Filter *.csproj)
  if ($files.Count -ne 1) { throw "Expected one .csproj in '$dir', found $($files.Count)" }
  return $files[0].FullName
}

function Ensure-Dir([string]$path) {
  if (-not (Test-Path $path)) { New-Item -ItemType Directory -Path $path | Out-Null }
}

function Move-ProjectDir([string]$oldDir, [string]$newDir) {
  Ensure-Dir (Split-Path -Parent $newDir)
  if ($WhatIf) { Write-Host "Would move dir: $oldDir -> $newDir"; return }
  Move-Item -Path $oldDir -Destination $newDir
}

function Update-CsprojMetadata([string]$csprojPath, [string]$newRootNs) {
  if ($WhatIf) { Write-Host "Would update csproj: $csprojPath (RootNamespace=$newRootNs)"; return }
  [xml]$xml = Get-Content -Raw -Path $csprojPath
  $pg = $xml.Project.SelectSingleNode('//PropertyGroup[RootNamespace]')
  if (-not $pg) { $pg = $xml.Project.SelectSingleNode('//PropertyGroup[1]') }
  if (-not $pg) { $pg = $xml.Project.AppendChild($xml.CreateElement('PropertyGroup')) }
  $rn = $pg.SelectSingleNode('RootNamespace')
  if (-not $rn) { $rn = $pg.AppendChild($xml.CreateElement('RootNamespace')) }
  $rn.InnerText = $newRootNs
  $xml.Save($csprojPath)
}

function Update-ProjectReferences([string]$repoRoot) {
  # Update Include paths that point to old locations
  $allCsproj = Get-ChildItem -Recurse -Path $repoRoot -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
  foreach ($f in $allCsproj) {
    $content = Get-Content -Raw -Path $f.FullName
    $orig = $content
    foreach ($m in $script:Map) {
      $old = [IO.Path]::GetFullPath((Join-Path $repoRoot $m.OldDir))
      $new = [IO.Path]::GetFullPath((Join-Path $repoRoot $m.NewDir))
      if (-not (Test-Path $old)) { continue }
      if (-not (Test-Path $new)) { continue }
      $oldProj = Get-Csproj $old
      $newProj = Get-Csproj $new
      $oldAbs = (Resolve-Path $oldProj | Select-Object -ExpandProperty Path)
      $newAbs = (Resolve-Path $newProj | Select-Object -ExpandProperty Path)
      $content = $content -replace [Regex]::Escape($oldAbs), $newAbs
    }
    if ($content -ne $orig) {
      if ($WhatIf) { Write-Host "Would update ProjectReferences in: $($f.FullName)" }
      else { Set-Content -Path $f.FullName -Value $content }
    }
  }
}

function Update-NamespacesInDir([string]$dir, [string]$oldRoot, [string]$newRoot) {
  $files = Get-ChildItem -Recurse -Path $dir -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
  foreach ($f in $files) {
    $text = Get-Content -Raw -Path $f.FullName
    $orig = $text
    $text = $text -replace "(?m)^namespace\s+${oldRoot}(?=\b)", "namespace ${newRoot}"
    $text = $text -replace "(?m)^using\s+${oldRoot}(?=\b)", "using ${newRoot}"
    if ($text -ne $orig) {
      if ($WhatIf) { Write-Host "Would update namespaces/usings in: $($f.FullName)" }
      else { Set-Content -Path $f.FullName -Value $text }
    }
  }
}

$repoRoot = Resolve-Path '.' | Select-Object -ExpandProperty Path
$script:Map = Read-Map -path $MapPath

Write-Host "Planned moves:" -ForegroundColor Cyan
foreach ($m in $Map) { Write-Host ("- {0} -> {1}  (RootNs={2})" -f $m.OldDir, $m.NewDir, $m.NewRootNamespace) }

if ($WhatIf) { Write-Host "WhatIf enabled: no changes will be made." }

# Move directories and update csproj RootNamespace
foreach ($m in $Map) {
  $oldAbs = Join-Path $repoRoot $m.OldDir
  $newAbs = Join-Path $repoRoot $m.NewDir
  $hadOld = Test-Path $oldAbs
  $hasNew = Test-Path $newAbs
  if ($hadOld) {
    Move-ProjectDir -oldDir $oldAbs -newDir $newAbs
    if (-not $WhatIf) {
      $csproj = Get-Csproj $newAbs
      Update-CsprojMetadata -csprojPath $csproj -newRootNs $m.NewRootNamespace
    }
  } elseif ($hasNew) {
    if ($WhatIf) { Write-Host "Would update csproj (existing new dir): $newAbs" }
    else {
      $csproj = Get-Csproj $newAbs
      Update-CsprojMetadata -csprojPath $csproj -newRootNs $m.NewRootNamespace
    }
  } else {
    Write-Warning "Missing both old and new dirs for map entry: $($m.OldDir) -> $($m.NewDir)"
  }
}

if ($UpdateSolution) {
  # Try to update solution membership (requires dotnet CLI)
  foreach ($m in $Map) {
    $oldAbs = Join-Path $repoRoot $m.OldDir
    $newAbs = Join-Path $repoRoot $m.NewDir
    if (-not (Test-Path $newAbs)) { continue }
    $oldProj = (Get-Csproj $newAbs) -replace [Regex]::Escape($m.NewDir), $m.OldDir
    $newProj = Get-Csproj $newAbs
    if (-not $WhatIf) {
      try { dotnet sln $Solution remove $oldProj 2>$null | Out-Null } catch {}
      try { dotnet sln $Solution add $newProj   2>$null | Out-Null } catch {}
    } else {
      Write-Host "Would update solution: remove $oldProj, add $newProj"
    }
  }
}

# Update ProjectReference Include paths after moves
if (-not $WhatIf) { Update-ProjectReferences -repoRoot $repoRoot }

if ($UpdateNamespaces -and -not $WhatIf) {
  # Best-effort update of root namespaces
  foreach ($m in $Map) {
    $newAbs = Join-Path $repoRoot $m.NewDir
    # infer old root from first segment of OldDir (ExxerFactor or ExxerRules)
    $oldRoot = if ($m.OldDir -like 'ExxerFactor*') { 'ExxerFactor' } elseif ($m.OldDir -like 'ExxerRules*') { 'ExxerRules' } else { $null }
    if ($oldRoot) { Update-NamespacesInDir -dir $newAbs -oldRoot $oldRoot -newRoot ($m.NewRootNamespace -replace '\.[^.]+$','') }
  }
}

Write-Host "Rename completed. Review changes and build the solution."
