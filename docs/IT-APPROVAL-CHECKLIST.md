# PDF Tool Portable - IT/security approval checklist

Status: **Prototype; not yet approved for company use.**

## Intended behavior

- Windows 10/11 x64 graphical program, delivered as ZIP; no installer.
- Runs without elevated privileges (provided user's working folders are writable).
- No Registry edits, shell integration, services, startup tasks, scheduled tasks, or background updater.
- PDF processing is local; the application code does not upload documents or contain telemetry.
- Reads only PDF files selected or dropped by the user.
- Writes results to the selected source's directory. Never overwrites existing output.
- Does not log PDF contents. Errors can display file paths to the user locally.

## Review before deploying at work

- [ ] IT/security team explicitly approves executable and dependency list.
- [ ] IT accepts unsigned binaries or an organizational code-signing process is completed.
- [ ] Confirm package licensing and include full third-party notices and an SBOM.
- [ ] Pin/verify GitHub Actions versions and NuGet packages, review supply-chain risks.
- [ ] Check Windows Defender/EDR and application control (WDAC/AppLocker) policies.
- [ ] Scan distributed ZIP and executable with the approved corporate malware scanner.
- [ ] Test with synthetic/public sample documents only; no real confidential PDFs.
- [ ] Confirm applicable document classification and data handling policies.
- [ ] Confirm writable destination folders and acceptable file retention.
- [ ] Validate malicious/corrupted PDF handling and set resource limits before production rollout.
- [ ] Verify package SHA-256 checksum from an authorized release channel.
- [ ] Confirm Office conversion approach with IT: automation may require licensed Office and may be prohibited.
- [ ] Obtain approval before introducing any network access, telemetry, auto-update, or context-menu integration.

## Security caveats

Portable is not a permission bypass and does not guarantee software complies with
company policy. This prototype has NOT been pen-tested or code-signed. Some inputs
(password-protected, malformed, exceptionally large PDFs) may be unsupported or fail.
Self-contained .NET may extract native components to a user temp directory at runtime;
IT should review that behavior. Use an approved sandbox/test machine first.

GitHub Actions build and dependency restore use the network on GitHub's hosted
build runner. End-user PDF processing does not require network access.
