using SkiaSharp;
using System.IO;
using System.Windows.Media.Imaging;

namespace BakingApplication.Utilities;

public static class BitmapToByteArrayConverter
{
    public static byte[] Convert(BitmapImage image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(ms);
            return ms.ToArray();
        }
    }
}
