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
using MSUI.Helper;
using MSUI.Helper.Quick;
using MSUI.QuickView;
using Skymu.Helpers;
using Skymu.UserControls;
using Skymu.ViewModels;
using Yggdrasil.Models;

// ReSharper disable once CheckNamespace
namespace Skymu.Themes.S714
{
    public class ConversationViewController : NSViewController
    {
        readonly MainViewModel vm;

        Label cvTitle;
        NSImageView avatar;

        public ConversationViewController(MainViewModel vm) : base()
        {
            this.vm = vm;
        }

        public override void LoadView()
        {
            View = new SeanSplitter
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizesSubviews = true,
                IsVertical = false
            };

            var profile = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizesSubviews = true
            }
                .AddTo(View)
                .CAll(View);

            avatar = new NSImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Universal.ContactAvatar,
                WantsLayer = true,
                Layer =
                {
                    CornerRadius = 70 / 2
                }
            }
                .AddTo(profile)
                .Size(70, 70)
                .Con(profile, NSLayoutAttribute.Top, 10)
                .Con(profile, NSLayoutAttribute.Left, 10);

            cvTitle = new Label("Skymu Internal")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = NSFont.SystemFontOfSize(15)
            }
                .AddTo(profile)
                .Con(profile, NSLayoutAttribute.Top, 10)
                .Con(profile, avatar, NSLayoutAttribute.Left, 10)
                .Con(profile, NSLayoutAttribute.Right)
                .Width(15, NSLayoutRelation.GreaterThanOrEqual);
        }

        public void SetConversation()
        {
            cvTitle.StringValue = vm.SelectedConversation.DisplayName;
            avatar.Image = ImageHelper.GetAvatar(vm.SelectedConversation.Avatar, vm.SelectedConversation is DirectMessage ? "contact" : "group");
        }
    }
}