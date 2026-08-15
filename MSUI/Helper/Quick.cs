using AppKit;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace MSUI.Helper.Quick
{
    public static class QCon
    {
        public static T CenterX<T>(this T what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1, int constant = 0) where T : NSView
        {
            to.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.CenterX, relation, to, NSLayoutAttribute.CenterX, multiplier, constant));
            return what;
        }
        public static T CenterY<T>(this T what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1, int constant = 0) where T : NSView
        {
            to.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.CenterY, relation, to,NSLayoutAttribute.CenterY, multiplier, constant));
            return what;
        }
        public static T Center<T>(this T what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1, int constant = 0) where T : NSView
        {
            return what
                .CenterX(to, relation, multiplier, constant)
                .CenterY(to, relation, multiplier, constant);
        }
        
        public static T Width<T>(this T what, int size, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            what.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.Width, relation, multiplier,
                size));
            return what;
        }

        public static T Height<T>(this T what, int size, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            what.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.Height, relation, multiplier,
                size));
            return what;
        }

        public static T Size<T>(this T what, int width, int height, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            return what
                .Width(width, relation, multiplier)
                .Height(height, relation, multiplier);
        }
        
        public static T Con<T>(this T what, NSView super, NSView to, NSLayoutAttribute attrw, NSLayoutAttribute attrt, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            super.AddConstraint(NSLayoutConstraint.Create(what, attrw, relation, to, attrt, multiplier, constant));
            return what;
        }
        /// <param name="attrw">attrt = reversed of this</param>
        public static T Con<T>(this T what, NSView super, NSView to, NSLayoutAttribute attrw, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            super.AddConstraint(NSLayoutConstraint.Create(what, attrw, relation, to, attrw.FlipAttr(), multiplier, constant));
            return what;
        }

        public static T Con<T>(this T what, NSView to, NSLayoutAttribute attr, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            to.AddConstraint(NSLayoutConstraint.Create(what, attr, relation, to, attr, multiplier, constant));
            return what;
        }

        public static T CHorizontal<T>(this T what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            return what
                .Con(to, NSLayoutAttribute.Left, constant, relation, multiplier)
                .Con(to, NSLayoutAttribute.Right, constant, relation, multiplier);
        }
        public static T CVertical<T>(this T what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            return what
                .Con(to, NSLayoutAttribute.Top, constant, relation, multiplier)
                .Con(to, NSLayoutAttribute.Bottom, constant, relation, multiplier);
        }
        
        public static T CAll<T>(this T what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int multiplier = 1) where T : NSView
        {
            return what
                .CHorizontal(to, constant, relation, multiplier)
                .CVertical(to, constant, relation, multiplier);
        }
    }

    public static class QHug
    {
        public static T HHorizontal<T>(this T what, int hug = 1000) where T : NSView
        {
            what.SetContentHuggingPriorityForOrientation(hug, NSLayoutConstraintOrientation.Horizontal);
            return what;
        }

        public static T HVertical<T>(this T what, int hug = 1000) where T : NSView
        {
            what.SetContentHuggingPriorityForOrientation(hug, NSLayoutConstraintOrientation.Vertical);
            return what;
        }
        public static T HAll<T>(this T what, int hug = 1000) where T : NSView
        {
            return what
                .HHorizontal(hug)
                .HVertical(hug);
        }
    }
    
    public static class QComp
    {
        public static T mpHorizontal<T>(this T what, int resist = 1000) where T : NSView
        {
            what.SetContentCompressionResistancePriority(resist, NSLayoutConstraintOrientation.Horizontal);
            return what;
        }
        public static T mpVertical<T>(this T what, int resist = 1000) where T : NSView
        {
            what.SetContentCompressionResistancePriority(resist, NSLayoutConstraintOrientation.Vertical);
            return what;
        }
        public static T mpAll<T>(this T what, int resist = 1000) where T : NSView
        {
            return what
                .mpHorizontal(resist)
                .mpVertical(resist);
        }
    }
}