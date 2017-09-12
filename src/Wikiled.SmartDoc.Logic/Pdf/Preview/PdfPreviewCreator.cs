using System;
using System.Drawing;
using System.IO;
using DevExpress.Pdf;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.SmartDoc.Logic.Helpers;

namespace Wikiled.SmartDoc.Logic.Pdf.Preview
{
    public class PdfPreviewCreator : IPreviewCreator
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int maxWidth = 595;

        private int maxHeight = 842;

        public Bitmap CreatePreview(FileInfo file)
        {
            Guard.NotNull(() => file, file);
            try
            {
                using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                {
                    documentProcessor.LoadDocument(file.FullName);
                    if (documentProcessor.Document.Pages.Count == 0)
                    {
                        return ImagerHelper.CreateEmpty();
                    }


                    if (documentProcessor.Document.Pages[0].CropBox.Height > maxHeight ||
                       documentProcessor.Document.Pages[0].CropBox.Width > maxWidth)
                    {
                        var width = documentProcessor.Document.Pages[0].CropBox.Width > maxWidth ? maxWidth : documentProcessor.Document.Pages[0].CropBox.Width;
                        var height = documentProcessor.Document.Pages[0].CropBox.Height > maxHeight ? maxHeight : documentProcessor.Document.Pages[0].CropBox.Height;
                        documentProcessor.Document.Pages[0].CropBox = new PdfRectangle(0, 0, width, height);
                    }

                    var bitmap = documentProcessor.CreateBitmap(1, 250);
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        int borderSize = 10;
                        using (Pen pen = new Pen(Color.Black, borderSize))
                        {
                            graphics.DrawRectangle(
                                pen,
                                0,
                                0,
                                bitmap.Width - borderSize,
                                bitmap.Height - borderSize);
                        }

                        return bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                return ImagerHelper.CreateEmpty();
            }
        }
    }
}
