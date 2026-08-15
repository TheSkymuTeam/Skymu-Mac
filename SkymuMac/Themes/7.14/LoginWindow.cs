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
using CoreText;
using Foundation;
using Skymu.Preferences;
using Skymu.UserControls;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MSUI.Helper;
using MSUI.Helper.Quick;
using MSUI.QuickView;
using MSUI.Stack;
using Skymu.Helpers;
using Yggdrasil.Enumerations;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
    public sealed class LoginWindowController : NSWindowController
    {
        public LoginWindowController()
        {
            Window = new LoginWindow();
            MainMenu.Use();
        }
    }

    public sealed class LoginWindow : NSWindow
    {
        LoginViewModel vm;

        public LoginWindow() : base(
            new CGRect(0, 0, 720, 475),
            NSWindowStyle.Titled,
            NSBackingStore.Buffered,
            false
        )
        {
            Title = Settings.BrandingName;
            Center();
            MakeKeyAndOrderFront(this);
            ContentView = new LoginLoadingViewController().View;
            new Task(() =>
            {
                try
                {
                    vm = new LoginViewModel();
                    vm.LoadPlugins();
                    if (vm.PluginItems == null || vm.PluginItems.Count <= 0)
                    {
                        InvokeOnMainThread(() =>
                        {
                            new NSAlert
                            {
                                MessageText = "No plugins detected. Unable to continue."
                            }.RunModal();
                            NSRunningApplication.CurrentApplication.Terminate();
                        });
                    }
                    else
                    {
                        Debug.WriteLine("[SKYMU] Plugin loaded, transitioning to the main login view");
                        InvokeOnMainThread(async () =>
                        {
                            var controller = new LoginMainViewController();
                            controller.OnOpenMainWindow += OpenMainWindow;
                            controller.vm = vm;
                            controller.LoadView();
                            if (controller.View == null)
                            {
                                new NSAlert
                                {
                                    MessageText =
                                        "ViewController did not return a main login View to display. Unable to continue."
                                }.RunModal();
                                NSRunningApplication.CurrentApplication.Terminate();
                            }

                            ContentView.Dispose();
                            ContentView = controller.View;
                        });
                    }
                }
                catch (Exception ex)
                {
                    InvokeOnMainThread(() =>
                    {
                        Universal.ExceptionHandler(ex, "This is an unexpected exception happened during a critical process. The app will now close,");
                        NSRunningApplication.CurrentApplication.Terminate();
                    });
                }
            }).Start();
        }

        void OpenMainWindow()
        {
            var mwc = new MainWindowController();
            var mw = (MainWindow)mwc.Window;
            ((AppDelegate) NSApplication.SharedApplication.Delegate).activeWindow = mwc;
            if (mw == null)
            {
                mwc.Dispose();
                Universal.Plugin.Dispose();
                // set text "Skype can't connect."
                return;
            }
            ContentView.Dispose();
            Close();
            vm.RunPostLogin(mw.vm);
        }
    }

    #region Loading view

    public sealed class LoginLoadingViewController : NSViewController
    {
        public override void LoadView()
        {
            View = new NSView();
            View.GetLayer().BackgroundColor = Colorizer.C.LoginBackground.CGColor;

            new Image(ImageHelper.ThemedImage("loader_30fps", "gif"))
                {
                    Animates = true,
                    CanDrawSubviewsIntoLayer = true,
                    ImageScaling = NSImageScale.None
                }
                .NoTAMIC()
                .AddTo(View)
                .Center(View);

            new Image(ImageHelper.ThemedImage("skype-logo-136x60"))
                {
                    ImageScaling = NSImageScale.None
                }
                .NoTAMIC()
                .AddTo(View)
                .CenterX(View)
                .Con(View, NSLayoutAttribute.Top, 40);
        }
    }

    #endregion

    #region Main login view
    
    public sealed class LoginMainViewController : NSViewController
    {
        internal event Action OnOpenMainWindow;
        LoginViewModel.PluginListing selectedListing;
        internal LoginViewModel vm;

        NSPopUpButton protocolPopup;
        Label protocolLabel;
        NSTextField usernameBox;
        NSMutableAttributedString usernameMutable;
        NSTextField passwordBox;
        NSMutableAttributedString passwordMutable;
        NSButton loginButton;
        NSMutableAttributedString loginMutable;
        
        readonly NSImage disabledBtn = ImageHelper.ThemedImage("signin-pill-disabled");
        readonly NSImage enabledBtn  = ImageHelper.ThemedImage("signin-pill");

        public override void LoadView()
        {
            View = new NSView();
            View.GetLayer().BackgroundColor = Colorizer.C.LoginBackground.CGColor;

            // ReSharper disable once ObjectCreationAsStatement
            new SpacedStack(NSLayoutAttribute.Top,
                View,
                (38, new Image(ImageHelper.ThemedImage("ms_logos"))
                    .NoTAMIC()
                    .AddTo(View)
                    .CenterX(View)
                    .Height(32)
                    .Hold(GHolder.H)
                ),
                (20, new Label("Sign in")
                    {
                        Alignment = NSTextAlignment.Center,
                        Font = NSFont.FromFontName("SegoeUI", 32),
                        TextColor = NSColor.White,
                    }
                    .NoTAMIC()
                    .AddTo(View)
                    .CenterX(View)
                    .Width(6717) // no i do not have a blades
                ),
                // two of the 15 GTOE is to make something at least visible when something goes wrong. TODO remove?
                (0, new Stack(false,
                    new Label("with")
                        {
                            Alignment = NSTextAlignment.Center,
                            Font = NSFont.FromFontName("SegoeUI", 13),
                            TextColor = NSColor.White
                        }
                        .NoTAMIC(),
                    new NSView
                        {
                            AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                               NSViewResizingMask.WidthSizable
                        }
                        .NoTAMIC()
                        .HAll()
                        .Width(15, NSLayoutRelation.GreaterThanOrEqual)
                        .Height(25)
                        .Hold(GHolder.H)
                        // Yes, C# supports variable assign in an argument!
                        .AddAnd(protocolLabel = new Label("Grindr - username and password") // eta
                            {
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable,
                                Alignment = NSTextAlignment.Center,
                                Font = NSFont.FromFontName("SegoeUI", 13),
                                TextColor = NSColor.White
                            }
                            .NoTAMIC()
                        , view => view
                            .CenterY(GHolder.H.V)
                            .Con(GHolder.H.V, NSLayoutAttribute.Left)
                        )
                        .AddAnd(new Label("▼")
                            {
                                Alignment = NSTextAlignment.Center,
                                Font = NSFont.FromFontName("SegoeUI", 13),
                                TextColor = NSColor.White
                            }
                            .NoTAMIC()
                        , view => view
                            .Con(GHolder.H.V, protocolLabel, NSLayoutAttribute.Left, NSLayoutAttribute.Right, 3)
                            .Con(GHolder.H.V, NSLayoutAttribute.Right)
                        )
                        .AddAnd(protocolPopup = new NSPopUpButton
                            {
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable,
                                Bordered = false,
                                AlphaValue = 0
                            }
                            .NoTAMIC()
                            .mpAll(1)
                        , view => view
                            .CenterY(GHolder.H.V)
                            .Con(GHolder.H.V, NSLayoutAttribute.Left)
                            .Con(GHolder.H.V, NSLayoutAttribute.Right)
                            .OnActivated((s, e) =>
                            {
                                protocolLabel.StringValue = protocolPopup.SelectedItem.Title;
                                foreach (var pl in vm.PluginItems)
                                {
                                    if (pl.InternalName == protocolPopup.SelectedItem.Identifier &&
                                        (int)pl.AuthenticationType == (int)protocolPopup.SelectedItem.Tag)
                                        selectedListing = pl;
                                }
                                vm.SelectedListing = selectedListing;
                            })
                        )
                    )
                    {
                        AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                           NSViewResizingMask.WidthSizable,
                        Spacing = 0
                    }
                    .NoTAMIC()
                    .AddTo(View)
                    .HAll()
                    .Width(15, NSLayoutRelation.GreaterThanOrEqual)
                    .Height(25)
                    .CenterX(View))
            );

            var form = new Stack(true, 
                new NSView
                    {
                        AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                           NSViewResizingMask.WidthSizable
                    }
                    .NoTAMIC(),
                    tup => tup.view
                        .Con(tup.parent, NSLayoutAttribute.Left)
                        .Con(tup.parent, NSLayoutAttribute.Right)
                        .Size(280, 36)
                        .AddAnd(usernameBox = new NSTextField
                            {
                                TranslatesAutoresizingMaskIntoConstraints = false,
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable,
                                Bordered = false,
                                DrawsBackground = false,
                                Font = NSFont.FromFontName("SegoeUI-Light", 16),
                                Cell =
                                {
                                    PlaceholderAttributedString = usernameMutable = new NSMutableAttributedString("Skype name, email or phone", new CTStringAttributes
                                    {
                                        ForegroundColor = Colorizer.C.LoginPlaceholder.CGColor
                                    })
                                },
                                TextColor = NSColor.White
                            }
                                .OnActivated(OnUsernameEnter)
                                .OnChanged(Typing),
                            tf => tf
                                .CenterY(tup.view)
                                .Con(tup.view, NSLayoutAttribute.Left)
                                .Con(tup.view, NSLayoutAttribute.Right)
                        )
                        .AddAnd(new ColoredLineView
                            {
                                TranslatesAutoresizingMaskIntoConstraints = false,
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable | NSViewResizingMask.MaxYMargin,
                                StrokeColor = Colorizer.C.LoginFormLine
                            },
                            cl => cl
                                .Con(tup.view, NSLayoutAttribute.Left)
                                .Con(tup.view, NSLayoutAttribute.Right)
                                .Con(tup.view, NSLayoutAttribute.Bottom)
                                .Height(1)
                        ),
                    new NSView
                    {
                        AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                           NSViewResizingMask.WidthSizable
                    }
                    .NoTAMIC(),
                    tup => tup.view
                        .Con(tup.parent, NSLayoutAttribute.Left)
                        .Con(tup.parent, NSLayoutAttribute.Right)
                        .Size(280, 36)
                        .AddAnd(passwordBox = new NSSecureTextField
                            {
                                TranslatesAutoresizingMaskIntoConstraints = false,
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable,
                                Bordered = false,
                                DrawsBackground = false,
                                Font = NSFont.FromFontName("SegoeUI-Light", 16),
                                Cell =
                                {
                                    PlaceholderAttributedString = passwordMutable = new NSMutableAttributedString("Password", new CTStringAttributes
                                    {
                                        ForegroundColor = Colorizer.C.LoginPlaceholder.CGColor
                                    })
                                },
                                TextColor = NSColor.White
                            }
                                .OnActivated(OnPasswordEnter)
                                .OnChanged(Typing),
                            tf => tf
                                .CenterY(tup.view)
                                .Con(tup.view, NSLayoutAttribute.Left)
                                .Con(tup.view, NSLayoutAttribute.Right)
                        )
                        .AddAnd(new ColoredLineView
                            {
                                TranslatesAutoresizingMaskIntoConstraints = false,
                                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                                   NSViewResizingMask.WidthSizable | NSViewResizingMask.MaxYMargin,
                                StrokeColor = Colorizer.C.LoginFormLine
                            },
                            cl => cl
                                .Con(tup.view, NSLayoutAttribute.Left)
                                .Con(tup.view, NSLayoutAttribute.Right)
                                .Con(tup.view, NSLayoutAttribute.Bottom)
                                .Height(1)
                        )
                )
                {
                    AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxYMargin |
                                       NSViewResizingMask.HeightSizable,
                }
                .NoTAMIC()
                .AddTo(View)
                .Center(View)
                .Width(280);

            loginButton = new NSButton
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Bordered = false,
                    ImagePosition = NSCellImagePosition.ImageOverlaps,
                    ImageScaling = NSImageScale.ProportionallyUpOrDown,
                    AttributedTitle = loginMutable = new NSMutableAttributedString("Sign in", new CTStringAttributes
                    {
                        ForegroundColor = NSColor.Gray.CGColor
                    }).Do(muta => muta.SetAlignment(NSTextAlignment.Center, new NSRange(0, muta.Value.Length))),
                    Cell =
                    {
                        BackgroundColor = NSColor.FromRgba(0, 0, 0, 0)
                    }
                }
                .AddTo(View)
                .CenterX(View)
                .Size(130, 35)
                .Con(View, form, NSLayoutAttribute.Top, 25)
                .OnActivated(OnLogin);
            
            protocolPopup.RemoveAllItems();
            foreach (var p in vm.PluginItems)
            {
                protocolPopup.AddItem(p.DisplayName);
                var item = protocolPopup.Items()[(int)protocolPopup.ItemCount - 1];
                item.Identifier = p.InternalName;
                item.Tag = (int) p.AuthenticationType;
            }
            vm.PluginSelectionUpdated += OnPluginSelectionUpdated;
            // TODO default to Spycord QR if found
            if (protocolPopup.ItemCount >= 8)
            {
                protocolPopup.SelectItem(7);
                vm.SelectedListing = vm.PluginItems[7];
            }
            else
            {
                protocolPopup.SelectItem(0);
                vm.SelectedListing = vm.PluginItems[0];
            }
            vm.OpenMainWindow += OnOpenMainWindow;
        }

        void Typing(object sender, EventArgs e) =>
            CheckEnableLoginButton();

        void OnPluginSelectionUpdated(LoginViewModel.PluginListing listing)
        {
            selectedListing = listing;

            passwordBox.Enabled = true;
            passwordMutable.MutableString.SetString(new NSString(listing.TextPassword ?? "Password"));
            loginMutable.MutableString.SetString(new NSString("Sign in"));
            usernameBox.Enabled = true;
            usernameMutable.MutableString.SetString(new NSString (listing.TextUsername ?? "Username"));

            if (listing.AuthenticationType != AuthenticationMethod.Password)
            {
                passwordBox.Enabled = false;
                passwordBox.StringValue = "";
                passwordMutable.MutableString.SetString(new NSString ("field not required"));

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (listing.AuthenticationType)
                {
                    case AuthenticationMethod.QRCode:
                        loginMutable.MutableString.SetString(new NSString("Scan QR code"));
                        usernameBox.Enabled = false;
                        usernameBox.StringValue = "";
                        usernameMutable.MutableString.SetString(new NSString ("field not required"));
                        break;
                    case AuthenticationMethod.Passwordless:
                        loginMutable.MutableString.SetString(new NSString("Send code"));
                        break;
                    case AuthenticationMethod.External:
                        loginMutable.MutableString.SetString(new NSString("External login"));
                        break;
                }
            }

            usernameBox.Cell.PlaceholderAttributedString = usernameMutable;
            passwordBox.Cell.PlaceholderAttributedString = passwordMutable;
            loginButton.Cell.AttributedTitle = loginMutable;

            CheckEnableLoginButton();
            OnProtocolChanged();
        }

        void CheckEnableLoginButton()
        {
            loginButton.Enabled = 
                !string.IsNullOrWhiteSpace(usernameBox.StringValue)
                && (!passwordBox.Enabled || !string.IsNullOrWhiteSpace(usernameBox.StringValue))
                || !passwordBox.Enabled && !usernameBox.Enabled;
            var attrs = new NSMutableDictionary
            {
                [NSStringAttributeKey.ForegroundColor] = loginButton.Enabled ? NSColor.White : NSColor.Gray
            };

            loginMutable.SetAttributes(attrs, new NSRange(0, loginMutable.Value.Length));
            loginMutable.SetAlignment(NSTextAlignment.Center, new NSRange(0, loginMutable.Value.Length));
            loginButton.AttributedTitle = loginMutable;
            loginButton.Image = loginButton.Enabled ? enabledBtn : disabledBtn;
        }

        void OnUsernameEnter(object sender, EventArgs e)
        {
            if (passwordBox.Enabled && string.IsNullOrWhiteSpace(passwordBox.StringValue))
                passwordBox.SelectText(null);
            else if (!string.IsNullOrWhiteSpace(usernameBox.StringValue))
                OnLogin(null, null);
        }

        void OnPasswordEnter(object sender, EventArgs e)
        {
            if (usernameBox.Enabled && string.IsNullOrWhiteSpace(usernameBox.StringValue))
                usernameBox.SelectText(null);
            else if (!string.IsNullOrWhiteSpace(passwordBox.StringValue))
                OnLogin(null, null);
        }

        async void OnLogin(object sender, EventArgs e)
            => await vm.Login(usernameBox.StringValue, passwordBox.StringValue);

        void OnProtocolChanged()
        {
            protocolLabel.StringValue = protocolPopup.SelectedItem.Title;
            foreach (var pl in vm.PluginItems)
            {
                if (pl.InternalName == protocolPopup.SelectedItem.Identifier &&
                    (int)pl.AuthenticationType == (int)protocolPopup.SelectedItem.Tag)
                    selectedListing = pl;
            }
            vm.SelectedListing = selectedListing;
        }
    }
    
    #endregion
}