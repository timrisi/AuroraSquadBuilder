using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class SquadronView : BaseView
	{
		public SquadronView ()
		{
			InitializeComponent ();

			MessagingCenter.Subscribe <SquadronViewModel> (this, "Squadron Copied", async vm => {
				await DisplayAlert ("", "Copied to Clipboard", "OK");
			});

			MessagingCenter.Subscribe <SquadronViewModel> (this, "Squadron Copied as XWS data", async vm => {
				await DisplayAlert ("", "Exported to Clipboard as XWS", "OK");
			});
		}
	}
}

