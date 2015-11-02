using System;
using XLabs.Forms.Mvvm;
using System.IO;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XLabs;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CustomUpgradesViewModel : ViewModel
	{
		public CustomUpgradesViewModel ()
		{
			MessagingCenter.Subscribe <Upgrade> (this, "Remove Upgrade", upgrade => {
				var upgradeGroup = Upgrades.FirstOrDefault (g => g.Contains (upgrade));

				if (upgradeGroup != null) {
					upgradeGroup.Remove (upgrade);
					if (upgradeGroup.Count == 0)
						Upgrades.Remove (upgradeGroup);
					Cards.SharedInstance.CustomUpgrades.Remove (upgrade);
				}
			});

			MessagingCenter.Subscribe <Upgrade> (this, "Edit Upgrade", upgrade => {
				string upgradeName = upgrade.Name;
				string shipName = upgrade.Ship?.Name;
				string factionName = upgrade.Faction?.Name;

				MessagingCenter.Subscribe <EditUpgradeViewModel, Upgrade> (this, "Finished Editing", (vm, updatedUpgrade) => {
					var originalGroup = Upgrades.FirstOrDefault (g => g.ToList ().Exists (u => u.Name == upgrade.Name));
					if (originalGroup != null) {
						originalGroup.Remove (upgrade);

						if (originalGroup.Count == 0)
							Upgrades.Remove (originalGroup);
					}

					if (Cards.SharedInstance.CustomUpgrades.Contains (upgrade))
						Cards.SharedInstance.CustomUpgrades [Cards.SharedInstance.CustomUpgrades.IndexOf (upgrade)] = updatedUpgrade;
					else
						Cards.SharedInstance.CustomUpgrades.Add (upgrade);

					var upgradeGroup = Upgrades.FirstOrDefault (g => g.Category == updatedUpgrade.Category);

					if (upgradeGroup == null) {
						upgradeGroup = new UpgradeGroup (updatedUpgrade.Category);
						Upgrades.Add (upgradeGroup);
					}

					upgradeGroup.Add (updatedUpgrade);

					Navigation.RemoveAsync <EditUpgradeViewModel> (vm);
					Upgrades = new ObservableCollection <UpgradeGroup> (Upgrades);
					MessagingCenter.Unsubscribe <EditUpgradeViewModel, Upgrade> (this, "Finished Editing");
				});

				Navigation.PushAsync<EditUpgradeViewModel> ((vm, p) => {
					vm.Upgrade = upgrade.Copy ();
				});
			});
		}

		public string PageName { get { return "Upgrades"; } }

		ObservableCollection <UpgradeGroup> upgrades;
		public ObservableCollection <UpgradeGroup> Upgrades {
			get {
				return upgrades;
			}
			set {
				SetProperty (ref upgrades, value);
			}
		}

		RelayCommand createUpgrade;
		public RelayCommand CreateUpgrade {
			get {
				if (createUpgrade == null)
					createUpgrade = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", (vm, upgrade) => {
							var upgradeGroup = Upgrades.FirstOrDefault (g => g.Category == upgrade.Category);

							if (upgradeGroup == null) {
								upgradeGroup = new UpgradeGroup (upgrade.Category);
								Upgrades.Add (upgradeGroup);
							}

							upgradeGroup.Add (upgrade);

							Cards.SharedInstance.CustomUpgrades.Add (upgrade);

							Navigation.RemoveAsync <CreateUpgradeViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created");
						});

						Navigation.PushAsync <CreateUpgradeViewModel> ();
					});

				return createUpgrade;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			var factions = Cards.SharedInstance.AllFactions;

			List <Upgrade> upgrades = Cards.SharedInstance.CustomUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

			var allUpgradeGroups = new ObservableCollection<UpgradeGroup> ();
			foreach (var upgrade in upgrades) {
				var upgradeGroup = allUpgradeGroups.FirstOrDefault (g => g.Category == upgrade.Category);

				if (upgradeGroup == null) {
					upgradeGroup = new UpgradeGroup (upgrade.Category);
					allUpgradeGroups.Add (upgradeGroup);
				}

				upgradeGroup.Add (upgrade);
			}

			Upgrades = allUpgradeGroups;
		}
	}
}

