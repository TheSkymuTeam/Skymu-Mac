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

#if NET5_0_OR_GREATER
using nfloat = System.Runtime.InteropServices.NFloat;
using nint = System.IntPtr;
#endif

using AppKit;
using CoreGraphics;
using Skymu.Preferences;
using Skymu.Quick;
using Skymu.UserControls;
using Skymu.ViewModels;
using System;
using Foundation;
using Yggdrasil.Enumerations;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
	public sealed class MainWindowController : NSWindowController
	{
		public MainWindowController()
		{
			Window = new MainWindow();
		}
	}

    public sealed class MainWindow : NSWindow
    {
        public MainViewModel vm { get; private set; }

        public MainWindow() : base(
	        new CGRect(0, 0, 720, 475),
	        NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable |
				NSWindowStyle.Resizable | NSWindowStyle.UnifiedTitleAndToolbar,
	        NSBackingStore.Buffered,
	        false
        )
	        => Construct();

	    async void Construct()
	    {
		    FrameAutosaveName = "M5Window";
		    Identifier = "M5Window";
		    Restorable = true;
	        vm = new MainViewModel();
	        await vm.InitSidebar();
	        if (Universal.CurrentUser == null)
		        // we already have an alert
		        return;
	        Title = Settings.BrandingName + "™ - " + Universal.CurrentUser.DisplayName;
	        
	        Universal.GroupAvatar = GenerateAvatarImage("group");
	        Universal.ContactAvatar = GenerateAvatarImage("contact");

	        var mvc = new MainViewController
	        {
		        vm = vm
	        };
	        ContentView = mvc.View;
	        Toolbar = mvc.SetupToolbar();
	        MakeKeyAndOrderFront(this);
        }

	    NSImage GenerateAvatarImage(string what)
	    {
		    return new NSImage(NSBundle.MainBundle.PathForResource(
			    "256-default-" + what,
			    "png",
			    Universal.Theme));
	    }
    }

    public sealed class MainViewController : NSViewController
    {
	    public MainViewModel vm { get; internal set; }
		SidebarSplitViewController ssvc;

        public override void LoadView()
        {
            View = new NSView();
	        Setup();
        }

        internal NSToolbar SetupToolbar()
        {
	        var bar = new NSToolbar();
            return bar;
        }

	    void Setup()
	    {
			ssvc = new SidebarSplitViewController(this);

            View = ssvc.View;
	    }

		public void SelectTab(bool home)
		{
			if (home)
			{
                if (ssvc.mainView.Subviews.Length != 0)
                    ssvc.mainView.WillRemoveSubview(ssvc.mainView.Subviews[0]);
            }
			else
			{

                if (ssvc.mainView.Subviews.Length == 0 || !ReferenceEquals(ssvc.conversationViewController.View, ssvc.mainView.Subviews[0]))
                {
                    if (ssvc.mainView.Subviews.Length != 0)
                        ssvc.mainView.WillRemoveSubview(ssvc.mainView.Subviews[0]);
                    ssvc.mainView.AddSubview(ssvc.conversationViewController.View);
                    QCon.CAll(ssvc.conversationViewController.View, ssvc.mainView);
                }
            }
		}

	    class SidebarSplitViewController : NSViewController
	    {
		    NSView split;
			readonly MainViewController mvc;
		    readonly MainViewModel vm;

		    internal ConversationViewController conversationViewController;
            internal NSView mainView;
            NSView sidebarView;

		    public SidebarSplitViewController(MainViewController mvc)
			{
				this.mvc = mvc;
				vm = mvc.vm;
			}
		    
		    public override void LoadView()
		    {
			    split = new SeanSplitter
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    IsVertical = true,
				    AutoresizesSubviews = true,
				    AutosaveName = "Sidebar", // TODO: Save per user and shii like original
				    Delegate = new SplitViewDelegate()
			    };
			    View = split;

			    sidebarView = CreateSidebarView();
			    mainView = new NSView
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    AutoresizesSubviews = true,
				    WantsLayer = true,
				    Layer =
				    {
					    BackgroundColor = NSColor.ControlBackground.CGColor
				    }
			    };
			    View.AddSubview(sidebarView);
			    View.AddSubview(mainView);

			    QCon.CVertical(mainView, View);
			    QCon.CVertical(sidebarView, View);
			    QCon.Con(sidebarView, View, NSLayoutAttribute.Left);
			    QCon.Con(View, mainView, sidebarView, NSLayoutAttribute.Left, NSLayoutAttribute.Right);
			    QCon.Con(mainView, View, NSLayoutAttribute.Right);
			    
			    if (125 > sidebarView.Frame.Width || sidebarView.Frame.Width > 300)
				    sidebarView.Frame = new CGRect(sidebarView.Frame.X, sidebarView.Frame.Y, 200, sidebarView.Frame.Height);

			    conversationViewController = new ConversationViewController(vm);
			    conversationViewController.LoadView();
			    
			    vm.ConversationOpened += (s, e) =>
			    {
					mvc.SelectTab(false);
				    conversationViewController.SetConversation();
			    };
		    }

		    NSView CreateSidebarView()
		    {
			    var sidebar = new NSView
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    WantsLayer = true,
				    Layer =
				    {
					    BackgroundColor = Colorizer.C.SidebarBackground.CGColor
				    }
			    };

			    var convlist = new ContactListViewController(mvc, ListType.Conversations)
			    {
					View = {
					    TranslatesAutoresizingMaskIntoConstraints = false
				    }
			    };
			    sidebar.AddSubview(convlist.View);
				QCon.Size(convlist.View, 100, 100, NSLayoutRelation.GreaterThanOrEqual);
			    QCon.CHorizontal(convlist.View, sidebar);
			    QCon.Con(convlist.View, sidebar, NSLayoutAttribute.Top, 14);
			    QCon.Con(convlist.View, sidebar, NSLayoutAttribute.Bottom);
			    
			    return sidebar;
		    }
	    }

	    class SplitViewDelegate : NSSplitViewDelegate
	    {
		    public override bool ShouldAdjustSize(NSSplitView splitView, NSView view)
			    => !view.Equals(splitView.Subviews[0]);

		    public override nfloat SetMinCoordinateOfSubview(NSSplitView splitView, nfloat proposedMinimumPosition, nint subviewDividerIndex)
			    => 125;
			
		    public override nfloat SetMaxCoordinateOfSubview(NSSplitView splitView, nfloat proposedMaximumPosition, nint subviewDividerIndex)
			    => 300;
	    }
    }
}
