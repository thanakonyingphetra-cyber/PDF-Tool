# Feature roadmap

Target: an offline Windows x64 PDF utility that runs without an installer.

## v0.1 - Foundation (prototype)
- [x] Basic WinForms interface with drag-and-drop
- [x] Merge PDFs in explicit list order (basic, unencrypted files)
- [x] Split PDF into single-page PDFs (basic)
- [x] Avoid overwriting existing outputs
- [x] Batch split from a list
- [x] CI definition for self-contained win-x64 ZIP and SHA-256
- [ ] CI build, Windows smoke test and manual GUI acceptance must pass
- [ ] Security, license and IT approval

## v0.2 - Page editor
- [ ] Delete selected pages without changing the source
- [ ] Reorder pages
- [ ] Rotate pages
- [ ] Extract selected pages/ranges

## v0.3 - Images
- [ ] PDF to PNG
- [ ] PDF to JPG
- [ ] PNG/JPG to PDF
- [ ] Validate and review PDF rasterization engine licensing

## v0.4 - Document enhancements
- [ ] Compression (report actual size savings; avoid unproven promises)
- [ ] Text/image watermarks
- [ ] Page numbering

## v0.5 - Microsoft Office formats
- [ ] Excel to PDF
- [ ] Word to PDF
- [ ] Establish an IT-approved offline engine; do not silently require Office or COM

## v0.6 - Document security
- [ ] Manage document passwords on files the operator is authorized to modify
- [ ] Audit encryption/permission semantics and secure handling

## v0.7 - Preview and batch tools
- [ ] Offline PDF thumbnail/page preview
- [ ] Jobs/progress/cancellation
- [ ] Better batch error handling and failure isolation

## v1.0 - Production candidate
- [ ] Automated regression suite, fuzz/malformed PDF tests, large file performance
- [ ] Vulnerability checks/SBOM/license audit
- [ ] Signed enterprise-approved distribution, if required
- [ ] Release process and rollback policy

Principles: offline document processing, no silent upload, no registry edits,
no auto-elevation, no source overwrite, no third-party binary without review.
