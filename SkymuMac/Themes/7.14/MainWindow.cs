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

using AppKit;
using CoreGraphics;
using Foundation;
using Skymu.Preferences;
using Skymu.Quick;
using Skymu.UserControls;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Skymu.Classes;
using Yggdrasil.Models;

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
			/*
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
			*/

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
	        var mvc = new MainViewController
	        {
		        vm = vm
	        };
	        ContentView = mvc.View;
	        Toolbar = mvc.SetupToolbar();
	        MakeKeyAndOrderFront(this);
        }
    }

    public sealed class MainViewController : NSViewController
    {
	    public MainViewModel vm { get; internal set; }
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
		    View = new SidebarSplitViewController(vm).View;
	    }
	    
	    #region Main split

	    class SidebarSplitViewController : NSViewController
	    {
		    NSView sidebarView;
		    NSView mainView;
		    private MainViewModel vm;

		    public SidebarSplitViewController(MainViewModel vm)
			    => this.vm = vm;
		    
		    public override void LoadView()
		    {
			    var split = new SeanSplitter
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    IsVertical = true,
				    AutoresizesSubviews = true,
				    AutosaveName = "Sidebar", // TODO: Save per user and shii like original
				    Delegate = new SplitViewDelegate()
			    };
			    View = split;

			    sidebarView = CreateSidebarView();
			    mainView = CreateMainView();
			    View.AddSubview(sidebarView);
			    View.AddSubview(mainView);

			    QCon.CVertical(mainView, View);
			    QCon.Con(mainView, View, NSLayoutAttribute.Right);
			    QCon.CVertical(sidebarView, View);
			    QCon.Con(sidebarView, View, NSLayoutAttribute.Left);
			    QCon.Con(View, sidebarView, mainView, NSLayoutAttribute.Right, NSLayoutAttribute.Left);
			    
			    if (125 > sidebarView.Frame.Width || sidebarView.Frame.Width > 300)
				    sidebarView.Frame = new CGRect(sidebarView.Frame.X, sidebarView.Frame.Y, 200, sidebarView.Frame.Height);
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

			    var conlist = new ContactListViewController(vm)
			    {
					View = {
					    TranslatesAutoresizingMaskIntoConstraints = false,
					    WantsLayer = true
				    }
			    };
			    sidebar.AddSubview(conlist.View);
			    QCon.Size(conlist.View, 100, 100, NSLayoutRelation.GreaterThanOrEqual);
			    QCon.CHorizontal(conlist.View, sidebar);
			    QCon.Con(conlist.View, sidebar, NSLayoutAttribute.Top, 14);
			    QCon.Con(conlist.View, sidebar, NSLayoutAttribute.Bottom);
			    
			    return sidebar;
		    }

		    NSView CreateMainView()
		    {
			    var main = new NSView
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    WantsLayer = true,
				    Layer =
				    {
					    BackgroundColor = NSColor.ControlBackground.CGColor
				    }
			    };

			    var label = new Label("Main content")
			    {
				    TranslatesAutoresizingMaskIntoConstraints = false,
				    Font = NSFont.SystemFontOfSize(18),
				    Alignment = NSTextAlignment.Center,
				    WantsLayer = true
			    };
			    main.AddSubview(label);
			    QCon.Width(label, 250, NSLayoutRelation.GreaterThanOrEqual);
			    QCon.Center(label, main);

			    return main;
		    }
	    }
	    
	    #endregion
	    
	    #region Views/delegates

	    class SplitViewDelegate : NSSplitViewDelegate
	    {
			public override bool ShouldAdjustSize(NSSplitView splitView, NSView view)
				=> !view.Equals(splitView.Subviews[0]);

			public override nfloat SetMinCoordinateOfSubview(NSSplitView splitView, nfloat proposedMinimumPosition, nint subviewDividerIndex)
				=> 125;
			
			public override nfloat SetMaxCoordinateOfSubview(NSSplitView splitView, nfloat proposedMaximumPosition, nint subviewDividerIndex)
				=> 300;
	    }
	    
	    public class ContactListViewController : NSViewController
	    {
		    NSOutlineView view;
		    readonly MainViewModel vm;

		    public ContactListViewController(MainViewModel vm) : base()
			    => this.vm = vm;

		    public override void LoadView()
		    {
			    view = new NSOutlineView
			    {
					AutoresizesSubviews = true
			    };
		
			    var scrollView = new NSScrollView
			    {
				    DocumentView = view,
				    HasVerticalScroller = true,
			    };

			    View = scrollView;
			    
			    var col = new NSTableColumn("name");
			    view.AddColumn(col);
			    
			    view.DataSource = new ContactTableDataSource(vm);
			    view.Delegate = new ContactTableDelegate(vm);
		    }
	    }

	    public class ContactTableDataSource : NSOutlineViewDataSource
	    {
		    private MainViewModel vm;

		    public ContactTableDataSource(MainViewModel vm)
		    {
			    this.vm = vm;
		    }

		    public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
		    {
			    return ((IDWrap)item).Metadata is Server;
		    }

		    public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
		    {
			    var md = vm.ContactList[(int) childIndex];
			    return new IDWrap(md.Identifier, md);
		    }

		    public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
		    {
			    return vm.ContactList?.Count ?? 0;
		    }

		    public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		    {
			    return new NSString(((IDWrap)item).Metadata.DisplayName);
		    }
	    }

	    public class ContactTableDelegate : NSOutlineViewDelegate
	    {
		    private MainViewModel vm;

		    public ContactTableDelegate(MainViewModel vm)
		    {
			    this.vm = vm;
		    }

		    public override NSView GetView(NSOutlineView tableView, NSTableColumn tableColumn, NSObject item)
		    {
			    var wrap = (IDWrap)item;
			    var dispName = vm.ContactList.FirstOrDefault(e => e.Identifier == wrap.Identifier)?.DisplayName;
			    if (dispName == null)
				    return null;
			    
			    var view = tableView.MakeView("cell", owner: this) as NSTextField ??
			               new Label(dispName) { Identifier = "cell" };
			    return view;
		    }
	    }
	    
	    #endregion
    }
}
