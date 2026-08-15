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
using Foundation;
using Skymu.Classes;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using MSUI.Helper.Quick;
using Yggdrasil.Enumerations;
using Yggdrasil.Models;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
	class ContactListViewController : NSViewController
	{
		readonly ContactTableDataSource ds;
		readonly ListType type;
		readonly MainViewController mvc;
        readonly MainViewModel vm;

        public ContactListViewController(MainViewController mvc, ListType type)
	    {
			vm = mvc.vm;
		    ds = new ContactTableDataSource(vm, type);
		    this.type = type;
		    this.mvc = mvc;
	    }

	    public override void LoadView()
	    {
		    View = new NSView
		    {
			    TranslatesAutoresizingMaskIntoConstraints = false,
			    WantsLayer = true,
			    Layer =
			    {
				    BackgroundColor = Colorizer.C.SidebarBackground.CGColor,
				    BorderColor = NSColor.Green.CGColor,
				    BorderWidth = 2
			    }
		    };
		    
		    var cvd = new ContactTableDelegate();
		    var view = new NSOutlineView
		    {
			    TranslatesAutoresizingMaskIntoConstraints = false,
				AutoresizesSubviews = true,
				BackgroundColor = NSColor.FromRgba(0, 0, 0, 0),
				WantsLayer = true,
				Layer =
				{
					BorderColor = NSColor.Blue.CGColor,
					BorderWidth = 5
				},
				DataSource = ds,
				Delegate = cvd,
				GridStyleMask = NSTableViewGridStyle.None
		    };
		    view.HeaderView = null;
	
		    var scrollView = new NSScrollView
		    {
			    TranslatesAutoresizingMaskIntoConstraints = false,
			    DocumentView = view,
			    HasVerticalScroller = true,
			    WantsLayer = true,
			    Layer =
			    {
				    BorderColor = NSColor.Red.CGColor,
				    BorderWidth = 5
			    }
		    };

		    View.AddSubview(scrollView);
		    QHug.HAll(view, 500);
		    QComp.mpAll(view, 1);
		    QCon.Size(scrollView, 5, 5, NSLayoutRelation.GreaterThanOrEqual); // This is absolutely necessary for the view to work
		    QCon.CHorizontal(scrollView, View);
		    QCon.Con(scrollView, View, NSLayoutAttribute.Top, 14);
		    QCon.Con(scrollView, View, NSLayoutAttribute.Bottom);
		    
		    var col = new NSTableColumn("name");
		    view.AddColumn(col);
		    
		    view.DataSource = ds;
		    view.Delegate = cvd;
		    cvd.ItemSelected += () =>
		    {
			    var i = (int)view.SelectedRow;
			    if (i == -1)
			    {
					vm.ClearActiveConversation();
					mvc.SelectTab(true);
				    return;
			    }
			    Conversation con = null;
			    switch (type)
			    {
				    case ListType.Contacts:
					    con = vm.ContactList.Count >= i + 1 ? vm.ContactList[i] : null;
					    if (con == null)
					    {
						    Universal.ExceptionHandler(
							    new IndexOutOfRangeException(
								    "The contact list that you see was bigger than the internal list."),
							    Universal.EX_IS_OKAY);
						    return;
					    }
					    break;
				    case ListType.Conversations:
					    con = vm.ConversationList.Count >= i + 1 ? vm.ConversationList[i] : null;
					    if (con == null)
					    {
						    Universal.ExceptionHandler(
							    new IndexOutOfRangeException(
								    "The conversation list that you see was bigger than the internal list."),
								    Universal.EX_IS_OKAY);
						    return;
					    }
					    break;
				    case ListType.Servers: // todo seg >w<
					    break;
				    default: return; // fuck
			    }
			    vm.SelectedConversation = con;
			    _ = vm.SetConversation();
		    };
	    }
    }

	class ContactTableDataSource : NSOutlineViewDataSource
    {
	    readonly ListType type;
	    internal readonly MainViewModel vm;

	    public ContactTableDataSource(MainViewModel vm, ListType type)
	    {
		    this.vm = vm;
		    this.type = type;
	    }

	    public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
		    => ((IDWrap)item).Metadata is Server;

	    public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
		    => new IDWrap(type == ListType.Contacts
			    ? vm.ContactList[(int)childIndex]
			    : type == ListType.Conversations
			    ? vm.ConversationList[(int)childIndex]
			    : type == ListType.Servers
			    ? (Metadata)vm.ServerList[(int)childIndex]
			    : throw new NotImplementedException("yeah no, today is not the day to do this.")); // this self harm comment should not appear because GCC is called beforehand
																									   // actually im not even doing this. this *might* appear on user end. probably just my insane paranoia, which adds to another reason why i should...
																							           // lmao the second dot hit the screen border of my retina (yes im spoiled :fire:)
																							           // btw, if you're going to say "this is going to harm you and not help you", yes that's exactly what i want
																							           // fuck-ass typo checker btw
	    public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
		    => (nint)(type == ListType.Contacts 
			    ? vm.ContactList.Count
			    : type == ListType.Conversations
				? vm.ConversationList.Count
				: type == ListType.Servers
				? vm.ServerList.Count
				: 0);

	    public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			=> new NSString(((IDWrap)item).Metadata.DisplayName);
    }

	class ContactTableDelegate : NSOutlineViewDelegate
	{
		public event Action ItemSelected;
		NSOutlineView outlineView;
		
	    public override NSView GetView(NSOutlineView view, NSTableColumn tableColumn, NSObject item)
	    {
		    outlineView = view;
		    var wrap = (IDWrap)item;
		    if (!(view.DataSource is ContactTableDataSource ds))
			    throw new Exception("Developer created assigned a ContactTableDelegate to a NSOutlineView without a ContactTableDataSource as the DataSource.");
		    var dispName =
				(
				    ds.vm.ContactList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    ?? ds.vm.ConversationList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    ?? (Metadata)ds.vm.ServerList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    // TODO server channels
			    )?.DisplayName;
		    if (dispName == null)
			    return null;

		    var cell = view.MakeView("cell", owner: this) as SeanContactCell 
										?? new SeanContactCell();

		    cell.TextField.StringValue = dispName;
		    cell.Level = outlineView.LevelForItem(item);
		    cell.UpdateAppearance(false);

		    return cell;
	    }

	    public override void SelectionDidChange(NSNotification notification)
	    {
		    ItemSelected?.Invoke();
		    if (outlineView == null)
		    {
			    Universal.ExceptionHandler( new Exception(
				    "Somehow, a different contact list item was selected before a single view was loaded. This should NOT happen."),
				    Universal.EX_IS_OKAY
				);
			    return;
		    }

		    var selectedIndex = outlineView.SelectedRow;
		    if (selectedIndex >= 0)
		    {
			    Debug.WriteLine(outlineView.Subviews[selectedIndex].GetType());
			    // todo penis?
		    }
	    }
	}
	
	public class SeanContactCell : NSTableCellView
	{
		private nint level;
		private NSBox backgroundBox;

		public nint Level
		{
			get => level;
			set
			{
				level = value;
				LayoutSubtreeIfNeeded();
			}
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			Construct();
		}

		public SeanContactCell()
			=> Construct();

		private void Construct()
		{
			// Remove default text field
			TextField?.RemoveFromSuperview();

			backgroundBox = new NSBox
			{
				BoxType = NSBoxType.NSBoxCustom,
				FillColor = NSColor.FromRgba(0, 0, 0, 0)
			};
			AddSubview(backgroundBox);

			TextField = new NSTextField
			{
				Bordered = false,
				Editable = false,
				DrawsBackground = false,
				Font = NSFont.SystemFontOfSize(13)
			};
			AddSubview(TextField);
		}

		public void UpdateAppearance(bool selected)
		{
			if (selected)
			{
				backgroundBox.FillColor = Colorizer.C.SidebarSelected;
			}
			else
			{
				backgroundBox.FillColor = NSColor.FromRgba(0, 0, 0, 0);
			}
		}

		public override void Layout()
		{
			base.Layout();

			var bounds = Bounds;
			var indent = Level * 16f; // Standard indent per level

			backgroundBox.Frame = bounds;
			TextField.Frame = new CGRect(indent + 18, 2, bounds.Width - indent - 20, bounds.Height - 4);
		}
	}
}