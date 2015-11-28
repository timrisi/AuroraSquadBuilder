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
				.Where (u => u.Ship == null || u.Ship.Id == Pilot.Ship.Id)
				.Where (u => u.Name != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
				.Where (u => u.Name != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak")).ToList ();

			if (Settings.AllowCustom) {
				var customUpgrades = Cards.SharedInstance.CustomUpgrades
					.Where (u => u.Category == type)
					.Where (u => u.Faction == null || u.Faction.Id == Pilot.Faction.Id)
					.Where (u => u.Ship == null || u.Ship.Id == Pilot.Ship.Id)
					.Where (u => u.Name != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
					.Where (u => u.Name != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"));
				
				upgrades.AddRange (customUpgrades);
			}

			upgrades = upgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

			ObservableCollection <Upgrade> valid = new ObservableCollection<Upgrade> ();
			foreach (var upgrade in upgrades) {
				if (upgrade.TIEOnly && !Pilot.Ship.Name.Contains ("TIE"))
					continue;
				
				if (upgrade.Slots.Count () == 0) {
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
				if (isValid)
					valid.Add (upgrade);
			}

			if (Pilot.Ship.LargeBase)
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.HugeOnly && !u.SmallOnly));
			else if (Pilot.Ship.Huge) {
				if (upgradeType == "Modification")
					return new ObservableCollection<Upgrade> (valid.Where (u => u.HugeOnly));
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.SmallOnly && !u.LargeOnly));
			}
					
			return new ObservableCollection <Upgrade> (valid.Where (u => !u.LargeOnly && !u.HugeOnly));		
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

