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

using CommunityToolkit.Mvvm.ComponentModel;
using QRCoder;
using Skymu.Credentials;
using Skymu.Classes;
using Skymu.Plugins;
using Skymu.Preferences;
using Skymu.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil;
using Yggdrasil.Models;
using Yggdrasil.Enumerations;

using AppKit;
using CoreGraphics;
using Foundation;

namespace Skymu.ViewModels
{
	public class LoginViewModel : ObservableObject
	{
		PluginListing selectedListing;

        public NSWindow Window;

        public event Action<bool> AnimationToggleRequested;
        public event Action<string> HeaderTextRequested;
        public event Action<PluginListing> PluginSelectionUpdated;
        public event Action OpenMainWindow;

        private ObservableCollection<PluginListing> pluginItems;
        public ObservableCollection<PluginListing> PluginItems
        {
            get => pluginItems;
            set => SetProperty(ref pluginItems, value);
        }
        
        public PluginListing SelectedListing
        {
            get { return selectedListing; }
            set
            {
                if (SetProperty(ref selectedListing, value))
                    HandleProtocolSelected(value);
            }
        }

        public SavedCredential PendingAutoLogin { get; private set; }
        public PluginListing PendingAutoLoginListing { get; private set; }
        public SavedCredential[] SavedCredentials { get; private set; }

        bool allowAutoLogin = true;
        
        public LoginViewModel()
        {
            pluginItems = new ObservableCollection<PluginListing>();
        }

