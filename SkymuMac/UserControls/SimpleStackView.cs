/*==========================================================*/
// This file is licensed under MIT, separately from the
// rest of the project.
/*==========================================================*/
// Copyright 2026 The Skymu Team
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
using CoreGraphics;
using Foundation;

namespace Skymu.UserControls
{
    public class SimpleStackView : NSView
{
	private readonly NSMutableArray arrangedSubviews = new NSMutableArray();
	public NSLayoutAttribute Alignment { get; set; } = NSLayoutAttribute.Top;
	public nfloat Spacing { get; set; } = 8;
	public NSUserInterfaceLayoutOrientation Orientation { get; set; } = NSUserInterfaceLayoutOrientation.Vertical;

	public void AddArrangedSubview(NSView view)
	{
		arrangedSubviews.Add(view);
		AddSubview(view);
		InvalidateIntrinsicContentSize();
		NeedsLayout = true;
	}

	public override CGSize IntrinsicContentSize
	{
		get
		{
			if (arrangedSubviews.Count == 0)
				return new CGSize(NSView.NoIntrinsicMetric, NSView.NoIntrinsicMetric);

			nfloat totalWidth = 0, totalHeight = 0;
			double maxWidth = 0, maxHeight = 0;

			for (nuint i = 0; i < arrangedSubviews.Count; i++)
			{
				var view = arrangedSubviews.GetItem<NSView>(i);
				var size = view.IntrinsicContentSize;

				if (Orientation == NSUserInterfaceLayoutOrientation.Horizontal)
				{
					if (size.Width > 0 && size.Width != NSView.NoIntrinsicMetric)
						totalWidth += size.Width;
					if (size.Height > 0 && size.Height != NSView.NoIntrinsicMetric)
						maxHeight = Math.Max(maxHeight, size.Height);
					else
						maxHeight = Math.Max(maxHeight, 22); // fallback
				}
				else
				{
					if (size.Width > 0 && size.Width != NSView.NoIntrinsicMetric)
						maxWidth = Math.Max(maxWidth, size.Width);
					else
						maxWidth = Math.Max(maxWidth, 80); // fallback
					if (size.Height > 0 && size.Height != NSView.NoIntrinsicMetric)
						totalHeight += size.Height;
				}
			}

			if (Orientation == NSUserInterfaceLayoutOrientation.Horizontal)
			{
				if (totalWidth > 0)
					totalWidth += Spacing * (arrangedSubviews.Count - 1);
				return new CGSize(totalWidth, maxHeight);
			}
			else
			{
				if (totalHeight > 0)
					totalHeight += Spacing * (arrangedSubviews.Count - 1);
				return new CGSize(maxWidth, totalHeight);
			}
		}
	}

	public override void Layout()
	{
		base.Layout();
		LayoutArrangedSubviews();
	}

	private void LayoutArrangedSubviews()
	{
		if (arrangedSubviews.Count == 0) return;

		if (Orientation == NSUserInterfaceLayoutOrientation.Vertical)
			LayoutVertical();
		else
			LayoutHorizontal();
	}

	private void LayoutVertical()
	{
		nfloat yPos = Bounds.Height;

		for (nuint i = 0; i < arrangedSubviews.Count; i++)
		{
			var view = arrangedSubviews.GetItem<NSView>(i);
			var size = view.IntrinsicContentSize;

			yPos -= size.Height;
			view.Frame = new CGRect(0, yPos, Bounds.Width, size.Height);
			yPos -= Spacing;
		}
	}

	private void LayoutHorizontal()
	{
		nfloat xPos = 0;

		for (nuint i = 0; i < arrangedSubviews.Count; i++)
		{
			var view = arrangedSubviews.GetItem<NSView>(i);
			var size = view.IntrinsicContentSize;

			view.Frame = new CGRect(xPos, 0, size.Width, Bounds.Height);
			xPos += size.Width + Spacing;
		}
	}
}
}