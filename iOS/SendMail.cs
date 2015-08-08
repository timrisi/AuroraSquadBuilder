using System;
using UIKit;
using MessageUI;
using System.Runtime.CompilerServices;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.iOS.SendMail))]
namespace SquadBuilder.iOS
{
	public class SendMail : ISendMail
	{
		public async void SendFeedback ()
		{
			var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			MFMailComposeViewController mail = new MFMailComposeViewController ();
			mail.Finished += async (object mailSender, MFComposeResultEventArgs ea) => {
				await rootController.DismissViewControllerAsync (true);
				if (ea.Result == MFMailComposeResult.Failed)
					new UIAlertView ("Message Failed!",
						"Your email failed to send",
						null, "Okay", null).Show ();
			};
			if (MFMailComposeViewController.CanSendMail) {
				mail.SetToRecipients (new string [] { "support@risiapps.com" });
				mail.SetSubject ("Squad Builder Feedback");
				mail.SetMessageBody ("", false);
				await rootController.PresentViewControllerAsync (mail, true);
			}
		}
	}
}

