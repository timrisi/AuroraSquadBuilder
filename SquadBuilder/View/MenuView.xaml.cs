using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class MenuView : BaseView
	{
		public MenuView ()
		{
			InitializeComponent ();
			BindingContext = new MenuViewModel ();

			MessagingCenter.Subscribe<Squadron, string> (this, "Xws Error", (Squadron squadron, string error) => {
				DisplayAlert ("Xws Error", error, "Okay");
			});

			MessagingCenter.Subscribe<MainViewModel> (this, "Sort squadrons", async (obj) => {
				var response = await DisplayActionSheet ("Sort Squadrons", null, "Cancel", "Alphabetical", "Wins", "Loses", "Total Games", "Manual");
				MessagingCenter.Send<MenuView, string> (this, "Sort type selected", response);
			});
		}
	}
}

