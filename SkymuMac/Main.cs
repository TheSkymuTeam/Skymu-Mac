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
using OmegaAOL.Bifrost.Http;
using Skymu.Classes;
using Skymu.Migration;
using Skymu.Preferences;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Yggdrasil;
using Yggdrasil.Bottles;
using Yggdrasil.Enumerations;
using Yggdrasil.Models;

namespace Skymu
{
    static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
            Debug.WriteLine("[SKYMU] Initialized NSApplication");
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            Debug.WriteLine("[SKYMU] Delegate set to our own one");
            NSApplication.Main(args);
            Debug.WriteLine("[SKYMU] Successful exit!");
        }
    }

	public static class Universal
	{
        // -----------------------------------------------------------------------------
        // Skymu metadata.
        // -----------------------------------------------------------------------------

        public const string NAME = "Skymu";
        public const string BUILD_VERSION = "0.1.0";
        public const string BUILD_NAME = "Breakpoint Activated";

        // -----------------------------------------------------------------------------
        // Skymu URLs.
        // -----------------------------------------------------------------------------

        public const string GITHUB_OWNER = "TheSkymuTeam";
        public const string GITHUB_REPO = "Skymu-Mac";
        public const string DISCORD_SERVER_INVITE = "https://skymu.app/discord";
        public const string SKYMU_WEBSITE_HELP = "https://skymu.app/wiki/about";
        public const string SKYMU_WEBSITE_PRIVACY = "https://skymu.app/legal/privacy";

        // -----------------------------------------------------------------------------
        // External URLs.
        // -----------------------------------------------------------------------------

        public const string EASTER_SKYPE_SOUNDS_REMIX = "https://www.youtube.com/watch?v=kVsH_ySm5_E";
        public const string EASTER_CHANTE_SKYPE = "https://www.youtube.com/watch?v=cdtNIyx10DM";
        public const string GITHUB_BASE_URL = "https://api.github.com/repos/" + GITHUB_OWNER + "/" + GITHUB_REPO;
        public const string GITHUB_RELEASES_URL = GITHUB_BASE_URL + "/releases/latest";
        public const string GITHUB_PULLS_URL = GITHUB_BASE_URL + "/pulls";

        // -----------------------------------------------------------------------------
        // Globally scoped variables.
        // -----------------------------------------------------------------------------


        public const string EX_IS_OKAY = "This will NOT close " + NAME + ", or log you out.";
        public const string EX_IS_BAD = NAME + " will quit after closing this alert.";

        public static ICore Plugin;
        public static ICall CallPlugin;
        public static ICore[] PluginList;
        public static bool HasLoggedIn = false;
        public static readonly string Theme = Settings.Theme;
        public static string Platform = NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
        public static string NetVersion = RuntimeInformation.FrameworkDescription;
        public static User CurrentUser;
        public static NSImage GroupAvatar;
        public static NSImage ContactAvatar;
        public static MainViewModel ActiveViewModel;
        public static LanguageManager Lang = new LanguageManager();

        public static event Action ThemeChanged;

        private static bool _isDarkTheme = false;
        public static bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme == value) return;
                _isDarkTheme = value;
                ThemeChanged?.Invoke();
            }
        }

        private static void PluginPopup(
            object sender,
            DialogBottle e,
            string prefix,
            string icon = null
        )
        {
            NSRunningApplication.CurrentApplication.BeginInvokeOnMainThread(() =>
            {
                var alert = new NSAlert
                {
                    MessageText = prefix + ((ICore)sender).Name,
                    InformativeText = e.Message,
                    Icon = string.IsNullOrEmpty(icon) ? null : new NSImage(NSBundle.MainBundle.PathForResource(
                        icon,
                        "png",
                        Theme)
                    ),
                };
                alert.AddButton("OK");
                if (!string.IsNullOrEmpty(e.CopyToClipboardText))
                    alert.AddButton("Copy to clipboard");
                if (alert.RunModal() == (uint)NSAlertButtonReturn.Second)
                {
                    NSPasteboard.GeneralPasteboard.SetStringForType(e.CopyToClipboardText, NSPasteboard.NSStringType);
                    ShowMessage("Copied to clipboard.");
                }
            });
        }

        public static void PluginDialogHandler(object sender, DialogBottle e)
        {
            switch (e.Type)
            {
                case DialogType.Warning:
                    PluginPopup(sender, e, "Warning from plugin ", WindowIcons.WarningIcon);
                    break;
                case DialogType.Error:
                    PluginPopup(sender, e, "Error in plugin ", WindowIcons.ErrorIcon);
                    break;
                case DialogType.Information:
                    PluginPopup(sender, e, "Message from plugin ", null);
                    break;
                case DialogType.Choice:
                    NSRunningApplication.CurrentApplication.BeginInvokeOnMainThread(() =>
                    {
                        var alert = new NSAlert
                        {
                            MessageText = ((ICore)sender).Name + "requests your choice",
                            InformativeText = e.Message,
                            Icon = NSImage.ImageNamed(WindowIcons.WarningIcon),
                        };
                        alert.AddButton("Yes");
                        alert.AddButton("No");
                        alert.Layout();
                        var act = alert.RunModal();
                        if (act == (uint)NSAlertButtonReturn.First || act == (uint)NSAlertButtonReturn.Second)
                            e.Action(act == (uint)NSAlertButtonReturn.First);
                    });
                    break;
            }
        }

        public static void PluginNotificationHandler(object sender, MessageBottle e)
        {
            NSRunningApplication.CurrentApplication.BeginInvokeOnMainThread(
                new Action(
                    delegate
                    {
                        ActiveViewModel?.HandleIncoming(e);
                    }
                )
            );
        }

        public static string GetCultureCode(string displayName)
        {
            try
            {
                return CultureInfo
                        .GetCultures(CultureTypes.AllCultures)
                        .FirstOrDefault(c =>
                            c.NativeName.StartsWith(displayName)
                            || c.DisplayName.StartsWith(displayName)
                            || c.EnglishName.StartsWith(displayName)
                        )
                        ?.Name
                    ?? "en-US";
            }
            catch { }
            return "en-US";
        }

        public static void URIHandler(string uri)
        {
            if (uri.StartsWith("?"))
            {
                var cmd = uri.Substring(1);
                // TODO: Handle URI commands
            }
            else if (uri.StartsWith("#"))
            {
                // TODO: Handle "add" with AddContact thing
            }
            else
            {
                var questionmark = uri.IndexOf("?");
                var skypename = uri.Substring(0, questionmark == -1 ? uri.Length : questionmark);
                /*if (ActiveViewModel != null)
                {
                    Conversation found = null;
                    foreach (var c in ActiveViewModel.ConversationList)
                        if ((c is DirectMessage u) && u.Partner.Username == skypename)
                        {
                            found = c;
                            break;
                        }
                    if (found == null)
                        foreach (DirectMessage u in ActiveViewModel.ContactList)
                            if (u.Partner.Username == skypename)
                            {
                                found = u;
                                break;
                            }
                    if (found != null)
                        Current.Dispatcher.Invoke(() => ActiveViewModel.SelectConversation(found));
                } TODO - although do Mac even use this... oh yeah it does lol */
            }
        }

        public static void Restart()
        {
            NotImplemented("Restarting automatically");
        }

        internal static readonly HttpClient SkymuHttpClient = new HttpClient(new BifrostEngine())
        {
            Timeout = TimeSpan.FromSeconds(10),
        };

        public static void ExceptionHandler(Exception ex, string context = "This popup was manually invoked. Unless it happened during the initialization, there is a very low chance of this causing a critical action, such as logging you out or exiting the app.")
        {
            Debug.WriteLine(ex);
            var alert = new NSAlert
            {
                MessageText = "That wasn't supposed to happen...",
                InformativeText = context,
                Icon = new NSImage(NSBundle.MainBundle.PathForResource(
                    WindowIcons.ErrorIcon,
                    "png",
                    Theme)
                ),
                AccessoryView = new NSTextField(new CGRect(0, 0, 400, 350))
                {
                    Editable = false,
                    Selectable = true,
                    BackgroundColor = NSColor.ControlBackground,
                    StringValue = ex.ToString()
                }
            };
            alert.AddButton("OK");
            alert.AddButton("Copy to clipboard");
            alert.Layout();
            if (alert.RunModal() == (uint)NSAlertButtonReturn.Second)
                NSPasteboard.GeneralPasteboard.SetStringForType(ex.Message, NSPasteboard.NSStringType);
        }

        public static void ShowMessage(
            string content,
            string title = null,
            string icon = null
        )
        {
            new NSAlert
            {
                MessageText = title,
                InformativeText = content,
                Icon = string.IsNullOrEmpty(icon) ? null : new NSImage(NSBundle.MainBundle.PathForResource(
                    icon,
                    "png",
                    Theme)
                ),
            }.RunModal();
        }

        public static void NotImplemented(string feature)
        {
            new NSAlert
            {
                Icon = NSImage.ImageNamed(WindowIcons.WarningIcon),
                MessageText = "Feature not implemented",
                InformativeText = feature + " hasn't been added to " + Settings.BrandingName + " yet."
            }.RunModal();
        }

        public static void OnStartup()
        {
            if (!Settings.UseSystemCulture)
                CultureInfo.CurrentCulture = new CultureInfo(
                    GetCultureCode(Settings.Language),
                    false
                );
            Migrator.Run();
            SkymuHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{NAME}Client-" + BUILD_VERSION);
            Settings.Default.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "Colorway":
                    case "Theme":
                    case "UseSystemCulture":
                        var alert = new NSAlert
                        {
                            Icon = new NSImage(NSBundle.MainBundle.PathForResource(
                                WindowIcons.WarningIcon,
                                "png",
                                Theme)
                            ),
                            AlertStyle = NSAlertStyle.Warning,
                            MessageText = "Restart " + Settings.BrandingName + "?",
                            InformativeText = "You need to restart " + Settings.BrandingName + " to fully apply this change. Would you like to save your settings and restart?",
                        };
                        alert.AddButton("OK");
                        alert.AddButton("Cancel");
                        if (alert.RunModal() == (uint)NSAlertButtonReturn.First)
                        {
                            Settings.Save();
                            Restart();
                        }
                        break;
                }
            };
        }

        const string NOGOODEXCEPTIONHANDLING = "This exception was inside of AppDomain. Unfortunately, as of now, " + NAME + " will be terminated after pressing OK.";
        static Universal()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception exception = e.ExceptionObject as Exception;

                if (exception != null)
                {
                    ExceptionHandler(exception, NOGOODEXCEPTIONHANDLING);
                }
                else
                {
                    ExceptionHandler(
                        new Exception(
                            $"{NAME} Exception Handling: CurrentDomain non-exception object thrown of an unknown nature.\n\n"
                                + e.ToString()
                        ),
                        NOGOODEXCEPTIONHANDLING
                    );
                }
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                NSRunningApplication.CurrentApplication.BeginInvokeOnMainThread(() =>
                    ExceptionHandler(e.Exception, "This exception was inside of TaskScheduler. This alert can be safely closed.")
                );
                e.SetObserved();
            };
            
            SQLitePCL.Batteries_V2.Init();
        }

        public static void OpenUrl(string url)
        {
#if !NET5_0_OR_GREATER
            NSWorkspace.SharedWorkspace.OpenURL( 
#else
            NSWorkspace.SharedWorkspace.OpenUrl(
#endif
                new NSUrl(url), NSWorkspaceLaunchOptions.Async, null, out var err);
            if (err != null)
                throw new Exception(err.Description);
        }
    }
}
