using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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
		}
	}
}

