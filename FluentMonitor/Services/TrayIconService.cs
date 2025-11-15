using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Media.Imaging;

namespace FluentMonitor.Services
{
    public class TrayIconService
    {
        private const int IconSize = 16;

        public BitmapImage GenerateCpuIcon(float cpuUsage)
        {
            using var bitmap = new Bitmap(IconSize, IconSize);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            // Background
            graphics.Clear(Color.Transparent);

            // Determine color based on usage
            Color iconColor;
            if (cpuUsage < 50)
                iconColor = Color.FromArgb(0, 120, 215); // Blue
            else if (cpuUsage < 80)
                iconColor = Color.FromArgb(255, 140, 0); // Orange
            else
                iconColor = Color.FromArgb(232, 17, 35); // Red

            // Draw CPU text or percentage
            var text = cpuUsage >= 100 ? "!!" : ((int)cpuUsage).ToString();
            var fontSize = text.Length == 1 ? 11 : (text == "!!" ? 10 : 8);

            using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(iconColor);

            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(text, font, brush, new RectangleF(0, 0, IconSize, IconSize), stringFormat);

            return BitmapToImageSource(bitmap);
        }

        public string GenerateTooltipText(float cpuUsage, float memoryUsage, float diskSpeed, float networkSpeed)
        {
            return $"Fluent Monitor\n" +
                   $"CPU: {cpuUsage:F1}%\n" +
                   $"内存: {memoryUsage:F1}%\n" +
                   $"磁盘: {FormatSpeed(diskSpeed)} MB/s\n" +
                   $"网络: {FormatSpeed(networkSpeed)} KB/s";
        }

        private string FormatSpeed(float speed)
        {
            if (speed < 0.01f)
                return "0.0";
            else if (speed < 1)
                return $"{speed:F2}";
            else if (speed < 10)
                return $"{speed:F1}";
            else
                return $"{speed:F0}";
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}
