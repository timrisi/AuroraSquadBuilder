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
				if (upgrade?.Name == "\"Heavy Scyk\" Interceptor") {
					var upgr = upgrade;
					MessagingCenter.Subscribe <UpgradesListView, string> (this, "Scyk Upgrade Selected", (uvm, upgradeType) => {
						if (upgradeType != "Cancel") {
							upgr.AdditionalUpgrades.Add (upgradeType);
							updateUpgrade (index, upgr, vm);
						}
						MessagingCenter.Unsubscribe <UpgradesListView, string> (this, "Scyk Upgrade Selected"); 
					});
					MessagingCenter.Send <PilotViewModel> (this, "Select Scyk Upgrade");
				} else
					updateUpgrade (index, upgrade, vm);

				NotifyPropertyChanged ("PointsDescription");
				MessagingCenter.Unsubscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected");
			});

			Navigation.PushAsync <UpgradesListViewModel> ((vm,p) => {
				vm.Pilot = Pilot;
				vm.UpgradeType = Pilot.UpgradeTypes [index];
				selectedUpgrade = null;
			});
		}

		void updateUpgrade (int index, Upgrade upgrade, UpgradesListViewModel vm) 
		{
			var oldUpgrade = Pilot.UpgradesEquipped [index];

			Pilot.UpgradesEquipped [index] = upgrade;

			if (oldUpgrade != null) {
				foreach (var additionalUpgrade in oldUpgrade.AdditionalUpgrades) {
					var oldIndex = Pilot.UpgradeTypes.ToList ().LastIndexOf (additionalUpgrade);
					Pilot.UpgradeTypes.RemoveAt (oldIndex);
					Pilot.UpgradesEquipped.RemoveAt (oldIndex);
				}

				foreach (var removedUpgrade in oldUpgrade.RemovedUpgrades) {
					Pilot.UpgradeTypes.Add (removedUpgrade);
					Pilot.UpgradesEquipped.Add (null);
				}

				for (int i = 0; i < oldUpgrade.Slots.Count (); i++) {
					var extraIndex = Pilot.UpgradesEquipped.IndexOf (oldUpgrade);
					if (index >= 0)
						Pilot.UpgradesEquipped [extraIndex] = null;
				}

				if (oldUpgrade.Id == "ordnancetubes") {
					for (int i = 0; i < pilot.UpgradeTypes.Count; i++) {
						var type = pilot.UpgradeTypes [i];
						if (type == "Missile" || type == "Torpedo") {
							pilot.UpgradeTypes [i] = "Hardpoint";
							pilot.UpgradesEquipped [i] = null;
						}
					}

					if (pilot.LinkedPilotCardGuid != Guid.Empty) {
						var otherPilot = Cards.SharedInstance.CurrentSquadron.Pilots.First (p => p.Name != pilot.Name && p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid);
						for (int i = 0; i < otherPilot.UpgradeTypes.Count; i++) {
							var type = otherPilot.UpgradeTypes [i];
							if (type == "Missile" || type == "Torpedo") {
								otherPilot.UpgradeTypes [i] = "Hardpoint";
								otherPilot.UpgradesEquipped [i] = null;
							}
						}
					}
				}
			}
			if (upgrade != null) {
				foreach (var newUpgrade in upgrade.AdditionalUpgrades) {
					Pilot.UpgradeTypes.Add (newUpgrade);
					Pilot.UpgradesEquipped.Add (null);
				}

				foreach (var removedUpgrade in upgrade.RemovedUpgrades) {
					var removedIndex = Pilot.UpgradeTypes.IndexOf (removedUpgrade);
					if (removedIndex < 0)
						continue;
					Pilot.UpgradeTypes.RemoveAt (removedIndex);
					Pilot.UpgradesEquipped.RemoveAt (removedIndex);
				}

				for (int i = 0; i < upgrade.Slots.Count (); i++) {
					var extraIndex = Pilot.Upgrades.IndexOf (new { Name = upgrade.Slots [i], IsUpgrade = false });
					if (index >= 0)
						Pilot.UpgradesEquipped [extraIndex] = upgrade;
				}

				if (upgrade.Id == "misthunter")
					pilot.UpgradesEquipped [pilot.UpgradesEquipped.Count - 1] = Cards.SharedInstance.Upgrades.First (u => u.Name == "Tractor Beam");
			}

			NotifyPropertyChanged ("Pilot");
			Navigation.RemoveAsync <UpgradesListViewModel> (vm);
		}

		RelayCommand selectUpgrade;
		public RelayCommand SelectUpgrade {
			get {
				if (selectUpgrade == null)
					selectUpgrade = new RelayCommand (() => {
						
						MessagingCenter.Subscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", (vm, upgrade) => {
							
							Navigation.RemoveAsync <UpgradesListViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created");
						});
						Navigation.PushAsync <CreateSquadronViewModel> ();
					});

				return selectUpgrade;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();
			SelectedUpgrade = null;

			MessagingCenter.Unsubscribe <UpgradesListViewModel, Upgrade> (this, "Upgrade selected");
		}
	}
}

