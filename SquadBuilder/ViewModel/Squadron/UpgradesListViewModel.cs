using System;
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

				if (value != null) {
					if (Device.OS == TargetPlatform.iOS) {
						Navigation.RemoveAsync<UpgradesListViewModel> (this);
						MessagingCenter.Send<UpgradesListViewModel, Upgrade> (this, "Upgrade selected", selectedUpgrade.Copy ());
					} else {
						MessagingCenter.Send<UpgradesListViewModel, Upgrade> (this, "Upgrade selected", selectedUpgrade.Copy ());
						Navigation.RemoveAsync<UpgradesListViewModel> (this);
					}
				}
			}
		}

		ObservableCollection <Upgrade> GetUpgrades (string type)
		{
			var upgrades = Cards.SharedInstance.Upgrades
				.Where (u => u.Category == type)
                .Where (u => Cards.SharedInstance.CurrentSquadron.Faction.Id == "mixed" || !u.FactionRestricted || u.Factions.Any (f => f.Id == Pilot.Faction.Id))
				.Where (u => string.IsNullOrEmpty (u.RequiredAction) || Pilot.Ship.Actions.Contains (u.RequiredAction))
				.Where (u => u.ShipRequirement == null || meetsRequirement (u.ShipRequirement))
				.Where (u => u.MinPilotSkill <= Pilot.PilotSkill)
                .Where (u => u.MaxAgility == null || Pilot.Agility <= u.MaxAgility)
                .Where (u => u.ShieldRequirement == null || Pilot.Shields == u.ShieldRequirement)
                .Where (u => !u.IsCustom || Settings.AllowCustom)
                .Where (u => !u.CCL || Settings.CustomCardLeague).ToList ();

			if (Settings.AllowCustom) {
				var customUpgrades = Cards.SharedInstance.CustomUpgrades
					.Where (u => u.Category == type)
                  	.Where (u => !u.FactionRestricted || u.Factions.Any (f => f.Id == Pilot.Faction.Id))
					.Where (u => string.IsNullOrEmpty (u.ShipRequirement) || meetsRequirement (u.ShipRequirement))
					.Where (u => string.IsNullOrEmpty (u.RequiredAction) || Pilot.Ship.Actions.Contains (u.RequiredAction))
					.Where (u => u.MinPilotSkill <= Pilot.PilotSkill);
				
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
					var upgradeTypes = new List <string> (Pilot.UpgradeTypes);
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
					
			return new ObservableCollection <Upgrade> (valid.Where (u => !u.LargeOnly && !u.HugeOnly));		
		}

		bool meetsRequirement (string shipRequirement)
		{
			var requirements = shipRequirement.Split (',');

			if (!requirements.Any (r => r.Trim ().ToLower ().Split (' ').All (s => Pilot.Ship.Name.ToLower ().Split (' ').Contains (s))))
				return false;

			return true;
		}

		public string PointsDescription {
			get { return Cards.SharedInstance.CurrentSquadron.PointsDescription; }
		}

		RelayCommand noUpgrade;
		public RelayCommand NoUpgrade {
			get {
				if (noUpgrade == null)
					noUpgrade = new RelayCommand (() => {
						MessagingCenter.Send <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", null);
						Navigation.RemoveAsync <UpgradesListViewModel> (this);
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
			text = text.ToLower ();
			Upgrades = new ObservableCollection<Upgrade> (GetUpgrades (UpgradeType).Where (u =>
																						   u.Name.ToLower ().Contains (text) ||
																						   u.Text.ToLower ().Contains (text) ||
			                                                                               (u.FactionRestricted && u.Factions.Any (f => f.Name.ToLower ().Contains (text))) ||
			                                                                               (!string.IsNullOrEmpty (u.ShipRequirement) && u.ShipRequirement.ToLower ().Contains (text))
			                                                                              ));
		}
	}
}

