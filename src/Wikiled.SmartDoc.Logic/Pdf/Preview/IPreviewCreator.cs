using System.Drawing;
using System.IO;

namespace Wikiled.SmartDoc.Logic.Pdf.Preview
{
    public interface IPreviewCreator
    {
        Bitmap CreatePreview(FileInfo file);
    }
}