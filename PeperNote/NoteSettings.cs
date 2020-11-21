using System;
using System.Windows.Media;

namespace PeperNote
{
    [Serializable]
    public class NoteSettings
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public bool AlwaysOnTop { get; set; }
        public Color NoteColor { get; set; }
    }
}
