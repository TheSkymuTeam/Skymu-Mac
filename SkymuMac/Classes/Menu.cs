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
using System;

// ReSharper disable once CheckNamespace
namespace Skymu.Menu
{
    public class MenuBar
    {
        readonly NSMenu rootMenu;

        public MenuBar()
        {
            rootMenu = new NSMenu();
        }

        public void Create(string title, params MenuItem[] items)
        {
            var menu = new NSMenu(title);
	
            foreach (var item in items)
            {
                EventHandler handler = null;
                if (item.Callback != null)
                    handler = (s, e) => item.Callback();
		
                var nsItem = new NSMenuItem(item.Label, item.KeyEquivalent ?? "", handler);
		
                if (item.ModifierMask.HasValue)
                    nsItem.KeyEquivalentModifierMask = item.ModifierMask.Value;
		
                menu.AddItem(nsItem);
            }
	
            var rootItem = new NSMenuItem(title);
            rootItem.Submenu = menu;
            rootMenu.AddItem(rootItem);
        }

        public void Install()
        {
            NSApplication.SharedApplication.MainMenu = rootMenu;
        }
    }

    public class MenuItem
    {
        public string Label { get; set; }
        public Action Callback { get; set; }
        public string KeyEquivalent { get; set; }
        public NSEventModifierMask? ModifierMask { get; set; }

        public MenuItem(string label, Action callback = null, string keyEq = "", NSEventModifierMask? mods = null)
        {
            Label = label;
            Callback = callback;
            KeyEquivalent = keyEq;
            ModifierMask = mods;
        }
    }

    public static class MB
    {
        public static MenuItem I(string label, Action callback = null, string keyEq = "", NSEventModifierMask? mods = null)
        {
            return new MenuItem(label, callback, keyEq, mods);
        }
    }
}