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

namespace Skymu.Themes.S714
{
	public partial class LoginWindowController : NSWindowController
	{
		public LoginWindowController() : base("LoginWindow") {}

		public new LoginWindow Window => (LoginWindow)base.Window;
	}
}
