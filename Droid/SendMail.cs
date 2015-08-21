using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Xamarin.Forms;

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
			emailIntent.PutExtra (Intent.ExtraSubject, "Squad Builder Feedback");
			emailIntent.PutExtra (Intent.ExtraText, "");
			Forms.Context.StartActivity (Intent.CreateChooser (emailIntent, "Send Feedback"));
		}
	}
}

