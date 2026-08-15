using AppKit;
using CoreAnimation;
using System;

namespace MSUI.Helper
{
    public static class TheGoodShit
    {
        public static T NoTAMIC<T>(this T view) where T : NSView
        {
            view.TranslatesAutoresizingMaskIntoConstraints = false;
            return view;
        }

        public static CALayer GetLayer(this NSView view)
        {
            view.WantsLayer = true;
            return view.Layer;
        }
        
        
        public static NSLayoutAttribute FlipAttr(this NSLayoutAttribute attr)
        {
            switch (attr)
            {
                case NSLayoutAttribute.Top:
                    return NSLayoutAttribute.Bottom;
                case NSLayoutAttribute.Bottom:
                    return NSLayoutAttribute.Top;
                case NSLayoutAttribute.Left:
                    return NSLayoutAttribute.Right;
                case NSLayoutAttribute.Right:
                    return NSLayoutAttribute.Left;
                case NSLayoutAttribute.Leading:
                    return NSLayoutAttribute.Trailing;
                case NSLayoutAttribute.Trailing:
                    return NSLayoutAttribute.Leading;
                case NSLayoutAttribute.Width:
                case NSLayoutAttribute.Height:
                case NSLayoutAttribute.CenterX:
                case NSLayoutAttribute.CenterY:
                    throw new InvalidOperationException("Center and size does not have an agreeable and reasonable way to flip. Invalid.");
                case NSLayoutAttribute.NoAttribute:
                    throw new InvalidOperationException("What do you mean flip an effectively null value?");
                default:
                    throw new NotImplementedException(attr.ToString());
            }
        }
    }
}