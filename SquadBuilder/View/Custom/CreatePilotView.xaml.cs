using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SquadBuilder
{
	public partial class CreatePilotView : ContentPage
	{
		public CreatePilotView ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			CreatePilotViewModel vm = BindingContext as CreatePilotViewModel;
			if (vm != null) {
				factionPicker.Items.Clear();
				foreach (var faction in vm.Factions) {
					if (faction.Name != "Mixed")
						factionPicker.Items.Add (faction.Name);
				}

				shipPicker.Items.Clear ();
				foreach (var ship in vm.Ships)
					shipPicker.Items.Add (ship.Name);
			}
		}
	}
}