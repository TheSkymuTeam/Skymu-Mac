using AppKit;

namespace MSUI.QuickView
{
    public class Label : NSTextField
    {
        public Label(string text)
            => Construct(text);

        void Construct(string text)
        {
            StringValue = text;
            DrawsBackground = false;
            Bordered = false;
            Editable = false;
        }
    }
}