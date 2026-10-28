using PdfSharp.Pdf;
using PdfTool;

string tempDir = Path.Combine(Path.GetTempPath(), "pdf-tool-test-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(tempDir);

try
{
    string first = Path.Combine(tempDir, "first.pdf");
    string second = Path.Combine(tempDir, "second.pdf");

    using (var a = new PdfDocument())
    {
        a.AddPage();
        a.Save(first);
    }
    using (var b = new PdfDocument())
    {
        b.AddPage();
        b.AddPage();
        b.Save(second);
    }

    string merged = PdfOperations.Merge(new[] { first, second });
    using (var check = PdfSharp.Pdf.IO.PdfReader.Open(merged, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import))
    {
        if (check.PageCount != 3)
            throw new Exception("Merge: expected 3 pages");
    }

    string mergedAgain = PdfOperations.Merge(new[] { first, second });
    if (mergedAgain == merged || !File.Exists(mergedAgain))
        throw new Exception("Merge must never overwrite a previous output");

    if (PdfOperations.Split(first) != 1)
        throw new Exception("Split: expected 1 page");

    if (!File.Exists(Path.Combine(tempDir, "first_page_0001.pdf")))
        throw new Exception("Split: output missing");

    if (PdfOperations.Split(first) != 1 ||
        !File.Exists(Path.Combine(tempDir, "first_page_0001 (1).pdf")))
        throw new Exception("Split must not overwrite previous output");

    if (!File.Exists(first) || !File.Exists(second))
        throw new Exception("Source files must be preserved");

    Console.WriteLine("PASS: Merge, Split, unique filenames, source preservation");
}
finally
{
    try { Directory.Delete(tempDir, recursive: true); }
    catch (IOException) { Console.Error.WriteLine("Warning: temporary test directory could not be removed."); }
}
