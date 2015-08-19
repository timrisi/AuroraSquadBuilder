using System;
using System.Runtime.CompilerServices;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.SendMail))]
namespace SquadBuilder.Droid
{
	public class SendMail : ISendMail
	{
		public async void SendFeedback ()
		{
			
		}
	}
}

