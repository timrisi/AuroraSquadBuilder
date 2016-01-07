using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Xamarin.Forms;
using Android.Content.PM;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.SendMail))]
namespace SquadBuilder.Droid
{
	public class SendMail : ISendMail
	{
		public void SendFeedback ()
		{
			var emailIntent = new Intent (Intent.ActionSend);
			emailIntent.SetType ("plain/text");
			emailIntent.PutExtra (Intent.ExtraEmail, new [] { "support@risiapps.com" });
			
			var versionNumber = Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionName;
			var minorVersion = Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionCode;
			emailIntent.PutExtra (Intent.ExtraSubject, "Squad Builder Feedback - Android " + versionNumber + " (" + minorVersion + ")");
			emailIntent.PutExtra (Intent.ExtraText, "");
			Forms.Context.StartActivity (Intent.CreateChooser (emailIntent, "Send Feedback"));
		}
	}
}

