/*==========================================================*/
// This file is licensed under CC BY-SA 3.0
//     <http://creativecommons.org/licenses/by-sa/3.0/>
// separately from rest of the project.
// I don't think I made any modification besides the C#
// rewrite.
// 2026, TheSkymuTeam. Originally by Erik, modified by the
// StackOverflow community: https://stackoverflow.com/a/33788973
// 
/*==========================================================*/

using System;
using Foundation;
using AppKit;
using CoreGraphics;

// fuck the top answer on the post because they didn't even read the tags and the answer sucks ass

namespace Skymu.UserControls
{
	[Register("VerticallyCenteredTextFieldCell")]
	public class VerticallyCenteredTextFieldCell : NSTextFieldCell
	{
        public VerticallyCenteredTextFieldCell () : base() { }
		public VerticallyCenteredTextFieldCell (IntPtr handle) : base(handle) { }
        [Export ("initWithCoder:")]
		public VerticallyCenteredTextFieldCell (NSCoder coder) : base(coder) { }

        public override CGRect TitleRectForBounds(CGRect frame)
        {
            var stringHeight = this.AttributedStringValue.Size.Height;
            var titleRect = base.TitleRectForBounds(frame);
            var oldY = frame.Y;
            titleRect.Location = new CGPoint(titleRect.X, frame.Y + (frame.Height - stringHeight) / 2.0);
            titleRect.Height -= (titleRect.Y - oldY);
            return titleRect;
        }

        public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
            => base.DrawInteriorWithFrame(TitleRectForBounds(cellFrame), inView);
    }
}
