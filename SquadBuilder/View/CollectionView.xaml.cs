using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class CollectionView : BaseView
	{
		public CollectionView ()
		{
			InitializeComponent ();
			BindingContext = new CollectionViewModel ();

			MessagingCenter.Subscribe <CollectionViewModel> (this, "Clear Collection", async vm => {
				var accept = await DisplayAlert ("Clear Collection Info", "Are you sure you want to erase all collection info?", "Ok", "Cancel");

				if (!accept)
					return;

				foreach (var expansion in Cards.SharedInstance.Expansions)
					expansion.Owned = 0;

				foreach (var ship in Cards.SharedInstance.Ships)
					ship.Owned = 0;

				foreach (var pilot in Cards.SharedInstance.Pilots)
					pilot.Owned = 0;

				foreach (var upgrade in Cards.SharedInstance.Upgrades)
					upgrade.Owned = 0;

				Cards.SharedInstance.SaveCollection ();
			});
		}
	}
}

