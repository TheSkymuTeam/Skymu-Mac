using System;
using AppKit;
using Foundation;
using CoreGraphics;

/*
namespace MacSplitView
{
    [Register("MySplitViewController")]
    public class MySplitViewController : NSSplitViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // 1. Create the Split View
            SplitView = new NSSplitView(new CGRect(0, 0, 800, 600))
            {
                Vertical = true, // Set false for a horizontal split
                DividerStyle = NSSplitViewDividerStyle.Thin
            };

            // 2. Create the first pane (e.g., Sidebar)
            var leftView = new NSView(new CGRect(0, 0, 200, 600)) { WantsLayer = true };
            leftView.Layer.BackgroundColor = NSColor.SystemBlueColor.CGColor;
            var leftItem = NSSplitViewItem.FromViewController(new NSViewController(), leftView);
            leftItem.CanCollapse = true;

            // 3. Create the second pane (e.g., Main Content)
            var rightView = new NSView(new CGRect(205, 0, 595, 600)) { WantsLayer = true };
            rightView.Layer.BackgroundColor = NSColor.SystemGrayColor.CGColor;
            var rightItem = NSSplitViewItem.FromViewController(new NSViewController(), rightView);
            
            // 4. Add items to Split View Controller
            AddSplitViewItem(leftItem);
            AddSplitViewItem(rightItem);
            
            View = SplitView;
        }
    }
}*/