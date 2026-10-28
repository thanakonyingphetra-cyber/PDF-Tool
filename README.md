# PDF Tool Portable

Offline Windows x64 PDF utility (prototype).

> **Status:** v0.1 development. NOT yet tested or approved for corporate deployment.
> No downloaded executable is guaranteed to be safe or policy-compliant without IT approval.

## Current v0.1 source

- Windows Forms user interface, file selection and drag/drop
- Merge PDFs in list order
- Split PDFs into one-page files; batch split
- Save beside the first input (merge) or original input (split)
- Use unique names rather than overwrite output files
- No network calls, telemetry, updater or Registry changes in application code

**Not yet implemented:** deletion/reordering/rotation/extraction of pages,
image conversion, Office conversion, compression, watermarks, numbering,
password tools and page previews. See [roadmap](docs/ROADMAP.md).

## Portable packaging

GitHub Actions compiles on a Windows runner with .NET 10 SDK, executes
smoke tests and publishes a self-contained, single-file target for Windows x64.
A successful workflow produces the build artifact:

`PDF-Tool-Portable-win-x64` containing `PDF-Tool-Portable-win-x64.zip`
and its SHA-256 file.

To test after a successful run:
1. In GitHub, open **Actions → Build and test Windows portable app**.
2. Select the successful workflow run and download the artifact.
3. Extract the outer artifact ZIP, then the portable ZIP.
4. Have IT/security approve and scan it before running on a company machine.
5. On an approved test Windows 10/11 x64 computer, run `PDF-Tool.exe`.

If the build fails or no artifact is present, there is **no validated downloadable EXE** yet.
Unsigned binaries may produce Microsoft SmartScreen/enterprise endpoint warnings.
Do not bypass enterprise security controls.

## Local developer build

On an authorized Windows development machine with .NET 10 SDK:

```powershell
dotnet restore src/PdfTool/PdfTool.csproj
dotnet run --project src/PdfTool/PdfTool.csproj
```

For a self-contained build:

```powershell
dotnet publish src/PdfTool/PdfTool.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o dist/portable
```

Never use sensitive company PDFs for initial tests. PDFs can be malformed;
the prototype is not a hardened PDF security boundary.

## Project documentation

- [Feature roadmap](docs/ROADMAP.md)
- [IT approval checklist](docs/IT-APPROVAL-CHECKLIST.md)
- [Third-party licensing notes](THIRD_PARTY_NOTICES.md)

## Privacy and deployment

The application processes selected PDFs locally without network requests.
The GitHub hosted **build** uses network access for source checkout/package restore.
Portable means **no installation steps**, not permission to bypass corporate policy.
