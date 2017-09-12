using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace Wikiled.SmartDoc.Helpers
{
    internal class DocumentImageSelector : TreeListNodeImageSelector, IValueConverter
    {
        private static readonly BitmapImage Image = new BitmapImage(
            new Uri("/Wikiled.SmartDoc;component/Resources/folder.png", UriKind.Relative));

        public override ImageSource Select(TreeListRowData rowData)
        {
            return Image;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Image;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
