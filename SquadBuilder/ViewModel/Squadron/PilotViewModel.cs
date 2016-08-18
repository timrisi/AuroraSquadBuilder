using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class PilotViewModel : ViewModel
	{
		Pilot pilot;
		public Pilot Pilot {
			get { return pilot; }
			set { SetProperty (ref pilot, value); }
		}

		object selectedUpgrade;
		public object SelectedUpgrade {
			get { return selectedUpgrade; }
			set {
				SetProperty (ref selectedUpgrade, value);
				if (value != null) {
					var index = Pilot.Upgrades.IndexOf (value);

					if (index < 0) {
						var upgrade = Pilot.UpgradesEquipped.FirstOrDefault (u => u.Name == (selectedUpgrade));
						index = Pilot.UpgradesEquipped.IndexOf (upgrade);
					}

					if ((pilot.UpgradesEquipped.Any (u => u?.Id == "ordnancetubes") || 
						(pilot.LinkedPilotCardGuid != Guid.Empty && Cards.SharedInstance.CurrentSquadron.Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid && p.Name != pilot.Name).UpgradesEquipped.Any (u => u?.Id == "ordnancetubes"))) &&
					    (Pilot.UpgradeTypes [index] == "Hardpoint" ||
					     Pilot.UpgradeTypes [index] == "Torpedo" ||
						 Pilot.UpgradeTypes [index] == "Missile") &&
						 Pilot.UpgradesEquipped [index] == null) {
						MessagingCenter.Subscribe <PilotView, string> (this, "Ordnance Type Selected", (vm, ordnanceType) => {
							if (ordnanceType != "Cancel") {
								Pilot.UpgradeTypes [index] = ordnanceType;
								pushUpgradeList (index);
							} else
								SelectedUpgrade = null;	
							MessagingCenter.Unsubscribe <PilotView, string> (this, "Ordnance Type Selected");
						});

						MessagingCenter.Send <PilotViewModel, string[]> (this, "Select Ordnance Tubes Type", new [] {
							"Hardpoint",
							"Torpedo",
							"Missile"
						});
					} else
						pushUpgradeList (index);
				}
			}
		}

		public string PointsDescription {
			get { return Cards.SharedInstance.CurrentSquadron.PointsDescription; }
		}

		void pushUpgradeList (int index)
		{
			MessagingCenter.Subscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", (vm, upgrade) => {
				if (upgrade?.UpgradeOptions != null && upgrade.UpgradeOptions.Any ()) {
					var upgr = upgrade;
					MessagingCenter.Subscribe<UpgradesListView, string> (this, "Upgrade Option Selected", (uvm, upgradeType) => {
						if (upgradeType != "Cancel") {
							upgr.AdditionalUpgrades.Add (upgradeType);
							updateUpgrade (index, upgr);
						}
						MessagingCenter.Unsubscribe<UpgradesListView, string> (this, "Upgrade Option Selected");
					});
					MessagingCenter.Send (this, "Select Upgrade Option", upgr.UpgradeOptions.ToList ());
				} else
					updateUpgrade (index, upgrade);

				NotifyPropertyChanged ("PointsDescription");
				MessagingCenter.Unsubscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected");
			});

			Navigation.PushAsync <UpgradesListViewModel> ((vm,p) => {
				vm.Pilot = Pilot;
				vm.UpgradeType = Pilot.UpgradeTypes [index];
				selectedUpgrade = null;
			});
		}

		void updateUpgrade (int index, Upgrade upgrade) 
		{
			Pilot.EquipUpgrade (index, upgrade);

			NotifyPropertyChanged ("Pilot");
		}

		RelayCommand selectUpgrade;
		public RelayCommand SelectUpgrade {
			get {
				if (selectUpgrade == null)
					selectUpgrade = new RelayCommand (() => {
						MessagingCenter.Subscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", (vm, upgrade) => {
//							Navigation.RemoveAsync <UpgradesListViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created");
						});
						Navigation.PushAsync <CreateSquadronViewModel> ();
					});

				return selectUpgrade;
			}
		}

		RelayCommand changePilot;
		public RelayCommand ChangePilot {
			get {
				if (changePilot == null) {
					changePilot = new RelayCommand (() => {
						MessagingCenter.Subscribe<PilotsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
							MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");

							var upgradeTypes = new ObservableCollection <string> (this.Pilot.UpgradeTypes);
							var upgradesEquipped = new ObservableCollection <Upgrade> (this.Pilot.UpgradesEquipped);

							var eptCount = upgradeTypes.Count (u => u == "Elite Pilot Talent");
							if (upgradesEquipped.Any (u => u?.Id == "awingtestpilot"))
								eptCount--;

							if (eptCount > 0 && !pilot.UpgradeTypes.Contains ("Elite Pilot Talent")) {
								var index = upgradeTypes.IndexOf ("Elite Pilot Talent");
								upgradeTypes.RemoveAt (index);
								upgradesEquipped.RemoveAt (index);
							}

							if (eptCount == 0 && pilot.UpgradeTypes.Contains ("Elite Pilot Talent")) {
								upgradeTypes.Insert (0, "Elite Pilot Talent");
								upgradesEquipped.Insert (0, null);
							}

							if (this.Pilot.Id == "outerrimsmuggler")
								upgradeTypes.Insert (upgradeTypes.IndexOf ("Crew"), "Missile");
							else if (this.Pilot.Ship.Id == "yt1300")
								upgradeTypes.Remove ("Missile");

							pilot.UpgradeTypes = upgradeTypes;
							pilot.UpgradesEquipped = upgradesEquipped;

							var pilotIndex = Cards.SharedInstance.CurrentSquadron.Pilots.IndexOf (this.Pilot);
							Cards.SharedInstance.CurrentSquadron.Pilots [pilotIndex] = pilot.Copy ();
							this.Pilot = Cards.SharedInstance.CurrentSquadron.Pilots [pilotIndex];
						});

						Navigation.PushAsync<PilotsListViewModel> ((vm, p) => {
							vm.Faction = Pilot?.Faction;
							vm.Ship = Pilot?.Ship;
						});
					});
				}

				return changePilot;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();
			SelectedUpgrade = null;

			MessagingCenter.Unsubscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected");
			MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");
		}
	}
}

