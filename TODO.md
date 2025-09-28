CI/CD TODO

Short checklist to finish and operate the pipelines after moving folders/repo.

- Secrets (GitHub → Settings → Secrets and variables → Actions)
  - NUGET_API_KEY: nuget.org API key (scoped Push).
  - VSCE_TOKEN: Azure DevOps PAT with scope Marketplace (Publish).
  - MARKETPLACE_PAT: same PAT (optional; used for VSIX CLI publish).

- NuGet (publish-nuget workflow)
  - Verify packages pack on runner: Actions → publish-nuget.
  - Confirm .snupkg symbols are pushed (Source Link already configured by Directory.Build.props).
  - Align repository URLs in `Directory.Build.props` with the correct GitHub repo.

- VS Code Marketplace (publish-vscode workflow)
  - Ensure `src/VSCode/package.json` has correct `publisher` (hebel), `icon`, and README screenshots.
  - Run Actions → publish-vscode. Marketplace listing should show icon and README.

- Visual Studio (VSIX) (publish-vsix workflow)
  - Workflow currently builds VSIX via MSBuild and collects `ExxerRules.Vsix.vsix`.
  - Optional: enable automated publish
    - Create a minimal `vs-publish.json` (publish manifest) with correct metadata.
    - Uncomment the publish step in `.github/workflows/publish-vsix.yml` and commit.
  - Manual alternative: upload the built VSIX from `src/VS/ExxerRules/ExxerRules.Vsix/bin/Release/` in Marketplace Manage.

- Triggers (optional hardening)
  - Change workflows to trigger on tags (e.g., `v*`) and/or releases instead of only manual dispatch.
  - Example: add `on: push: tags: - 'v*'` to workflows when you’re ready.

- Quality gates (optional)
  - Add Stryker mutation testing job; fail pipeline on surviving mutants.
  - Add unit test step (xUnit) and test coverage artifact.
  - Add `dotnet format`/lint as a pre-pack step.

- VSIX metadata
  - `source.extension.vsixmanifest`: Publisher must match your Marketplace publisher display name; Icon/Preview files must exist.
  - Images live in `src/VS/ExxerRules/Icons/` and are linked into the VSIX.

- Versioning
  - VS Code: bump `src/VSCode/package.json` version before each publish.
  - NuGet: bump `PackageVersion` in csproj(s) before packing.
  - VSIX: bump `Version` in `source.extension.vsixmanifest` for marketplace updates.

- Signing (optional)
  - If you plan to sign NuGet packages, register signing cert at nuget.org and add a signing step.

Notes
  - PATs are generated in Azure DevOps and can be reused for VS Code and VSIX publish.
  - Keep secrets out of logs; rotate any key that was shared.
