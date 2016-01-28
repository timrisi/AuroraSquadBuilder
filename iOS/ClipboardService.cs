using System;
using UIKit;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.iOS.ClipboardService))]
namespace SquadBuilder.iOS
{
	public class ClipboardService : IClipboardService
	{
		public void CopyToClipboard(String text)
		{
			UIPasteboard clipboard = UIPasteboard.General;
			clipboard.String = text;
		}
	}
}

