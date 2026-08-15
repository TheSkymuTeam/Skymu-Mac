using System;
using AppKit;

namespace MSUI.Helper
{
    public static class AddView
    {
        public static NSStackView Add(this NSStackView parent, NSView child, NSStackViewGravity gravity)
        {
            parent.AddView(child, gravity);
            return parent;
        }

        public static T Add<T>(this T parent, NSView child) where T : NSView
        {
            parent.AddSubview(child);
            return parent;
        }
        
        public static T AddAnd<T, T2>(this T parent, T2 child, Action<T2> then) where T : NSView where T2 : NSView
        {
            parent.AddSubview(child);
            then.Invoke(child);
            return parent;
        }

        /// <returns>The child (first argument or object being invoked to)</returns>
        public static T AddTo<T>(this T child, NSView parent) where T : NSView
        {
            parent.AddSubview(child);
            return child;
        }
    }
}