using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class CreateSquadronView : BaseView
	{
		public CreateSquadronView ()
		{
			InitializeComponent ();

			MessagingCenter.Subscribe <CreateSquadronViewModel> (this, "Negative Squad Points", vm => {
				DisplayAlert ("Invalid Point Value", "Squadron Points value cannot be negative", "Okay");
			});
		}
	}
}

