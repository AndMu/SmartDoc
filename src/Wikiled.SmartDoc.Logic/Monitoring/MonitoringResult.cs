using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Helpers;

namespace Wikiled.SmartDoc.Logic.Monitoring
{
    public class MonitoringResult : IDisposable
    {
        private readonly Lazy<BitmapSource> bitmapSource;

        private readonly Bitmap bitmap;

        public MonitoringResult(FileInfo file, string @class, Bitmap preview)
        {
            Guard.NotNull(() => file, file);
            Guard.NotNull(() => preview, preview);
            File = file;
            Class = @class;
            bitmap = preview;
            bitmapSource = new Lazy<BitmapSource>(() => ImagerHelper.Bitmap2BitmapImage(bitmap));
        }

        public event EventHandler MoveRequest;

        public BitmapSource Preview => bitmapSource.Value;

        public string Class { get; private set; }

        public FileInfo DestinationFile { get; private set; }

        public FileInfo File { get; }

        public void ApplyChange(DirectoryInfo destination)
        {
            Guard.NotNull(() => destination, destination);
            DestinationFile = new FileInfo(Path.Combine(destination.FullName, Class, File.Name));
            MoveRequest?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }
    }
}
