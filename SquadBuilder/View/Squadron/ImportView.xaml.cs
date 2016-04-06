using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

			MessagingCenter.Subscribe <Squadron, IList<string>> (this, "Invalid xws info", ((squadron, errors) => {
				DisplayAlert ("Invalid xws info", string.Join ("\n", errors), "Okay");
			}));
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
		}
	}
}

