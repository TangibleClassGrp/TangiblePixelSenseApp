using System;

namespace Drag_and_Drop
{
    public class PhotoData
    {
        public string Source { get; private set; }
        public string Caption { get; private set; }
        public long Identifier { get; private set; }

        public PhotoData(string source, string caption, long identifier)
        {
            this.Source = source;
            this.Caption = caption;
            this.Identifier = identifier;
        }
    }
}
