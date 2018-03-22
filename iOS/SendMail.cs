using System;
using UIKit;
using MessageUI;
using System.Runtime.CompilerServices;
using Foundation;
using HockeyApp.iOS;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.iOS.SendMail))]
namespace SquadBuilder.iOS
{
	public class SendMail : ISendMail
	{
		public void SendFeedback ()
		{
			var feedbackManager = BITHockeyManager.SharedHockeyManager.FeedbackManager;
			feedbackManager.ShowFeedbackComposeView ();
		}
	}
}

