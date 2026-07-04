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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Yggdrasil.Enumerations;
using Yggdrasil.Models;

using Foundation;
using Security;
using System.Text;
using CoreImage;

namespace Skymu.Credentials
{
    internal static class CredentialManager
    {
        internal static void Save(SavedCredential credential)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
                // there_com.An_Unknown_User, stub, stub.sgoobman, etc...
                // usenrame in b64 ofc
                Account = credential.Plugin.Replace(".", "_")
                + credential.User?.Identifier == null ? ""
                : ("." + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.User?.Identifier ?? ""))),
                // stub.1.sgoodman.sgoodman.Saul Goodman.PASSWORD.AVATAR all in base64 except for auth type
                ValueData = NSData.FromString(
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.Plugin))
                    + "." + ((int)credential.AuthenticationType).ToString()
                    + "." + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.User.Identifier))
                    + "." + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.User.Username))
                    + "." + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.User.DisplayName))
                    + "." + Convert.ToBase64String(Encoding.ASCII.GetBytes(credential.PasswordOrToken))
                    + "." + Convert.ToBase64String(credential.User.Avatar)
                )
            };

            SecKeyChain.Remove(record);

            var statusCode = SecKeyChain.Add(record);
            if (statusCode != SecStatusCode.Success)
                throw new Exception(statusCode.ToString());
        }

        static SavedCredential decode(string data)
        {
            try
            {
                string[] split = data.Split(".");
                return new SavedCredential(
                    new User(
                        Encoding.ASCII.GetString(Convert.FromBase64String(split[4])),
                        Encoding.ASCII.GetString(Convert.FromBase64String(split[3])),
                        Encoding.ASCII.GetString(Convert.FromBase64String(split[2])),
                        avatar: Convert.FromBase64String(split[6])
                    ),
                    Encoding.ASCII.GetString(Convert.FromBase64String(split[5])),
                    (AuthenticationMethod)int.Parse(split[1]),
                    Encoding.ASCII.GetString(Convert.FromBase64String(split[0]))
                );
            }
            catch
            {
                return null;
            }
        }

        internal static SavedCredential Get(User user, string plugin)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
                Account = plugin.Replace(".", "_") + "." + Convert.ToBase64String(Encoding.ASCII.GetBytes(user.Identifier))
            };

            var match = SecKeyChain.QueryAsRecord(record, out var resultCode);

            if (resultCode == SecStatusCode.Success && match != null)
            {
                return decode(NSString.FromData(match.ValueData, NSStringEncoding.UTF8));
            }

            return null;
        }

        internal static SavedCredential GetFirst(string plugin)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
            };

            var match = SecKeyChain.QueryAsRecord(record, 1, out var resultCode);
            if (resultCode == SecStatusCode.Success && match != null)
            {
                foreach (var cred in match)
                {
                    var sc = decode(NSString.FromData(cred.ValueData, NSStringEncoding.UTF8));
                    if (sc?.Plugin == plugin)
                        return sc;
                }
            }

            return null;
        }

        internal static SavedCredential[] GetAll()
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
            };

            var results = new List<SavedCredential>();
            var match = SecKeyChain.QueryAsRecord(record, 1, out var resultCode);
            if (resultCode == SecStatusCode.Success && match != null)
            {
                foreach (var cred in match)
                {
                    var sc = decode(NSString.FromData(cred.ValueData, NSStringEncoding.UTF8));
                    if (sc != null)
                        results.Add(sc);
                }
            }

            return results.ToArray();
        }

        internal static void Purge(User user, string plugin)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
            };

            var match = SecKeyChain.QueryAsRecord(record, 1, out var resultCode);
            if (resultCode == SecStatusCode.Success && match != null)
            {
                foreach (var cred in match)
                {
                    var sc = decode(NSString.FromData(cred.ValueData, NSStringEncoding.UTF8));
                    if (sc?.Plugin == plugin && sc?.User?.Identifier == user.Identifier)
                        SecKeyChain.Remove(cred);
                }
            }
        }

        internal static void PurgePlugin(string plugin)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
            };

            var match = SecKeyChain.QueryAsRecord(record, 1, out var resultCode);
            if (resultCode == SecStatusCode.Success && match != null)
            {
                foreach (var cred in match)
                {
                    var sc = decode(NSString.FromData(cred.ValueData, NSStringEncoding.UTF8));
                    if (sc?.Plugin == plugin)
                        SecKeyChain.Remove(cred);
                }
            }
        }

        internal static void PurgeAll()
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = NSBundle.MainBundle.BundleIdentifier,
            };

            var match = SecKeyChain.QueryAsRecord(record, 1, out var resultCode);
            if (resultCode == SecStatusCode.Success && match != null)
            {
                foreach (var cred in match)
                {
                    SecKeyChain.Remove(cred);
                }
            }
        }
    }
}
