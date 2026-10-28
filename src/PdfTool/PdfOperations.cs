using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfTool;

/// <summary>All PDF work is local; original files are never modified.</summary>
internal static class PdfOperations
{
    public static string Merge(IReadOnlyList<string> paths)
    {
        if (paths.Count < 2)
            throw new ArgumentException("Please select at least two PDF files.");

        using var merged = new PdfDocument();

        foreach (string path in paths)
        {
            using var source = PdfReader.Open(path, PdfDocumentOpenMode.Import);
            for (int i = 0; i < source.PageCount; i++)
                merged.AddPage(source.Pages[i]);
        }

        string directory = Path.GetDirectoryName(Path.GetFullPath(paths[0]))!;
        return SaveWithoutOverwrite(merged, directory, "Merged");
    }

    public static int Split(string path)
    {
        using var source = PdfReader.Open(path, PdfDocumentOpenMode.Import);
        string directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
        string baseName = Path.GetFileNameWithoutExtension(path);

        for (int i = 0; i < source.PageCount; i++)
        {
            using var onePage = new PdfDocument();
            onePage.AddPage(source.Pages[i]);
            SaveWithoutOverwrite(onePage, directory, baseName + "_page_" + (i + 1).ToString("0000"));
        }

        return source.PageCount;
    }

    private static string SaveWithoutOverwrite(PdfDocument document, string directory, string baseName)
    {
        // Generate a unique temporary file alongside the output, then move it
        // without replacing an existing destination.
        string temporary = Path.Combine(directory, ".pdf-tool-" + Guid.NewGuid().ToString("N") + ".pdf");

        try
        {
            document.Save(temporary);

            for (int attempt = 0; attempt < 10000; attempt++)
            {
                string suffix = attempt == 0 ? "" : " (" + attempt + ")";
                string destination = Path.Combine(directory, baseName + suffix + ".pdf");

                try
                {
                    // The two-argument overload throws if destination exists.
                    File.Move(temporary, destination);
                    return destination;
                }
                catch (IOException) when (File.Exists(destination))
                {
                    // Another program already uses this name; try the next.
                }
            }

            throw new IOException("Unable to find an unused output filename.");
        }
        finally
        {
            if (File.Exists(temporary))
                File.Delete(temporary);
        }
    }
}
