using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Skymu.Forms
{
	public partial class ErrorWindow : AppKit.NSView
	{
		public ErrorWindow (IntPtr handle) : base (handle) { }
		[Export ("initWithCoder:")]
		public ErrorWindow (NSCoder coder) : base (coder) { }

		public void SetError(Exception ex)
			=> SetError(ex.Message);
		public void SetError(string error)
			=> new Action(() => { }).Invoke();
	}
}
