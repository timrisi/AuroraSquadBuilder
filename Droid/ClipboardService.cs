using System;

using Android.App;
using Android.Content;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.ClipboardService))]
namespace SquadBuilder.Droid
{
	public class ClipboardService : IClipboardService
	{
		public void CopyToClipboard(String text)
		{
			LaunchActivity.AndroidClipboardManager.Text = text;
		}
	}
}

