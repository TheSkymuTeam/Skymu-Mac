using System;
using Foundation;

using AppKit;
using CoreGraphics;

// Text button that looks like text normally, but an inline on hover

// TODO. Not complete.

namespace Skymu.UserControls
{
    [Register("SkyTextButton")]
    public class SkyTextButton : NSButton
    {
        NSColor bgColor = NSColor.FromRgb('\xAC', '\xAC', '\xAC');
        NSTrackingArea trackingArea;
        bool isHovering;
            
        public SkyTextButton() : base()
            => Construct();
        public SkyTextButton(IntPtr handle) : base(handle)
            => Construct();
        [Export("initWithCoder:")]
        public SkyTextButton(NSCoder coder) : base(coder)
            => Construct();

        void Construct()
        {
            BezelStyle = NSBezelStyle.Rounded;
            Bordered = false;
            Cell.Bordered = false;
            Cell.BackgroundColor = NSColor.Clear;
            Cell.HighlightsBy = (int)NSCellStyleMask.NoCell;
        }

        public override void ViewDidMoveToWindow()
        {
            base.ViewDidMoveToWindow();
            UpdateTrackingArea();
        }

        public override void UpdateTrackingAreas()
        {
            base.UpdateTrackingAreas();
            UpdateTrackingArea();
        }

        void UpdateTrackingArea()
        {
            if (trackingArea != null)
                RemoveTrackingArea(trackingArea);

            var options = NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveInKeyWindow;
            trackingArea = new NSTrackingArea(Bounds, options, this, null);
            AddTrackingArea(trackingArea);
        }

        public override void MouseEntered(NSEvent theEvent)
        {
            isHovering = true;
            UpdateColors();
        }

        public override void MouseExited(NSEvent theEvent)
        {
            isHovering = false;
            UpdateColors();
        }

        void UpdateColors()
        {
            var textColor = NSColor.Text.UsingColorSpace(NSColorSpace.DeviceRGB);
            Cell.AttributedTitle = new NSAttributedString(Cell.Title, new NSStringAttributes {
                ForegroundColor = isHovering
                    ? NSColor.FromDeviceRgba(
                        1 - textColor.RedComponent,
                        1 - textColor.GreenComponent,
                        1 - textColor.BlueComponent,
                        1.0f
                    ) : NSColor.ControlText
            });
            Cell.BackgroundColor = isHovering
            ? bgColor
            : NSColor.Clear;
            NeedsDisplay = true;
        }

        public override void DrawRect(CGRect dirtyRect)
        {
            if (isHovering)
            {
                var inset = Bounds;//.Inset(1, 1);
                nfloat radius = inset.Height / 2;
                var path = NSBezierPath.FromRoundedRect(inset, radius, radius);
                path.AddClip();
                bgColor.SetFill();
                path.Fill();
            }

            base.DrawRect(dirtyRect);
        }
    }
}
