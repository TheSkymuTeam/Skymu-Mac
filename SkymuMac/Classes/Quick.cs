/*==========================================================*/
// This file is licensed under MIT, separately from the
// rest of the project.
/*==========================================================*/
// Copyright 2026 nilFinx
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated
// documentation files (the “Software”), to deal in the
// Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice
// shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/*==========================================================*/

using System;
using AppKit;
using Foundation;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Skymu.Quick
{
    public static class QCon
    {
        public static NSLayoutAttribute FlipAttr(NSLayoutAttribute attr)
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
                default:
                    throw new NotImplementedException(attr.ToString());
            }
        }
        
        public static void CenterX(NSView what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1, int constant = 0)
            => to.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.CenterX, relation, to, NSLayoutAttribute.CenterX, markiplier, constant));
        public static void CenterY(NSView what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1, int constant = 0)
            => to.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.CenterY, relation, to, NSLayoutAttribute.CenterY, markiplier, constant));
        public static void Center(NSView what, NSView to, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1, int constant = 0)
        {
            CenterX(what, to, relation, markiplier, constant);
            CenterY(what, to, relation, markiplier, constant);
        }
        
        public static void Width(NSView what, int size, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
            => what.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.Width, relation, markiplier, size));
        public static void Height(NSView what, int size, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
            => what.AddConstraint(NSLayoutConstraint.Create(what, NSLayoutAttribute.Height, relation, markiplier, size));
        public static void Size(NSView what, int width, int height, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
        {
            Width(what, width, relation, markiplier);
            Height(what, height, relation, markiplier);
        }
        
        public static void Con(NSView super, NSView what, NSView to, NSLayoutAttribute attrw, NSLayoutAttribute attrt, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
            => super.AddConstraint(NSLayoutConstraint.Create(what, attrw, relation, to, attrt, markiplier, constant));
        public static void Con(NSView what, NSView to, NSLayoutAttribute attr, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
            => to.AddConstraint(NSLayoutConstraint.Create(what, attr, relation, to, attr, markiplier, constant));
        public static void CHorizontal(NSView what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
        {
            Con(what, to, NSLayoutAttribute.Left, constant, relation, markiplier);
            Con(what, to, NSLayoutAttribute.Right, constant, relation, markiplier);
        }
        public static void CVertical(NSView what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
        {
            Con(what, to, NSLayoutAttribute.Top, constant, relation, markiplier);
            Con(what, to, NSLayoutAttribute.Bottom, constant, relation, markiplier);
        }
        
        public static void CAll(NSView what, NSView to, int constant = 0, NSLayoutRelation relation = NSLayoutRelation.Equal, int markiplier = 1)
        {
            CHorizontal(what, to, constant, relation, markiplier);
            CVertical(what, to, constant, relation, markiplier);
        }
    }

    public static class QHug
    {
        public static void H(NSView what, int hug = 1000)
            => what.SetContentHuggingPriorityForOrientation(hug, NSLayoutConstraintOrientation.Horizontal);
        public static void V(NSView what, int hug = 1000)
            => what.SetContentHuggingPriorityForOrientation(hug, NSLayoutConstraintOrientation.Vertical);
        public static void All(NSView what, int hug = 1000)
        {
            H(what, hug);
            V(what, hug);
        }
    }
    
    public static class QComp
    {
        public static void H(NSView what, int resist = 1000)
            => what.SetContentCompressionResistancePriority(resist, NSLayoutConstraintOrientation.Horizontal);
        public static void V(NSView what, int resist = 1000)
            => what.SetContentCompressionResistancePriority(resist, NSLayoutConstraintOrientation.Vertical);
        public static void All(NSView what, int resist = 1000)
        {
            H(what, resist);
            V(what, resist);
        }
    }

    public static class QImg
    {
        public static NSImage ThemedImage(string key, string extension = "png")
        {
            return new NSImage(NSBundle.MainBundle.PathForResource(
                key,
                extension,
                Universal.Theme)
            );
        }
    }
}