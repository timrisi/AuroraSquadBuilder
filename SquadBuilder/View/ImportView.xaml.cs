using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class ImportView : BaseView
	{
		public ImportView ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			MessagingCenter.Subscribe <Squadron> (this, "Error importing squad", ((squadron) => {
				DisplayAlert ("Error importing squad", "", "Okay");
			}));
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
		}
	}
}

