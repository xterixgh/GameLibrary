using Avalonia.Media.Imaging;
using GameLibrary.Models;

namespace GameLibrary.Helpers
{
    public class GameCardItem
    {
        public Games Game { get; set; }
        public Bitmap? Cover { get; set; }
        public string Letter { get; set; }
        public bool HasCover { get; set; }
        public bool ShowPlaceholder { get; set; }
        public string StatusName { get; set; }
        public string Meta { get; set; }
    }
}
