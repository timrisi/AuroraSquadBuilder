﻿using System;

using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Xamarin.Forms;

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

				if (value != null) {
					if (Device.OS == TargetPlatform.iOS) {
						NavigationService.PopAsync (); //<UpgradesListViewModel> (this);
						MessagingCenter.Send<UpgradesListViewModel, Upgrade> (this, "Upgrade selected", selectedUpgrade.Copy ());
					} else {
						MessagingCenter.Send<UpgradesListViewModel, Upgrade> (this, "Upgrade selected", selectedUpgrade.Copy ());
						NavigationService.PopAsync (); //<UpgradesListViewModel> (this);
					}
				}
			}
		}

		ObservableCollection <Upgrade> GetUpgrades (string type)
		{
			try {
				var upgrades = Upgrade.Upgrades
					.Where (u => u.Category == type)
				                    .Where (u => Squadron.CurrentSquadron.Faction.Id == "mixed" || !u.FactionRestricted || u.Factions.Any (f => f.Id == Pilot.Faction.Id) || (u.Id == "maul" && Squadron.CurrentSquadron.Pilots.Any (p => p.Name.Contains ("Ezra Bridger") || (bool)(p.UpgradesEquippedString?.Contains ("Ezra Bridger") ?? false))))
					.Where (u => string.IsNullOrEmpty (u.RequiredAction) || Pilot.Ship.Actions.Contains (u.RequiredAction))
					.Where (u => u.ShipRequirement == null || meetsRequirement (u.ShipRequirement))
					.Where (u => u.MinPilotSkill <= Pilot.PilotSkill)
					.Where (u => u.MaxAgility == null || Pilot.Agility <= u.MaxAgility)
					.Where (u => u.ShieldRequirement == null || Pilot.Shields == u.ShieldRequirement)
					.Where (u => !u.IsCustom || Settings.AllowCustom)
					.Where (u => !u.CCL || Settings.CustomCardLeague)
					.Where (u => !u.HotAC || Settings.IncludeHotac).ToList ();

				if (Settings.AllowCustom) {
					var customUpgrades = Upgrade.CustomUpgrades
						.Where (u => u.Category == type)
						  .Where (u => !u.FactionRestricted || u.Factions.Any (f => f.Id == Pilot.Faction.Id))
						.Where (u => string.IsNullOrEmpty (u.ShipRequirement) || meetsRequirement (u.ShipRequirement))
						.Where (u => string.IsNullOrEmpty (u.RequiredAction) || Pilot.Ship.Actions.Contains (u.RequiredAction))
						.Where (u => u.MinPilotSkill <= Pilot.PilotSkill);

					upgrades.AddRange (customUpgrades);
				}

				upgrades = upgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

				ObservableCollection<Upgrade> valid = new ObservableCollection<Upgrade> ();
				foreach (var upgrade in upgrades) {
#region Hard-coded Exceptions
					if (upgradeType == "Salvaged Astromech" && (bool)(Pilot.UpgradesEquippedString?.Contains ("Havoc") ?? false) && !upgrade.Unique)
						continue;

					if (upgradeType == "Modification" && (bool)(Pilot.UpgradesEquippedString?.Contains ("Smuggling Compartment") ?? false) && upgrade.Cost > 3)
						continue;

					if (upgradeType == "Crew" && (bool)(Pilot.UpgradesEquippedString?.Contains ("TIE Shuttle") ?? false) && upgrade.Cost > 4)
						continue;
#endregion

					if (upgrade.Unique && (bool) Squadron.CurrentSquadron.Pilots?.Any (p => p != null && p.Name == upgrade.Name || (bool) p?.UpgradesEquipped?.Any (u => u?.Name == upgrade?.Name)))
						continue;

					if (upgrade.SquadLimit != null && Squadron.CurrentSquadron.Pilots?.Count (p => p != null && (bool)(p.UpgradesEquippedString?.Contains (upgrade.Name) ?? false)) >= upgrade.SquadLimit)
						continue;

					if (upgrade.Limited && (bool)Pilot?.UpgradesEquipped?.Any (u => u?.Name == upgrade.Name))
						continue;
					
					if (!upgrade.Slots.Any () && !upgrade.RequiredSlots.Any ()) {
						valid.Add (upgrade);
						continue;
					}

					var pilotUpgrades = new List<object> (Pilot.Upgrades);
					pilotUpgrades.Remove (Upgrade.CreateUpgradeSlot (upgrade.Category));

					bool isValid = true;
					foreach (var slot in upgrade.Slots) {
						var slotObject = Upgrade.CreateUpgradeSlot (slot);
						if (pilotUpgrades.Contains (slotObject))
							pilotUpgrades.Remove (slotObject);
						else {
							isValid = false;
							break;
						}
					}

					if (isValid) {
						var upgradeTypes = new List<string> (Pilot.UpgradeTypes);
						foreach (var slot in upgrade.RequiredSlots) {
							if (upgradeTypes.Contains (slot))
								upgradeTypes.Remove (slot);
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

				return new ObservableCollection<Upgrade> (valid.Where (u => !u.LargeOnly && !u.HugeOnly));
			} catch (Exception e) {
				return new ObservableCollection<Upgrade> ();
			}
		}

		bool meetsRequirement (string requirementList)
		{
			
			var requirements = requirementList.Split (',');

			if (!requirements.Any (r => r.Trim ().ToLower ().Split (' ').All (s => Pilot.Ship.Name.ToLower ().Split (' ').Any (p => p.Contains (s)))))
				return false;

			return true;
		}

		public string PointsDescription {
			get { return Squadron.CurrentSquadron.PointsDescription; }
		}

		Command noUpgrade;
		public Command NoUpgrade {
			get {
				if (noUpgrade == null)
					noUpgrade = new Command (() => {
						MessagingCenter.Send <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", null);
						NavigationService.PopAsync (); // <UpgradesListViewModel> (this);
					});

				return noUpgrade;
			}
		}

		string searchText;
		public string SearchText {
			get { return searchText; }
			set {
				if (value == null)
					value = "";

				SetProperty (ref searchText, value);

				SearchUpgrades (value);
			}
		}

		public void SearchUpgrades (string text)
		{
			try {
				text = text.ToLower ();
				var upgrades = GetUpgrades (UpgradeType).Where (u =>
					   u.Name.ToLower ().Contains (text) ||
					   u.Text.ToLower ().Contains (text) ||
                                           u.Keywords.Contains (text) ||
				       	   (u.FactionRestricted && u.Factions.Any (f => f.Name.ToLower ().Contains (text))) ||
                                           (!string.IsNullOrEmpty (u.ShipRequirement) && u.ShipRequirement.ToLower ().Contains (text)));
				Upgrades = new ObservableCollection<Upgrade> (upgrades);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}
	}
}

