namespace Wikiled.SmartDoc.Logic.Pdf.Readers.DevExpress
{
    public class NullTextParser : ITextParser
    {
        private NullTextParser()
        {}

        public static NullTextParser Instance { get; } = new NullTextParser();

        public string Parse()
        {
            return string.Empty;
        }
    }
}
