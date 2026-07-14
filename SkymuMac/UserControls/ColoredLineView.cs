/*==========================================================*/
// This file is licensed under MIT, separately from the
// rest of the project.
/*==========================================================*/
// Copyright 2026 The Skymu Team
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated
// documentation files (the “Software”), to deal in the
// Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice
// shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/*==========================================================*/

using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace Skymu.UserControls
{
    [Register("ColoredLineView")]
	public class ColoredLineView : NSView
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
        
        public ColoredLineView() : base() { }
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
