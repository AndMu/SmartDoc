using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Wikiled.SmartDoc.Logic.Helpers
{
    public static class ImagerHelper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        public static Bitmap CreateEmpty()
        {
            Bitmap bitmap = new Bitmap(36, 50);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Rectangle rect = new Rectangle(0, 0, 36, 50);
                graphics.FillRectangle(new SolidBrush(Color.White), rect);
                return bitmap;
            }
        }
    }
}
