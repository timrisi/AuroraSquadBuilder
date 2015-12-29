﻿using System;
using XLabs.Forms.Mvvm;
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

		ObservableCollection <UpgradeGroup> getAllUpgrades ()
		{
			var allUpgradeGroups = new List <UpgradeGroup>();

			var upgradeList = Cards.SharedInstance.Upgrades.ToList ();

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

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			allUpgrades = getAllUpgrades ();

			UpgradeGroups = new ObservableCollection <UpgradeGroup> (allUpgrades);
		}

		public override void OnViewDisappearing ()
		{
			base.OnViewDisappearing ();

			Cards.SharedInstance.SaveCollection ();
		}
	}
}