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

using AppKit;
using CoreGraphics;
using Foundation;

namespace Skymu.UserControls
{
	public partial class ColoredLineView : NSView
	{
        NSColor strokeColor = NSColor.Control;
        [Export("StrokeColor")]
        public NSColor StrokeColor
        {
            get { return strokeColor; }
            set
            {
                WillChangeValue("StrokeColor");
                strokeColor = value;
                DidChangeValue("StrokeColor");
            }
        }

        public ColoredLineView(IntPtr handle) : base(handle) { }
        [Export("initWithCoder:")]
        public ColoredLineView(NSCoder coder) : base(coder) { }

        public override void DrawRect(CGRect dirtyRect)
        {
            base.DrawRect(dirtyRect);
            var path = new NSBezierPath();
            if (Frame.Height > Frame.Width)
            {
                path.MoveTo(new CGPoint(Bounds.GetMidX(), 0));
                path.LineTo(new CGPoint(Bounds.GetMidX(), Bounds.Height));
                path.LineWidth = Bounds.Width;
            }
            else
            {
                path.MoveTo(new CGPoint(0, Bounds.GetMidY()));
                path.LineTo(new CGPoint(Bounds.Width, Bounds.GetMidY()));
                path.LineWidth = Bounds.Height;
            }
            strokeColor.SetStroke();
            path.Stroke(); }
    }
}
