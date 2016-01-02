using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SquadBuilder
{
	public partial class EditPilotView : ContentPage
	{
		public EditPilotView ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			EditPilotViewModel vm = BindingContext as EditPilotViewModel;
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