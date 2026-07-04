// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Skymu.Themes.S714
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSView MainView { get; set; }

		[Outlet]
		AppKit.NSToolbarItem SelfInfoRootView { get; set; }

		[Outlet]
		AppKit.NSStackView SelfInfoView { get; set; }

		[Outlet]
		AppKit.NSView Sidebar { get; set; }

		[Outlet]
		AppKit.NSSplitView SidebarSplitter { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SelfInfoRootView != null) {
				SelfInfoRootView.Dispose ();
				SelfInfoRootView = null;
			}

			if (SelfInfoView != null) {
				SelfInfoView.Dispose ();
				SelfInfoView = null;
			}

			if (SidebarSplitter != null) {
				SidebarSplitter.Dispose ();
				SidebarSplitter = null;
			}

			if (MainView != null) {
				MainView.Dispose ();
				MainView = null;
			}

			if (Sidebar != null) {
				Sidebar.Dispose ();
				Sidebar = null;
			}
		}
	}
}
