﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SquadBuilder
{
	public partial class EditUpgradeView : ContentPage
	{
		public EditUpgradeView ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();

			EditUpgradeViewModel vm = BindingContext as EditUpgradeViewModel;
			if (vm != null) {
				factionPicker.Items.Clear();
				factionPicker.Items.Add ("Any");
				foreach (var faction in vm.Factions)
					factionPicker.Items.Add(faction.Name);

				shipPicker.Items.Clear ();
				shipPicker.Items.Add ("Any");
				foreach (var ship in vm.Ships)
					shipPicker.Items.Add (ship.Name);

				upgradeTypePicker.Items.Clear ();
				foreach (var upgradeType in vm.UpgradeTypes)
					upgradeTypePicker.Items.Add (upgradeType);
			}
		}
	}
}

