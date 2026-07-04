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

using Skymu.Preferences;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;

using AppKit;
using Foundation;

namespace Skymu.Themes.S714
{
	public partial class MainWindow : NSWindow
	{
		public MainWindow(IntPtr handle) : base(handle)
			=> Construct();
		[Export("initWithCoder:")]
		public MainWindow(NSCoder coder) : base(coder)
		{
			RestoreState(coder);
			Construct();
        }

		NSSplitView balls;
		public MainViewModel vm;

		void Construct()
		{
			this.Title = Settings.BrandingName;
			vm = new MainViewModel();
		}

        public override void AwakeFromNib()
        {
			base.AwakeFromNib();

			// mess...
			try
			{
				var item = Toolbar.Items.FirstOrDefault(i => i.Identifier == SelfInfoRootView.Identifier);
                var container = item.View;
				container.AddSubview(SelfInfoView);
				container.AddConstraints(new NSLayoutConstraint[] {
					NSLayoutConstraint.Create(SelfInfoView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, container, NSLayoutAttribute.Top, 1, 0),
					NSLayoutConstraint.Create(SelfInfoView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, container, NSLayoutAttribute.Leading, 1, 0),
					NSLayoutConstraint.Create(SelfInfoView, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, container, NSLayoutAttribute.Trailing, 1, 0)
				});

				Sidebar.WantsLayer = true;
				Sidebar.Layer.BackgroundColor = NSColor.Red.CGColor;
				MainView.WantsLayer = true;
				MainView.Layer.BackgroundColor = NSColor.Green.CGColor;
            }
            catch (Exception ex)
			{
                Debug.WriteLine(ex);
                Universal.ShowMessage("An error occured initializing the self profile detials. The app will quit.");
				NSRunningApplication.CurrentApplication.Terminate();
			}
        }
    }
}
