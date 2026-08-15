using AppKit;

namespace MSUI.Helper
{
    public class Holder
    {
        public NSView V { get; set; }
    }

    public static class GHolder
    {
        public static Holder H = new Holder();
    }

    public static class HelperForHolder
    {
        public static T Hold<T>(this T view, Holder holder) where T : NSView
        {
            holder.V = view;
            return view;
        }
    }
}