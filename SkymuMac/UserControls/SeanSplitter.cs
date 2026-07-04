using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace Skymu.UserControls
{
    [Register("SeanSplitter")]
    public class SeanSplitter : NSSplitView
	{
        public SeanSplitter() : base() { }
        public SeanSplitter(IntPtr handle) : base(handle) { }
        [Export("initWithCoder:")]
        public SeanSplitter(NSCoder coder) : base(coder) { }

        public override nfloat DividerThickness => 15;

        public override void DrawDivider(CGRect rect)
        {
            base.DrawDivider(rect);

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

