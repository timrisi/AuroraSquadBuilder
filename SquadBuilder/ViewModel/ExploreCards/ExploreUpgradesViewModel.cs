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
	public class ExploreUpgradesViewModel : ViewModel
	{
		string upgradeType;
		public string UpgradeType {
			get { return upgradeType; }
			set {
				upgradeType = value;
				Upgrades = GetUpgrades (value);
			}
		}

		ObservableCollection<Upgrade> upgrades = new ObservableCollection<Upgrade> ();
		public ObservableCollection<Upgrade> Upgrades {
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

		ObservableCollection<Upgrade> GetUpgrades (string type)
		{
			var upgrades = Cards.SharedInstance.Upgrades.Where (u => u != null)
				.Where (u => u.Category == type)
				.Where (u => !u.IsCustom || Settings.AllowCustom)
				.Where (u => !u.CCL || Settings.CustomCardLeague)
                .Where (u => !u.HotAC || Settings.IncludeHotac).ToList ();

			if (Settings.AllowCustom) {
				var customUpgrades = Cards.SharedInstance.CustomUpgrades
					.Where (u => u.Category == type).ToList ();

				upgrades.AddRange (customUpgrades);
			}

			return new ObservableCollection <Upgrade> (upgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ());
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

