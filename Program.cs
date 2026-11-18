
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

internal class Program
{

    [DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);
    private static int Main(string[] args)
    {
        try
        {
            // Require a destination directory and a filename prefix.
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
                return 1;

            string outputDirectory = Path.GetFullPath(args[0]);

            // Create directory if necessary.
            Directory.CreateDirectory(outputDirectory);

            // Get primary monitor dimensions.
            SetProcessDpiAwarenessContext(new IntPtr(-4));
            int width = GetSystemMetrics(SM_CXSCREEN);
            int height = GetSystemMetrics(SM_CYSCREEN);

            if (width <= 0 || height <= 0)
                return 2;

            // Generate a unique timestamped filename.
            string filename =
                $"{args[1]}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";

            string outputPath =
                Path.Combine(outputDirectory, filename);

            // Capture primary monitor.
            using Bitmap screenshot = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(
                    0,
                    0,
                    0,
                    0,
                    new Size(width, height),
                    CopyPixelOperation.SourceCopy
                );
            }

            // Save as PNG.
            screenshot.Save(outputPath, ImageFormat.Png);

            return 0;
        }
        catch
        {
            // Intentionally silent.
            return 1;
        }
    }

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}
