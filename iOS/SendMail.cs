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
			try {
				if (MFMailComposeViewController.CanSendMail) {
					MFMailComposeViewController mail = new MFMailComposeViewController ();

					mail.Finished += async (object mailSender, MFComposeResultEventArgs ea) => {
						await rootController.DismissViewControllerAsync (true);
						if (ea.Result == MFMailComposeResult.Failed)
							new UIAlertView ("Message Failed!",
								"Your email failed to send",
								null, "Okay", null).Show ();
					};

					mail.SetToRecipients (new string [] { "support@risiapps.com" });
					mail.SetSubject ("Squad Builder Feedback");
					mail.SetMessageBody ("", false);
					await rootController.PresentViewControllerAsync (mail, true);
				} else {
					var alert = UIAlertController.Create ("Error", "No e-mail account setup on device.  You can e-mail feedback to support@risiapps.com", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create ("Okay" ,UIAlertActionStyle.Default,(action) => {}));
					rootController.PresentViewControllerAsync (alert, true);
				}
			} catch (Exception e) {
				new UIAlertView ("Error", "No e-mail account setup on device.  You can e-mail feedback to support@risiapps.com", null, "Okay", null).Show ();
			}
		}
	}
}

