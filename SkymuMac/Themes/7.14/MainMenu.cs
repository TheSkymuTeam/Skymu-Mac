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
using Skymu.Menu;
using static Skymu.Menu.MB;

namespace Skymu.Themes.S714
{
    public static class MainMenu
    {
        public static void Use()
        {
            var menu = new MenuBar();
            menu.Create("Skymu",
                I("About", () => {}),
                I("Quit", () => NSRunningApplication.CurrentApplication.Terminate(), "q", NSEventModifierMask.CommandKeyMask)
            );
            menu.Create("File",
                I("New", () => {}, "n", NSEventModifierMask.CommandKeyMask),
                I("Open", () => {}, "o", NSEventModifierMask.CommandKeyMask)
            );
            menu.Install();
        }
    }
}