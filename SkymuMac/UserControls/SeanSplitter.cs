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


// TODO: Hitbox n shit

using AppKit;
using CoreGraphics;
using Foundation;
using System;

namespace Skymu.UserControls
{
    [Register("SeanSplitter")]
    public class SeanSplitter : NSSplitView
    {
        public SeanSplitter() : base()
            => Construct();
        public SeanSplitter(IntPtr handle) : base(handle)
            => Construct();
        [Export("initWithCoder:")]
        public SeanSplitter(NSCoder coder) : base(coder)
            => Construct();
        
        void Construct()
        {
            DividerStyle = NSSplitViewDividerStyle.Thin;
        }

        public override void DrawDivider(CGRect rect)
        {
            var path = new NSBezierPath();
            if (IsVertical)
            {
                path.MoveTo(new CGPoint(rect.GetMidX(), 0));
                path.LineTo(new CGPoint(rect.GetMidX(), rect.Height));
            }
            else
            {
                path.MoveTo(new CGPoint(0, rect.GetMidY()));
                path.LineTo(new CGPoint(rect.Width, rect.GetMidY()));
            }
            path.LineWidth = 1;
            Colorizer.C.SidebarSplitter.SetStroke();
            path.Stroke();
        }
    }
}

