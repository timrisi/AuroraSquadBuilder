using System;

using Android.App;
using Android.Content;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.ClipboardService))]
namespace SquadBuilder.Droid
{
	public class ClipboardService : IClipboardService
	{
		public void CopyToClipboard(String text)
		{
			var clipboardmanager = (ClipboardManager)Forms.Context.GetSystemService (Context.ClipboardService);
			clipboardmanager.Text = text;
		}
	}
}

