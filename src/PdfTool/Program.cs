using System;
using System.IO;
using System.Windows.Forms;

namespace PdfTool;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // Offline startup test: instantiate the form on Windows without showing it.
        // This is for diagnostics/CI and does not read or upload user PDFs.
        bool diagnostic = args.Length == 2 &&
            string.Equals(args[0], "--self-test", StringComparison.OrdinalIgnoreCase);

        if (diagnostic)
        {
            try
            {
                ApplicationConfiguration.Initialize();
                using var form = new MainForm();
                _ = form.Handle;
                File.WriteAllText(args[1], "PASS: Windows Forms startup completed successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                // Only an explicitly requested diagnostic writes a local result.
                try { File.WriteAllText(args[1], "FAIL: " + ex); }
                catch { /* Preserve original failure and exit code. */ }
                return 1;
            }
        }

        try
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
            return 0;
        }
        catch (Exception ex)
        {
            // The GUI executable has no console. Show a visible failure rather
            // than silently terminating when initialization throws.
            try
            {
                MessageBox.Show(
                    "PDF Tool could not open.\n\n" + ex.GetType().Name + ": " + ex.Message +
                    "\n\nPlease share only the error message, not a company PDF.",
                    "PDF Tool - Startup error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch { /* Windows may forbid UI; avoid retrying or bypassing policy. */ }
            return 1;
        }
    }
}
