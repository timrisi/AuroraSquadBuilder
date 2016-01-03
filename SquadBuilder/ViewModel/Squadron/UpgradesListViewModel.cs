﻿using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Xamarin.Forms;
using XLabs;
using System.Collections.Generic;
using System.IO;

namespace SquadBuilder
{
	public class UpgradesListViewModel : ViewModel
	{
		string upgradeType;
		public string UpgradeType { 
			get { return upgradeType; }
			set {
				upgradeType = value;
				Upgrades = GetUpgrades (value);
			}
		}

		ObservableCollection <Upgrade> upgrades;
		public ObservableCollection <Upgrade> Upgrades {
			get { return upgrades; }
			set {
				SetProperty (ref upgrades, value);
			}
		}

		Pilot pilot;
		public Pilot Pilot {
			get { return pilot; }
			set {
				SetProperty (ref pilot, value);
			}
		}

		 Upgrade selectedUpgrade;
		 public Upgrade SelectedUpgrade {
			get { return selectedUpgrade; }
			set {
				SetProperty (ref selectedUpgrade, value);

				if (value != null)
					MessagingCenter.Send <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", selectedUpgrade.Copy ());
			}
		}

		ObservableCollection <Upgrade> GetUpgrades (string type)
		{
			var upgrades = Cards.SharedInstance.Upgrades
				.Where (u => u.Category == type)
				.Where (u => u.Faction == null || u.Faction.Id == Pilot.Faction.Id)
				.Where (u => u.Name != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
				.Where (u => u.Name != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"))
				.Where (u => u.ShipRequirement == null || meetsRequirement (u.ShipRequirement)).ToList ();

			if (Settings.AllowCustom) {
				var customUpgrades = Cards.SharedInstance.CustomUpgrades
					.Where (u => u.Category == type)
					.Where (u => u.Faction == null || u.Faction.Id == Pilot.Faction.Id)
					.Where (u => string.IsNullOrEmpty (u.ShipRequirement) || meetsRequirement (u.ShipRequirement))
					.Where (u => u.Name != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
					.Where (u => u.Name != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"));
				
				upgrades.AddRange (customUpgrades);
			}

			upgrades = upgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

			ObservableCollection <Upgrade> valid = new ObservableCollection<Upgrade> ();
			foreach (var upgrade in upgrades) {
				if (!upgrade.Slots.Any () && !upgrade.RequiredSlots.Any ()) {
					valid.Add (upgrade);	
					continue;
				}

				var pilotUpgrades = new List <object> (Pilot.Upgrades);
				pilotUpgrades.Remove (new { Name = upgrade.Category, IsUpgrade = false });

				bool isValid = true;
				foreach (var slot in upgrade.Slots) {
					var slotObject = new { Name = slot, IsUpgrade = false };
					if (pilotUpgrades.Contains (slotObject))
						pilotUpgrades.Remove (slotObject);
					else {
						isValid = false;
						break;
					}
				}

				if (isValid) {
					pilotUpgrades = new List <object> (Pilot.Upgrades);
					foreach (var slot in upgrade.RequiredSlots) {
						var slotObject = new { Name = slot, IsUpgrade = false };
						if (pilotUpgrades.Contains (slotObject))
							pilotUpgrades.Remove (slotObject);
						else {
							isValid = false;
							break;
						}
					}
				}

				if (isValid)
					valid.Add (upgrade);
			}

			if (Settings.HideUnavailable)
				valid = new ObservableCollection<Upgrade> (valid.Where (u => u.IsAvailable));

			if (Pilot.Ship.LargeBase)
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.HugeOnly && !u.SmallOnly));
			else if (Pilot.Ship.Huge) {
				if (upgradeType == "Modification")
					return new ObservableCollection<Upgrade> (valid.Where (u => u.HugeOnly));
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.SmallOnly && !u.LargeOnly));
			}
					
			return new ObservableCollection <Upgrade> (valid.Where (u => !u.LargeOnly && !u.HugeOnly));		
		}

		bool meetsRequirement (string shipRequirement)
		{
			var requirements = shipRequirement.Split (' ');

			foreach (var requirement in requirements) {
				if (!Pilot.Ship.Name.ToLower ().Contains (requirement.ToLower ()))
					return false;
			}

			return true;
		}

		RelayCommand noUpgrade;
		public RelayCommand NoUpgrade {
			get {
				if (noUpgrade == null)
					noUpgrade = new RelayCommand (() => MessagingCenter.Send <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", null));

				return noUpgrade;
			}
		}

		string searchText;
		public string SearchText {
			get { return searchText; }
			set {
				SetProperty (ref searchText, value);

				SearchUpgrades (value);
			}
		}

		public void SearchUpgrades (string text)
		{
			Upgrades = new ObservableCollection<Upgrade> (GetUpgrades (UpgradeType).Where (u => u.Name.ToLower ().Contains (text.ToLower ())));
		}
	}
}
