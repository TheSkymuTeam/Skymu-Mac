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

using Skymu.Preferences;
using Skymu.ViewModels;
using System;
using System.Threading.Tasks;
using Yggdrasil.Enumerations;

using AppKit;
using Foundation;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
	public partial class LoginWindow : NSWindow 
	{
		public LoginWindow(IntPtr handle) : base(handle)
			=> Construct();
		[Export("initWithCoder:")]
		public LoginWindow(NSCoder coder) : base(coder)
			=> Construct();

        LoginViewModel.PluginListing selectedListing;
        LoginViewModel vm;
        
        public nint SelectionIndex {
            get => (nint)ProtocolSelector.IndexOfSelectedItem;
            set => ProtocolSelector.SelectItem(ProtocolSelector.Items()[value]);
        }

        void Construct()
		{
			Title = Settings.BrandingName;
            vm = new LoginViewModel();
        }

        public override async void AwakeFromNib()
		{
            base.AwakeFromNib();
            
            ContentView = LoadingView;
            ContentView.Layer.BackgroundColor = Colorizer.C.LoginBackground.CGColor;

            Throbber.Animates = true;
            Throbber.CanDrawSubviewsIntoLayer = true;
            Throbber.ImageScaling = NSImageScale.None;

            bool fail = false;
            var task = new Task(() =>
            {
                vm.PluginSelectionUpdated += OnPluginSelectionUpdated;
                vm.LoadPlugins();
                if (vm.PluginItems == null || vm.PluginItems.Count <= 0)
                {
                    fail = true;
                    InvokeOnMainThread(() =>
                    {
                        new NSAlert()
                        {
                            MessageText = "No plugins detected. Unable to continue."
                        }.RunModal();
                        NSRunningApplication.CurrentApplication.Terminate();
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        ProtocolSelector.RemoveAllItems();
                        foreach (var p in vm.PluginItems)
                        {
                            ProtocolSelector.AddItem(p.DisplayName);
                            var item = ProtocolSelector.Items()[ProtocolSelector.ItemCount - 1];
                            item.Identifier = p.InternalName;
                            item.Tag = (int) p.AuthenticationType;
                        }

                        ProtocolSelector.SelectItem(0);
                    });
                }
            });
            task.Start();
            
            await Task.Delay(1000);
            if (!task.IsCompleted)
                await task; // in case a plugin is taking ages to load, modal is showing, ...
            if (task.IsFaulted)
            {
                Universal.ExceptionHandler(task.Exception);
                NSRunningApplication.CurrentApplication.Terminate();
            }
            if (fail)
                return;
            
            LoginView.WantsLayer = true;
            LoginView.Layer.BackgroundColor = Colorizer.C.LoginBackground.CGColor;
            UsernameBox.PlaceholderAttributedString = new NSAttributedString(UsernameBox.PlaceholderString, new NSStringAttributes
            {
                ForegroundColor = Colorizer.C.LoginPlaceholder
            });
            PasswordBox.PlaceholderAttributedString = new NSAttributedString(PasswordBox.PlaceholderString, new NSStringAttributes
            {
                ForegroundColor = Colorizer.C.LoginPlaceholder
            });
            LoginButton.AttributedTitle = new NSAttributedString(LoginButton.Title, new NSStringAttributes
            {
                ForegroundColor = NSColor.White
            });
            UsernameBox.Changed += Typing;
            PasswordBox.Changed += Typing;
            vm.OpenMainWindow += OpenMainWindow;
            vm.SelectedListing = vm.PluginItems[0];
            ContentView = LoginView;
        }

        void Typing(object sender, EventArgs e) =>
            CheckEnableLoginButton();

        void OnPluginSelectionUpdated(LoginViewModel.PluginListing listing)
        {
            selectedListing = listing;

            InvokeOnMainThread(() =>
            {
                PasswordBox.Enabled = true;
                PasswordBox.PlaceholderString =
                    listing.TextPassword ?? "Password";
                LoginButton.Title = "Login";

                UsernameBox.Enabled = true;
                UsernameBox.PlaceholderString = listing.TextUsername ?? Universal.Lang["790.placeholderString"]; // Username

                if (listing.AuthenticationType != AuthenticationMethod.Password)
                {
                    PasswordBox.Enabled = false;
                    UsernameBox.StringValue = "";
                    PasswordBox.PlaceholderString = "field not required";

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (listing.AuthenticationType)
                    {
                        case AuthenticationMethod.QRCode:
                            LoginButton.Title = "Scan QR code";
                            UsernameBox.Enabled = false;
                            UsernameBox.StringValue = "";
                            UsernameBox.PlaceholderString = "field not required";
                            break;
                        case AuthenticationMethod.Passwordless:
                            LoginButton.Title = "Send code";
                            break;
                        case AuthenticationMethod.External:
                            LoginButton.Title = "External login";
                            break;
                        default:
                            LoginButton.Title = "Login";
                            break;
                    }
                }

                CheckEnableLoginButton();
                OnProtocolChanged(null);
            });
        }
        
        void CheckEnableLoginButton()
        {
            LoginButton.Enabled = 
                !string.IsNullOrWhiteSpace(UsernameBox.StringValue)
                && (!PasswordBox.Enabled || !string.IsNullOrWhiteSpace(UsernameBox.StringValue))
                || !PasswordBox.Enabled && !UsernameBox.Enabled;
            LoginButton.AttributedTitle = new NSAttributedString(LoginButton.Title, new NSStringAttributes
            {
                ForegroundColor = LoginButton.Enabled ? NSColor.White : NSColor.Black
            });
        }

        partial void OnUsernameEnter(NSObject sender)
        {
            if (PasswordBox.Enabled && string.IsNullOrWhiteSpace(PasswordBox.StringValue))
                PasswordBox.SelectText(null);
            else if (!string.IsNullOrWhiteSpace(UsernameBox.StringValue))
                OnLogin(null);
        }

        partial void OnPasswordEnter(NSObject sender)
        {
            if (UsernameBox.Enabled && string.IsNullOrWhiteSpace(UsernameBox.StringValue))
                UsernameBox.SelectText(null);
            else if (!string.IsNullOrWhiteSpace(PasswordBox.StringValue))
                OnLogin(null);
        }

        async partial void OnLogin(NSObject sender)
            => await vm.Login(UsernameBox.StringValue, PasswordBox.StringValue);

        partial void OnProtocolChanged(NSObject sender)
        {
            ProtocolText.StringValue = ProtocolSelector.SelectedItem.Title;
            foreach (var pl in vm.PluginItems)
            {
                if (pl.InternalName == ProtocolSelector.SelectedItem.Identifier &&
                    (int) pl.AuthenticationType == ProtocolSelector.SelectedItem.Tag)
                    selectedListing = pl;
            }
            vm.SelectedListing = selectedListing;
        }

        void OpenMainWindow()
        {
            var mwc = new MainWindowController();
            ((AppDelegate)NSApplication.SharedApplication.Delegate).activeWindow = mwc;
            this.Close();
            vm.RunPostLogin(mwc.Window.vm);
        }

        public override void PerformClose(NSObject sender)
        {
            base.PerformClose(sender);
            NSRunningApplication.CurrentApplication.Terminate();
        }
    }
}
