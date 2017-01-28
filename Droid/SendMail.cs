using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Xamarin.Forms;
using Android.Content.PM;
using HockeyApp.Android;
using Android.App;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.SendMail))]
namespace SquadBuilder.Droid
{
	public class SendMail : ISendMail
	{
		public static Context ApplicationContext;

		public void SendFeedback ()
		{
			FeedbackManager.ShowFeedbackActivity (SendMail.ApplicationContext);

			//var emailIntent = new Intent (Intent.ActionSend);
			//emailIntent.SetType ("plain/text");
			//emailIntent.PutExtra (Intent.ExtraEmail, new [] { "support@risiapps.com" });
			
			//var versionNumber = Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionName;
			//var minorVersion = Forms.Context.PackageManager.GetPackageInfo (Forms.Context.PackageName, 0).VersionCode;
			//emailIntent.PutExtra (Intent.ExtraSubject, "Squad Builder Feedback - Android " + versionNumber + " (" + minorVersion + ")");
			//emailIntent.PutExtra (Intent.ExtraText, "");
			//Forms.Context.StartActivity (Intent.CreateChooser (emailIntent, "Send Feedback"));
		}
	}
}

