// OnXXX and other stuff in the Fluent style.

using AppKit;
using System;

namespace MSUI.Helper
{
    public static class FluentBind
    {
        public static T OnActivated<T>(this T control, EventHandler action) where T : NSControl
        {
            control.Activated += action;
            return control;
        }

        public static T OnChanged<T>(this T field, EventHandler changed) where T : NSTextField
        {
            field.Changed += changed;
            return field;
        }
    }
}