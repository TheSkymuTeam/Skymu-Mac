using AppKit;

namespace MSUI.QuickView
{
    public class Image : NSImageView
    {
        public void _(NSImage image)
            => Image = image;

        public Image(NSImage image)
            => _(image);
    }
}