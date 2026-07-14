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

using System;
namespace Skymu.Classes
{
	public static class WindowIcons
	{
		public static readonly string Globe = "globe_64";
		public static readonly string Notification = "notification_64";
		public static readonly string SignalStrength0 = "signal-strength-0_64";
		public static readonly string SignalStrength1 = "signal-strength-1_64";
		public static readonly string SignalStrength2 = "signal-strength-2_64";
		public static readonly string SignalStrength3 = "signal-strength-3_64";
		public static readonly string SkypeCredit = "skypecredit_64";
		public static readonly string SkypeCreditBlue = "skypecredit-blue_64";
		public static readonly string WiFi = "wifi_64";
		public static readonly string WiFiBig = "wifi-big_64";
		public static readonly string WiFiEars = "wifi-ears_64";
		public static readonly string CheckIcon = "CheckIcon";
		public static string WarningIcon
		{
			get => Universal.Theme == "5" ? "SkypeBlueWarning.icns" : "ErrorIcon";
		}
        public static readonly string ErrorIcon = "ErrorIcon";
		public static readonly string LargeFacebookErrorIcon = "LargeFacebookErrorIcon";
		public static readonly string SkypeCreditIcon = "SkypeCreditIcon";
    }
}

