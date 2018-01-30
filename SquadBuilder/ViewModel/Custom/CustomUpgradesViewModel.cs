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
					Upgrade.CustomUpgrades.Remove (upgrade);
				}
			});

			MessagingCenter.Subscribe <Upgrade> (this, "Edit Upgrade", upgrade => {
				MessagingCenter.Subscribe <EditUpgradeViewModel, Upgrade> (this, "Finished Editing", (vm, updatedUpgrade) => {
					Upgrade.CustomUpgrades [Upgrade.CustomUpgrades.IndexOf (upgrade)] = updatedUpgrade;

					var originalGroup = Upgrades.FirstOrDefault (g => g.ToList ().Exists (u => u.Name == upgrade.Name));
					var upgradeGroup = Upgrades.FirstOrDefault (g => g.Category == updatedUpgrade.Category);

					if (originalGroup != null && upgradeGroup == originalGroup)
						upgradeGroup [upgradeGroup.IndexOf (upgrade)] = updatedUpgrade;
					else if (upgradeGroup != null) {
						originalGroup.Remove (upgrade);

						if (originalGroup.Count == 0)
							Upgrades.Remove (originalGroup);

						if (upgradeGroup != null)
							upgradeGroup.Add (updatedUpgrade);
					}

					if (upgradeGroup == null) {
						upgradeGroup = new UpgradeGroup (updatedUpgrade.Category);
						Upgrades.Add (upgradeGroup);
						upgradeGroup.Add (updatedUpgrade);
					}

					Navigation.RemoveAsync <EditUpgradeViewModel> (vm);
					Upgrades = new ObservableCollection <UpgradeGroup> (Upgrades);
					MessagingCenter.Unsubscribe <EditUpgradeViewModel, Upgrade> (this, "Finished Editing");
				});

				Navigation.PushAsync<EditUpgradeViewModel> ((vm, p) => {
					vm.Upgrade = upgrade.Copy ();
					vm.Create = false;
				});
			});
		}

		public string PageName { get { return "Upgrades"; } }

		ObservableCollection<UpgradeGroup> upgrades = new ObservableCollection<UpgradeGroup> ();
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
						MessagingCenter.Subscribe<EditUpgradeViewModel, Upgrade>(this, "Upgrade Created", (vm, upgrade) => {
							var upgradeGroups = new ObservableCollection<UpgradeGroup> (Upgrades);
							var upgradeGroup = upgradeGroups.FirstOrDefault (g => g.Category == upgrade.Category);

							if (upgradeGroup == null) {
								upgradeGroup = new UpgradeGroup (upgrade.Category);
								upgradeGroups.Add (upgradeGroup);
							}

							upgradeGroup.Add (upgrade);

							Upgrades = new ObservableCollection<UpgradeGroup> (upgradeGroups);

							Upgrade.CustomUpgrades.Add (upgrade);
							Upgrade.GetAllUpgrades ();
							Navigation.RemoveAsync <EditUpgradeViewModel> (vm);
							MessagingCenter.Unsubscribe<EditUpgradeViewModel, Upgrade>(this, "Upgrade Created");
						});

						Navigation.PushAsync<EditUpgradeViewModel>((vm, p) => {
							vm.Upgrade = new Upgrade();
							vm.Create = true;
						});
						//MessagingCenter.Subscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", (vm, upgrade) => {
						//	var upgradeGroup = Upgrades.FirstOrDefault (g => g.Category == upgrade.Category);

						//	if (upgradeGroup == null) {
						//		upgradeGroup = new UpgradeGroup (upgrade.Category);
						//		Upgrades.Add (upgradeGroup);
						//	}

						//	upgradeGroup.Add (upgrade);

						//	Upgrade.CustomUpgrades.Add (upgrade);
						//	Upgrade.GetAllUpgrades ();
						//	Navigation.RemoveAsync <CreateUpgradeViewModel> (vm);
						//	MessagingCenter.Unsubscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created");
						//});

						//Navigation.PushAsync <CreateUpgradeViewModel> ();
					});

				return createUpgrade;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			MessagingCenter.Unsubscribe<EditUpgradeViewModel, Upgrade>(this, "Finished Editing");
			                                                         
			var factions = Faction.AllFactions;

			List <Upgrade> upgrades = Upgrade.CustomUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

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

