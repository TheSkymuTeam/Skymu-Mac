using System;
using AppKit;
using MSUI.Helper;
using MSUI.Helper.Quick;

namespace MSUI.Stack
{
    public class Stack : NSStackView
    {
        public new NSUserInterfaceLayoutOrientation Orientation
        {
            get => base.Orientation;
            private set => base.Orientation = value;
        }

        static NSView __(bool vert, params NSView[] rest)
        {
            if (rest.Length == 1)
                return rest[0];
            else if (rest.Length == 2)
                return new NSStackView
                    {
                        Orientation = vert.ToUILayoutOrientation()
                    }
                    .Add(rest[0], vert ? NSStackViewGravity.Top : NSStackViewGravity.Leading)
                    .Add(rest[1], vert ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
            else
                return new NSStackView
                    {
                        Orientation = vert.ToUILayoutOrientation()
                    }
                    .Add(rest[0], vert ? NSStackViewGravity.Top : NSStackViewGravity.Leading)
                    .Add(rest[1], NSStackViewGravity.Center)
                    .Add(__(vert, rest.Slice(2, rest.Length - 1)),
                        vert ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
        }
        static (NSView, Action<(NSView parent, NSView view)>) __(bool vert, params (NSView, Action<(NSView parent, NSView view)>)[] rest)
        {
            if (rest.Length == 1)
                return (rest[0].Item1, rest[0].Item2);
            else if (rest.Length == 2)
            {
               var sb = new NSStackView
                    {
                        Orientation = vert.ToUILayoutOrientation()
                    }
                    .Add(rest[0].Item1, vert ? NSStackViewGravity.Top : NSStackViewGravity.Leading)
                    .Add(rest[1].Item1, vert ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
               rest[0].Item2.Invoke((sb, rest[0].Item1));
               rest[1].Item2.Invoke((sb, rest[1].Item1));
               return (sb, null);
            }
            else
            {
                var sb = new NSStackView
                    {
                        Orientation = vert.ToUILayoutOrientation()
                    }
                    .Add(rest[0].Item1, vert ? NSStackViewGravity.Top : NSStackViewGravity.Leading)
                    .Add(rest[1].Item1, NSStackViewGravity.Center);
                rest[0].Item2.Invoke((sb, rest[0].Item1));
                rest[1].Item2.Invoke((sb, rest[1].Item1));
                var shit = __(vert, rest.Slice(2, rest.Length - 1));
                sb.Add(shit.Item1, vert ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
                return (sb, shit.Item2);
            }
        }
        
        public Stack(bool vertical, NSView a, NSView b, params NSView[] rest)
        {
            Orientation = vertical
                ? NSUserInterfaceLayoutOrientation.Vertical
                : NSUserInterfaceLayoutOrientation.Horizontal;
            _(vertical, a, b, rest);
        }
        void _(bool vertical, NSView a, NSView b, params NSView[] rest)
        {
            AddView(a, vertical ? NSStackViewGravity.Top : NSStackViewGravity.Leading);
            if (rest.Length == 0)
                this.Add(b, vertical ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
            else
            {
                this.Add(b, NSStackViewGravity.Center)
                    .Add(__(vertical, rest), vertical ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
            }
        }
        
        public Stack(bool vertical, NSView a, Action<(NSView parent, NSView view)> aa, NSView b, Action<(NSView parent, NSView view)> bb, params (NSView, Action<(NSView parent, NSView view)>)[] rest)
        {
            Orientation = vertical
                ? NSUserInterfaceLayoutOrientation.Vertical
                : NSUserInterfaceLayoutOrientation.Horizontal;
            _(vertical, a, aa, b, bb, rest);
        }
        void _(bool vertical, NSView a, Action<(NSView parent, NSView view)> aa, NSView b, Action<(NSView parent, NSView view)> bb, params (NSView, Action<(NSView parent, NSView view)>)[] rest)
        {
            AddView(a, vertical ? NSStackViewGravity.Top : NSStackViewGravity.Leading);
            aa.Invoke((this, a));
            if (rest.Length == 0)
            {
                this.Add(b, vertical ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
                bb.Invoke((this, b));
            }
            else
            {
                this.Add(b, NSStackViewGravity.Center);
                bb.Invoke((this, b));
                var shit = __(vertical, rest);
                this.Add(shit.Item1, vertical ? NSStackViewGravity.Bottom : NSStackViewGravity.Trailing);
                shit.Item2?.Invoke((this, shit.Item1));
            }
        }
    }
    
    
    public class SpacedStack
    {
        public SpacedStack(NSLayoutAttribute lead, NSView parent, params (int spacing, NSView view)[] views)
        {
            var previous = parent;
            foreach (var (spacing, view) in views)
            {
                parent.AddSubview(view);
                view.Con(parent, previous, lead, previous.Equals(parent) ? lead : lead.FlipAttr(), spacing);
                previous = view;
            }
        }
    }
}