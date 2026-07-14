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
using CoreText;
using Foundation;
using Skymu.Preferences;
using Skymu.Quick;
using Skymu.UserControls;
using Skymu.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                        Universal.ExceptionHandler(ex);
                        NSRunningApplication.CurrentApplication.Terminate();
                    });
                }
            }).Start();
        }

        async void OpenMainWindow()
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
            SetupLoading();
        }

        private void SetupLoading()
        {
            View.WantsLayer = true;
            View.Layer.BackgroundColor = Colorizer.C.LoginBackground.CGColor;

            var throbber = new NSImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = QImg.ThemedImage("loader_30fps", "gif"),
                Animates = true,
                CanDrawSubviewsIntoLayer = true,
                ImageScaling = NSImageScale.None
            };
            View.AddSubview(throbber);
            QCon.Center(throbber, View);

            var logo = new NSImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = QImg.ThemedImage("skype-logo-136x60"),
                ImageScaling = NSImageScale.None
            };
            View.AddSubview(logo);
            QCon.CenterX(logo, View);
            QCon.Con(logo, View, NSLayoutAttribute.Top, 40);
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
        
        readonly NSImage disabledBtn =  QImg.ThemedImage("signin-pill-disabled");
        readonly NSImage enabledBtn = QImg.ThemedImage("signin-pill");

        public override void LoadView()
        {
            View = new NSView();
            SetupLoading();
        }

        private void SetupLoading()
        {
            View.WantsLayer = true;
            View.Layer.BackgroundColor = Colorizer.C.LoginBackground.CGColor;

            var logo = new NSImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = QImg.ThemedImage("ms_logos")
            };
            View.AddSubview(logo);
            QCon.CenterX(logo, View);
            QCon.Con(logo, View, NSLayoutAttribute.Top, 38);
            QCon.Height(logo, 32);

            var signinHint = new Label("Sign in")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = NSTextAlignment.Center,
                Font = NSFont.FromFontName("SegoeUI", 32),
                TextColor = NSColor.White,
            };
            View.AddSubview(signinHint);
            QCon.CenterX(signinHint, View);
            QCon.Width(signinHint, 6717); // no i do not have a blades
            QCon.Con(View, signinHint, logo, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom, 20);

            var protocolStack = new NSStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable,
                Spacing = 0
            };
            View.AddSubview(protocolStack);
            QHug.All(protocolStack);
            QCon.CenterX(protocolStack, View);
            QCon.Con(View, protocolStack, signinHint, NSLayoutAttribute.Top, NSLayoutAttribute.Bottom, 0);
            QCon.Width(protocolStack, 15,
                NSLayoutRelation.GreaterThanOrEqual); // in case layout issues occur, it's less confusing
            QCon.Height(protocolStack, 25);

            var protocolSpace = new Label("")
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            protocolStack.AddView(protocolSpace, NSStackViewGravity.Center);

            var protocolWith = new Label("with")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = NSTextAlignment.Center,
                Font = NSFont.FromFontName("SegoeUI", 13),
                TextColor = NSColor.White
            };
            protocolStack.AddView(protocolWith, NSStackViewGravity.Center);

            var protocolSelector = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable
            };
            protocolStack.AddView(protocolSelector, NSStackViewGravity.Center);
            QHug.All(protocolSelector);
            QCon.Width(protocolSelector, 15, NSLayoutRelation.GreaterThanOrEqual); // another "hey smth is wrong" thing
            QCon.Height(protocolSelector, 25);

            protocolLabel = new Label("Grindr - username and password") // eta
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable,
                Alignment = NSTextAlignment.Center,
                Font = NSFont.FromFontName("SegoeUI", 13),
                TextColor = NSColor.White
            };
            protocolSelector.AddSubview(protocolLabel);
            QCon.CenterY(protocolLabel, protocolSelector);
            QCon.Con(protocolLabel, protocolSelector, NSLayoutAttribute.Left);

            var protocolDropLabel = new Label("▼")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = NSTextAlignment.Center,
                Font = NSFont.FromFontName("SegoeUI", 13),
                TextColor = NSColor.White
            };
            protocolSelector.AddSubview(protocolDropLabel);
            QCon.Con(protocolSelector, protocolLabel, protocolDropLabel, NSLayoutAttribute.Right, NSLayoutAttribute.Left, 3);
            QCon.Con(protocolDropLabel, protocolSelector, NSLayoutAttribute.Right);

            protocolPopup = new NSPopUpButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable,
                Bordered = false,
                Transparent = true
            };
            protocolSelector.AddSubview(protocolPopup);
            QComp.All(protocolPopup, 1);
            QCon.CenterY(protocolPopup, protocolSelector);
            QCon.Con(protocolPopup, protocolSelector, NSLayoutAttribute.Left);
            QCon.Con(protocolPopup, protocolSelector, NSLayoutAttribute.Right);

            protocolPopup.RemoveAllItems();
            foreach (var p in vm.PluginItems)
            {
                protocolPopup.AddItem(p.DisplayName);
                var item = protocolPopup.Items()[protocolPopup.ItemCount - 1];
                item.Identifier = p.InternalName;
                item.Tag = (int) p.AuthenticationType;
            }

            protocolPopup.Activated += (s, e) =>
            {
                
                protocolLabel.StringValue = protocolPopup.SelectedItem.Title;
                foreach (var pl in vm.PluginItems)
                {
                    if (pl.InternalName == protocolPopup.SelectedItem.Identifier &&
                        (int)pl.AuthenticationType == protocolPopup.SelectedItem.Tag)
                        selectedListing = pl;
                }
                vm.SelectedListing = selectedListing;
            };
            vm.PluginSelectionUpdated += OnPluginSelectionUpdated;

            var form = new NSStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxYMargin |
                                   NSViewResizingMask.HeightSizable,
                Orientation = NSUserInterfaceLayoutOrientation.Vertical
            };
            View.AddSubview(form);
            QCon.Center(form, View);
            QCon.Width(form, 280);

            var unameHolder = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable
            };
            form.AddView(unameHolder, NSStackViewGravity.Top);
            QCon.Con(unameHolder, form, NSLayoutAttribute.Left);
            QCon.Con(unameHolder, form, NSLayoutAttribute.Right);
            QCon.Size(unameHolder, 280, 36);

            usernameMutable = new NSMutableAttributedString("Skype name, email or phone", new CTStringAttributes
            {
                ForegroundColor = Colorizer.C.LoginPlaceholder.CGColor
            });
            
            usernameBox = new NSTextField
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable,
                Bordered = false,
                DrawsBackground = false,
                Font = NSFont.FromFontName("SegoeUI-Light", 16),
                Cell =
                {
                    PlaceholderAttributedString = usernameMutable
                },
                TextColor = NSColor.White
            };
            usernameBox.Changed += Typing;
            usernameBox.Activated += OnUsernameEnter;
            unameHolder.AddSubview(usernameBox);
            QCon.CenterY(usernameBox, unameHolder);
            QCon.Con(usernameBox, unameHolder, NSLayoutAttribute.Left);
            QCon.Con(usernameBox, unameHolder, NSLayoutAttribute.Right);
            
            var unameLine = new ColoredLineView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable | NSViewResizingMask.MaxYMargin,
                StrokeColor = Colorizer.C.LoginFormLine
            };
            unameHolder.AddSubview(unameLine);
            QCon.Con(unameLine, unameHolder, NSLayoutAttribute.Left);
            QCon.Con(unameLine, unameHolder, NSLayoutAttribute.Right);
            QCon.Con(unameLine, unameHolder, NSLayoutAttribute.Bottom);
            QCon.Height(unameLine, 1);
            
            var passHolder = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable
            };
            form.AddView(passHolder, NSStackViewGravity.Center);
            QCon.Con(passHolder, form, NSLayoutAttribute.Left);
            QCon.Con(passHolder, form, NSLayoutAttribute.Right);
            QCon.Size(passHolder, 280, 36);

            passwordMutable = new NSMutableAttributedString("Password", new CTStringAttributes
            {
                ForegroundColor = Colorizer.C.LoginPlaceholder.CGColor
            });

            passwordBox = new NSTextField
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable,
                Bordered = false,
                DrawsBackground = false,
                Font = NSFont.FromFontName("SegoeUI-Light", 16),
                Cell =
                {
                    PlaceholderAttributedString = passwordMutable
                },
                TextColor = NSColor.White
            };
            passwordBox.Changed += Typing;
            passwordBox.Activated += OnPasswordEnter;
            passHolder.AddSubview(passwordBox);
            QCon.CenterY(passwordBox, passHolder);
            QCon.Con(passwordBox, passHolder, NSLayoutAttribute.Left);
            QCon.Con(passwordBox, passHolder, NSLayoutAttribute.Right);
            
            var passLine = new ColoredLineView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin |
                                   NSViewResizingMask.WidthSizable | NSViewResizingMask.MaxYMargin,
                StrokeColor = Colorizer.C.LoginFormLine
            };
            passHolder.AddSubview(passLine);
            QCon.Con(passLine, passHolder, NSLayoutAttribute.Left);
            QCon.Con(passLine, passHolder, NSLayoutAttribute.Right);
            QCon.Con(passLine, passHolder, NSLayoutAttribute.Bottom);
            QCon.Height(passLine, 1);

            loginMutable = new NSMutableAttributedString("Sign in", new CTStringAttributes
            {
                ForegroundColor = NSColor.Gray.CGColor
            });
            loginMutable.SetAlignment(NSTextAlignment.Center, new NSRange(0, loginMutable.Value.Length));

            loginButton = new NSButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Bordered = false,
                ImagePosition = NSCellImagePosition.ImageOverlaps,
                ImageScaling = NSImageScale.ProportionallyUpOrDown,
                AttributedTitle = loginMutable,
                Cell =
                {
                    BackgroundColor = NSColor.FromRgba(0, 0, 0, 0)
                }
            };
            loginButton.Activated += OnLogin;
            form.AddView(loginButton, NSStackViewGravity.Bottom);
            QCon.CenterX(loginButton, form);
            QCon.Size(loginButton, 130, 35);

            // TODO default to Spycord QR if found
            protocolPopup.SelectItem(7);
            vm.SelectedListing = vm.PluginItems[7];
            vm.OpenMainWindow += () => OnOpenMainWindow?.Invoke();
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
            OnProtocolChanged(null);
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

        void OnProtocolChanged(NSObject sender)
        {
            protocolLabel.StringValue = protocolPopup.SelectedItem.Title;
            foreach (var pl in vm.PluginItems)
            {
                if (pl.InternalName == protocolPopup.SelectedItem.Identifier &&
                    (int)pl.AuthenticationType == protocolPopup.SelectedItem.Tag)
                    selectedListing = pl;
            }
            vm.SelectedListing = selectedListing;
        }
    }
    
    #endregion
}