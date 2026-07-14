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
using Skymu.Preferences;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Skymu
{
    public class LanguageManager
    {
        public string this[string key]
        {
            get => Get(key);
        }
        public string Get(string key)
        {
            return key;
            var value = NSBundle.FromPath(
                Path.Combine(NSBundle.MainBundle.ResourcePath, NSLocale.CurrentLocale.LanguageCode + ".lproj")).LocalizedString(key, key, "Localizable");

            return value
                .Replace("Skype", Settings.BrandingName)
                .Replace("skype:", Universal.NAME.ToLowerInvariant() + ":");
        }

        public string Format(string key, params object[] args)
        {
            var value = Get(key);

            value = value.Replace("%%", "%");

            int i = 0;
            value = Regex.Replace(value, "%[dfs]", _ => "{" + i++.ToString() + "}");

            return string.Format(value, args);
        }
    }
}