        public void LoadPlugins()
        {
            PluginItems.Clear();
            PluginManager.DisposeAll();
            Universal.PluginList = PluginManager.Load(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0])), "..", "Plugins")) ?? new ICore[0];
            int pluginIndex = 0;
            SavedCredential[] savedCredentials = CredentialManager.GetAll();
            SavedCredentials = savedCredentials;

            foreach (var plugin in Universal.PluginList)
            {
#if DEBUG
                allowAutoLogin = !DebugConfig.DisableAutoLogin && !DebugConfig.TestMode;
                if (DebugConfig.TestMode && plugin.InternalName.ToLowerInvariant() == "stub")
                {
                    PendingAutoLogin = new SavedCredential(new User("Saul Goodman", "sgoodman", "sgoodman"), "sgoodman", AuthenticationMethod.Token, plugin.InternalName.ToLowerInvariant());
                    Universal.Plugin = plugin;
                    Universal.CallPlugin = Universal.Plugin as ICall;
                }
#endif

                SavedCredential match = null;
                foreach (SavedCredential cred in savedCredentials)
                {
                    if (cred.Plugin == plugin.InternalName)
                    {
                        match = cred;
                        break;
                    }
                }

                if (plugin.AuthenticationTypes.Length <= 1)
                {
                    var listing = new PluginListing(
                        plugin.Name,
                        pluginIndex,
                        plugin.InternalName,
                        plugin.AuthenticationTypes[0].AuthType,
                    plugin.AuthenticationTypes[0].CustomTextUsername,
                        plugin.AuthenticationTypes[0].CustomTextPassword
                    );

                    if (match != null && PendingAutoLogin == null && Settings.AutoLogin && allowAutoLogin)
                    {
                        PendingAutoLogin = match;
                        PendingAutoLoginListing = listing;
                        Universal.Plugin = plugin;
                        Universal.CallPlugin = Universal.Plugin as ICall;
                    }
                    PluginItems.Add(listing);
#if DEBUG
                    if (DebugConfig.TestMode && plugin.InternalName.ToLowerInvariant() == "stub")
                        PendingAutoLoginListing = listing;
#endif
                }
                else
                {
                    foreach (AuthTypeInfo ati in plugin.AuthenticationTypes)
                    {
                        string name = plugin.Name;
                        if (ati.CustomTextAuthType != null)
                        {
                            name += " - " + ati.CustomTextAuthType;
                        }
                        else
                        {
                            switch (ati.AuthType)
                            {
                                case AuthenticationMethod.Password:
                                    name += " - password";
                                    break;
                                case AuthenticationMethod.QRCode:
                                    name += " - QR code";
                                    break;
                                case AuthenticationMethod.Passwordless:
                                    name += " - passwordless";
                                    break;
                                case AuthenticationMethod.External:
                                    name += " - external login";
                                    break;
                                case AuthenticationMethod.Token:
                                    name += " - token login";
                                    break;
                                default:
                                    continue;
                            }
                        }
                        var listing = new PluginListing(name, pluginIndex, plugin.InternalName, ati.AuthType, ati.CustomTextUsername, ati.CustomTextPassword);
                        if (match != null && PendingAutoLogin == null && Settings.AutoLogin && allowAutoLogin) // TODO check against authentication type too?
                        {
                            PendingAutoLogin = match;
                            PendingAutoLoginListing = listing;
                            Universal.Plugin = plugin;
                            Universal.CallPlugin = Universal.Plugin as ICall;
                        }
                        PluginItems.Add(listing);
                    }
                }
                pluginIndex++;
            }
        }
        public void HandleProtocolSelected(PluginListing listing)
        {
            if (listing == null || PendingAutoLogin != null) return;
            selectedListing = listing;
            Universal.Plugin = Universal.PluginList[listing.PluginIndex];
            Universal.CallPlugin = Universal.Plugin as ICall;
            PluginSelectionUpdated?.Invoke(listing);
        }

        public void ClearPendingAutoLogin()
        {
            PendingAutoLogin = null;
            PendingAutoLoginListing = null;
        }

        public async void RunPostLogin(MainViewModel mainWindow)
        {
            // TODO Tray.SetStatus(Universal.CurrentUser.ConnectionStatus);
            Universal.HasLoggedIn = true;
            // TODO SoundManager.Play("LOGIN");
            // TODO new Updater();
            string brand = Settings.BrandingName;

            // request the user to publish their details on the public userlist
            if (!Settings.AnonymizeOptOutShown)
            {
                var alert = new NSAlert()
                {
                    MessageText = "Publicly display user statistics?",
                    InformativeText = Settings.BrandingName + " sends information such as your display name and username to its user count server by default. This is done to populate the user "
                        + "count at the bottom of the sidebar, and also to form a searchable list of online users.\n\nYour data is not retained, stored, cached, sold, or otherwise used by Skymu in any way. "
                        + "Your username and display name are only used to populate the list.\n\nTo improve the accuracy of the public list, it is recommended that you click 'Yes'."
                };
                alert.AddButton("Yes");
                alert.AddButton("No");
                var act = alert.RunModal();
                if (act == (uint)NSAlertButtonReturn.First)
                {
                    Settings.Anonymize = false;
                    Settings.AnonymizeOptOutShown = true;
                    Settings.Save();
                }
                else if (act == (uint)NSAlertButtonReturn.Second)
                {
                    Settings.Anonymize = true;
                    Settings.AnonymizeOptOutShown = true;
                    Settings.Save();
                }
            }
        }

        public PluginListing GetPreferredDefaultListing()
        {
            if (PluginItems == null || PluginItems.Count == 0)
                return null;

            // to not confuse users, the vast majority of who are looking for discord
            var discordListings = PluginItems
                .Where(p => string.Equals(p.InternalName, "discord", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (discordListings.Count > 0)
            {
                var discordQr = discordListings.FirstOrDefault(p => p.AuthenticationType == AuthenticationMethod.QRCode);
                if (discordQr != null)
                    return discordQr;

                return discordListings[0];
            }

            return PluginItems[0];
        }


        public async Task TryAutoLogin()
        {

            if (PendingAutoLogin == null)
            {
                AnimationToggleRequested?.Invoke(false);
                return;
            }

            // TODO Tray.SetConnecting();

            LoginResult lr = await Task.Run(async () =>
                await Universal.Plugin.Authenticate(PendingAutoLogin)
            );

            if (lr == LoginResult.Success)
            {
                await InitiateMain();
            }
            else
            {
                // TODO ray.SetStatus(PresenceStatus.Offline);
                PendingAutoLogin = null;
                selectedListing = PendingAutoLoginListing;
                PluginSelectionUpdated?.Invoke(PendingAutoLoginListing);
                AnimationToggleRequested?.Invoke(false);
                if (lr == LoginResult.Failure)
                    HeaderTextRequested?.Invoke(Universal.Lang["sF_USERENTRY_ERROR_1101"]);
            }
        }

        public async Task Login(string username, string password)
        {
            if (selectedListing == null) return;
            AnimationToggleRequested?.Invoke(true);
            // TODO Tray.SetConnecting();

            var result = await Universal.Plugin.Authenticate(
                selectedListing.AuthenticationType,
                username,
                password
            );

            if (result == LoginResult.Success)
            {
                await InitiateMain();
                return;
            }

            if (result == LoginResult.TwoFARequired)
            {
                await Handle2FA();
                return;
            }

            // TODO Tray.SetStatus(PresenceStatus.Offline);
            AnimationToggleRequested?.Invoke(false);
            HeaderTextRequested?.Invoke(Universal.Lang["sF_USERENTRY_ERROR_1101"]);
        }

        private async Task Handle2FA()
        {
            string totp = null;

            if (selectedListing.AuthenticationType == AuthenticationMethod.QRCode)
            {
                string qr = await Universal.Plugin.GetQRCode();
                if (!string.IsNullOrEmpty(qr))
                {
                    using (NSData data = NSData.FromArray(new PngByteQRCode(
                        new QRCodeGenerator().CreateQrCode(qr, QRCodeGenerator.ECCLevel.Q)
                    ).GetGraphic(20)))
                    {
                        var qralert = new NSAlert()
                        {
                            MessageText = "Scan code to authenticate",
                            Icon = new NSImage(NSBundle.MainBundle.PathForResource(
                                WindowIcons.ErrorIcon,
                                "png",
                                "WindowIcons")
                            ),
                            AccessoryView = new NSImageView(new CGRect(0, 0, 250, 250))
                            {
                                Image = new NSImage(data)
                            }
                        };
                        qralert.AddButton("Cancel");
                        bool done = false;
                        qralert.Layout();
                            if (!done && qralert.RunModal() == (uint)NSAlertButtonReturn.First)
                            {
                                AnimationToggleRequested?.Invoke(false);
                                HeaderTextRequested?.Invoke(Universal.Lang["sF_USERENTRY_ERROR_1101"]);
                                return;
                            }

                        new Task(async () =>
                        {
                            LoginResult qrResult = await Universal.Plugin.AuthenticateTwoFA(null);
                            if (qrResult == LoginResult.Success)
                            {
                                done = true;
                                Window.InvokeOnMainThread(() => qralert.Buttons[0].PerformClick(null));
                                await InitiateMain();
                                return;
                            }
                        }).Start();
                    }
                }
                AnimationToggleRequested?.Invoke(false);
                HeaderTextRequested?.Invoke(Universal.Lang["sF_USERENTRY_ERROR_1101"]);
                return;
            }

            var alert = new NSAlert()
            {
                MessageText = "Two-factor authentication required",
                InformativeText = Universal.Plugin.Name + " has requested that you provide a 2FA code to log in. Please enter it below.",
                Icon = NSImage.ImageNamed(WindowIcons.ErrorIcon),
                AccessoryView = new NSTextField(new CGRect(0, 0, 255, 23))
                {
                    Alignment = NSTextAlignment.Center,
                    Cell = new VerticallyCenteredTextFieldCell()
                }
            };
            alert.AddButton("Sign in");
            alert.AddButton("Cancel");
            alert.Layout();

            if (alert.RunModal() == (uint)NSAlertButtonReturn.First)
            {
                totp = ((NSTextField)alert.AccessoryView).StringValue;
                LoginResult optResult = await Universal.Plugin.AuthenticateTwoFA(totp);
                if (optResult == LoginResult.Success)
                {
                    await InitiateMain();
                    return;
                }
            }

            AnimationToggleRequested?.Invoke(false);
            HeaderTextRequested?.Invoke(Universal.Lang["sF_USERENTRY_ERROR_1101"]);
        }

        private async Task InitiateMain()
        {
            Debug.WriteLine($"[SKYMU] Login success. Initiating main window...");
            if (Settings.SaveCredentials)
            {
                SavedCredential cred = await Universal.Plugin.StoreCredential();
                if (cred != null)
                    CredentialManager.Save(cred);
            }

            HeaderTextRequested?.Invoke("Loading user data");

            OpenMainWindow?.Invoke();
        }

        public class PluginListing
        {
            public PluginListing(string name, int index, string internalName, AuthenticationMethod authType, string textUsername, string textPassword)
            {
                DisplayName = name;
                PluginIndex = index;
                InternalName = internalName;
                AuthenticationType = authType;
                TextUsername = textUsername;
                TextPassword = textPassword;
            }

            public string DisplayName { get; private set; }
            public int PluginIndex { get; private set; }
            public string InternalName { get; private set; }
            public AuthenticationMethod AuthenticationType { get; private set; }
            public string TextUsername { get; private set; }
            public string TextPassword { get; private set; }
        }
    }
}

