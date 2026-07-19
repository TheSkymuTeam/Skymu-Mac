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
using Foundation;
using Skymu.UserControls;
using Skymu.ViewModels;
using System;
using System.Linq;
using Skymu.Classes;
using Yggdrasil.Enumerations;
using Yggdrasil.Models;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
	class ContactListViewController : NSViewController
	{
		readonly ContactTableDataSource ds;
		private readonly ListType type;
		readonly MainViewController mvc;
        NSOutlineView view;
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
		    
		    view.DataSource = ds;
		    var cvd = new ContactTableDelegate();
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
				    default: throw new NotImplementedException("no im not doing that :3");
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
				: throw new NotImplementedException("so uh, you (as in the programmer, not the user) just tried to make a view of new list before modifying this code? sorry but no im not having that today"));

	    public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			=> new NSString(((IDWrap)item).Metadata.DisplayName);
    }

	class ContactTableDelegate : NSOutlineViewDelegate
	{
		public event Action ItemSelected;
	    
	    public override NSView GetView(NSOutlineView tableView, NSTableColumn tableColumn, NSObject item)
	    {
		    var wrap = (IDWrap)item;
		    if (!(tableView.DataSource is ContactTableDataSource ds))
			    throw new Exception("ya forgot to set the delegate!");
		    var dispName =
				(
				    ds.vm.ContactList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    ?? ds.vm.ConversationList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    ?? (Metadata)ds.vm.ServerList.FirstOrDefault(e => e.Identifier == wrap.Identifier)
				    // TODO server channels
			    )?.DisplayName;
		    if (dispName == null)
			    return null;
		    
		    var view = tableView.MakeView("cell", owner: this) as NSTextField ??
		               new Label(dispName) { Identifier = "cell" };
		    return view;
	    }

	    public override void SelectionDidChange(NSNotification notification)
	    {
		    ItemSelected?.Invoke();
	    }
	}
}