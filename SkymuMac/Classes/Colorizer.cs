using AppKit;

namespace Skymu
{
	public abstract class Colors
	{
		public NSColor LoginBackground = NSColor.FromRgb('\x00', '\xaf', '\xf0');
		public NSColor LoginPlaceholder = NSColor.FromRgb('\x9e', '\xd4', '\xf4');
		public NSColor SidebarSplitter = NSColor.FromRgb('\xe7', '\xf1', '\xf5');
	}
	
	class Light : Colors { }

	public static class Colorizer
	{
		public static Colors C = new Light();
	}
}

