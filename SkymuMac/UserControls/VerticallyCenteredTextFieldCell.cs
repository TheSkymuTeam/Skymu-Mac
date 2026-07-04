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
using Foundation;
using AppKit;
using CoreGraphics;

// Source - https://stackoverflow.com/a/33788973
// Posted by Erik, modified by community. See post 'Timeline' for change history
// Retrieved 2026-06-28, License - CC BY-SA 3.0
// fuck the top answer on the post guy becuase they didn't even read the tags and the answer sucks ass

namespace Skymu.UserControls
{
	public partial class VerticallyCenteredTextFieldCell : NSTextFieldCell
	{
        public VerticallyCenteredTextFieldCell () : base() { }
		public VerticallyCenteredTextFieldCell (IntPtr handle) : base(handle) { }
        [Export ("initWithCoder:")]
		public VerticallyCenteredTextFieldCell (NSCoder coder) : base(coder) { }

        public override CGRect TitleRectForBounds(CGRect frame)
        {
            var stringHeight = this.AttributedStringValue.Size.Height;
            var titleRect = this.TitleRectForBounds(frame);
            var oldY = frame.Y;
            titleRect.Location = new CGPoint(titleRect.X, frame.Y + (frame.Height - stringHeight) / 2.0);
            titleRect.Height -= (titleRect.Y - oldY);
            return titleRect;
        }

        public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
            => base.DrawInteriorWithFrame(TitleRectForBounds(cellFrame), inView);
    }
}
