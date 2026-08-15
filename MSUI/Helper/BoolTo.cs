using AppKit;

namespace MSUI.Helper
{
    public static class BoolTo
    {
        public static NSUserInterfaceLayoutOrientation ToUILayoutOrientation(this bool vertical)
            => vertical ? NSUserInterfaceLayoutOrientation.Vertical : NSUserInterfaceLayoutOrientation.Horizontal;
    }
}