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
	[Register ("LoginWindow")]
	partial class LoginWindow
	{
		[Outlet]
		AppKit.NSView LoadingView { get; set; }

		[Outlet]
		AppKit.NSButton LoginButton { get; set; }

		[Outlet]
		AppKit.NSView LoginView { get; set; }

		[Outlet]
		AppKit.NSTextField PasswordBox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton ProtocolSelector { get; set; }

		[Outlet]
		AppKit.NSTextField ProtocolText { get; set; }

		[Outlet]
		AppKit.NSImageView Throbber { get; set; }

		[Outlet]
		AppKit.NSTextField UsernameBox { get; set; }

		[Action ("OnLogin:")]
		partial void OnLogin (Foundation.NSObject sender);

		[Action ("OnPasswordEnter:")]
		partial void OnPasswordEnter (Foundation.NSObject sender);

		[Action ("OnProtocolChanged:")]
		partial void OnProtocolChanged (Foundation.NSObject sender);

		[Action ("OnUsernameEnter:")]
		partial void OnUsernameEnter (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (LoginView != null) {
				LoginView.Dispose ();
				LoginView = null;
			}

			if (PasswordBox != null) {
				PasswordBox.Dispose ();
				PasswordBox = null;
			}

			if (ProtocolSelector != null) {
				ProtocolSelector.Dispose ();
				ProtocolSelector = null;
			}

			if (ProtocolText != null) {
				ProtocolText.Dispose ();
				ProtocolText = null;
			}

			if (Throbber != null) {
				Throbber.Dispose ();
				Throbber = null;
			}

			if (UsernameBox != null) {
				UsernameBox.Dispose ();
				UsernameBox = null;
			}
		}
	}
}
