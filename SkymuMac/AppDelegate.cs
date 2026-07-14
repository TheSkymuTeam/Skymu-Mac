/*==========================================================*/
// Copyright © The Skymu Team and other contributors.
// For any inquiries or concerns, email contact@skymu.app.
/*==========================================================*/
// Modification or redistribution of this code is governed
// by the terms set out in the project license agreement.
// If you do not comply with those terms, you may not
// modify or distribute any original code from the project.
/*==========================================================*/
// License: https://skymu.app/legal/license
// SPDX-License-Identifier: AGPL-3.0-or-later
/*==========================================================*/

using Foundation;
using AppKit;

namespace Skymu
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public NSWindowController activeWindow;

		public AppDelegate() { }

		public override void DidFinishLaunching(NSNotification notification)
		{
			Universal.OnStartup();
            activeWindow = new Themes.S714.LoginWindowController();
            activeWindow.Window.MakeKeyAndOrderFront(this);
		}

		public override bool ApplicationShouldHandleReopen(NSApplication sender, bool hasVisibleWindows)
        {
			activeWindow.Window.IsVisible = true;
			return true;
        }
    }
}

