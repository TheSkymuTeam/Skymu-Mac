using AppKit;

namespace Skymu
{
	public abstract class Colors
	{
		public readonly NSColor LoginBackground = NSColor.FromRgb('\x00', '\xaf', '\xf0');
		public readonly NSColor LoginFormLine = NSColor.FromRgb('\xb0', '\xDD', '\xF8');
		public readonly NSColor LoginPlaceholder = NSColor.FromRgb('\x9e', '\xd4', '\xf4');
		public readonly NSColor SidebarBackground = NSColor.FromRgb('\xf9', '\xfb', '\xfd');
		// TOOD rename me to SplitterDivider
		public readonly NSColor SidebarSplitter = NSColor.FromRgb('\xe7', '\xf1', '\xf5');
	}
	
	class Light : Colors { }

	public static class Colorizer
	{
		public static Colors C = new Light();
	}
}

