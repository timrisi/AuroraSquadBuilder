using System;

using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class UpgradesCollectionViewModel : ViewModel
	{
		ObservableCollection <UpgradeGroup> allUpgrades;

		public UpgradesCollectionViewModel ()
		{
			allUpgrades = getAllUpgrades ();

			UpgradeGroups = new ObservableCollection <UpgradeGroup> (allUpgrades);
		}

		public string PageName { get { return "Upgrades Owned"; } }

		ObservableCollection <UpgradeGroup> upgradeGroups = new ObservableCollection <UpgradeGroup> ();
		public ObservableCollection <UpgradeGroup> UpgradeGroups {
			get {
				return upgradeGroups;
			}
			set {
				SetProperty (ref upgradeGroups, value);
			}
		}

		string category;
		public string Category {
			get { return category; }
			set { 
				SetProperty (ref category, value); 
				filterUpgrades ();
			}
		}

		ObservableCollection <UpgradeGroup> getAllUpgrades ()
		{
			var allUpgradeGroups = new List <UpgradeGroup>();

			var upgradeList = Upgrade.Upgrades.Where (u => !u.CCL && !u.HotAC).ToList ();

			foreach (var upgrade in upgradeList) {
				var upgradeGroup = allUpgradeGroups.FirstOrDefault (g => g.Category == upgrade.Category);

				if (upgradeGroup == null) {
					upgradeGroup = new UpgradeGroup (upgrade.Category);
					allUpgradeGroups.Add (upgradeGroup);
				}

				upgradeGroup.Add (upgrade);
			}

			return new ObservableCollection <UpgradeGroup> (allUpgradeGroups.OrderBy (g => g.Category));
		}

		void filterUpgrades ()
		{
			UpgradeGroups.Clear ();

			var filteredUpgradeGroups = allUpgrades.Where (u => Category == null || u.Category == Category).ToList ();

			foreach (var upgradeGroup in filteredUpgradeGroups)
				UpgradeGroups.Add (upgradeGroup);
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			allUpgrades = getAllUpgrades ();

			UpgradeGroups = new ObservableCollection <UpgradeGroup> (allUpgrades);
			filterUpgrades ();
		}
	}
}