# Offline NuGet workflow (local-only)

## 1) Update a package version (central management)
- Edit `src/Directory.Packages.props` and change the `Version` for the `PackageVersion` you want.
- Save the file.

## 2) Download all referenced packages to the local cache
```powershell
pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass -File src/VS/fetch-packages.ps1 -Source https://api.nuget.org/v3/index.json -OutputSubPath artifacts/nuget/offline -SkipDownloadIfExists
```
- Adds all packages (and dependencies) from `Directory.Packages.props` and any `.csproj` with explicit versions into `artifacts/nuget/offline`.
- Use `-DryRun` to list packages without downloading.

## 3) Download a single package/version on demand
- Using nuget.exe (gets full dependency closure; add -Prerelease for prerelease versions):
```powershell
$nugetUrl = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
$nugetExe = "$env:TEMP\nuget.exe"; Invoke-WebRequest $nugetUrl -OutFile $nugetExe -UseBasicParsing
& $nugetExe install <PackageId> -Version <Version> -Source https://api.nuget.org/v3/index.json -OutputDirectory artifacts/nuget/offline -DependencyVersion Highest -NonInteractive
```
- Using dotnet (just the nupkg; no dependency closure):
```powershell
dotnet nuget download <PackageId> --version <Version> --source https://api.nuget.org/v3/index.json --output artifacts/nuget/offline
```

## 4) Restore and build using ONLY the local feed
```powershell
cd src
# Uses src/NuGet.config (offline source only). For fallback to nuget.org use `NuGet.online.config`.
dotnet restore IndFusion.sln --configfile NuGet.config
dotnet build   IndFusion.sln --configfile NuGet.config -c Release
```

## Notes
- If restore fails with "package not found", fetch that package (step 3) and rerun restore.
- The offline source is `..\artifacts\nuget\offline` as configured in `src/NuGet.config`.
- For prerelease versions, include `-Prerelease` in the nuget.exe install command when the version contains a suffix (e.g., `-preview`).
- If downloads seem too fast or nothing appears in `artifacts/nuget/offline`, run the script with `-DryRun` to confirm discovered packages, and verify you are using PowerShell 7+ (`#requires -Version 7.0`).
