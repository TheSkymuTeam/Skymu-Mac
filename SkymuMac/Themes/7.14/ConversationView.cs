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
using Skymu.Helpers;
using Skymu.Quick;
using Skymu.UserControls;
using Skymu.ViewModels;

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
            };
            View.AddSubview(profile);
            QCon.CAll(profile, View);

            avatar = new NSImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = ImageHelper.Circle(Universal.ContactAvatar)
            };
            profile.AddSubview(avatar);
            QCon.Size(avatar, 70, 70);
            QCon.Con(avatar, profile, NSLayoutAttribute.Top);
            QCon.Con(avatar, profile, NSLayoutAttribute.Left);

            cvTitle = new Label("Skymu Internal")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = NSFont.SystemFontOfSize(15)
            };
            profile.AddSubview(cvTitle);
            QCon.Con(cvTitle, profile, NSLayoutAttribute.Top);
            QCon.Con(profile, cvTitle, avatar, NSLayoutAttribute.Left, NSLayoutAttribute.Right);
            QCon.Con(cvTitle, profile, NSLayoutAttribute.Right);
            QCon.Width(cvTitle, 15, NSLayoutRelation.GreaterThanOrEqual);
        }

        public void SetConversation()
        {
            cvTitle.StringValue = vm.SelectedConversation.DisplayName;
            avatar.Image = ImageHelper.Circle(ImageHelper.GetAvatar(vm.SelectedConversation.Avatar, "contact"));
        }
    }
}