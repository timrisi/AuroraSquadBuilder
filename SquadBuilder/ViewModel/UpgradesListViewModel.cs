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

				if (value != null)
					MessagingCenter.Send <UpgradesListViewModel, Upgrade> (this, "Upgrade selected", value);
			}
		}

		ObservableCollection <Upgrade> GetUpgrades (string type)
		{
			XElement upgradesXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Upgrades.xml"));
			XElement factionsXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Factions.xml"));
			XElement shipsXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Ships.xml"));

			var category = upgradesXml.Elements ("Category").FirstOrDefault (c => c.Attribute ("type").Value == upgradeType);

			if (category == null)
				return new ObservableCollection <Upgrade> ();
			
			var upgrades = (from upgrade in category.Elements ()
							where (upgrade.Element ("Faction")?.Value == null || factionsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute ("id") == (string)upgrade.Element ("Faction"))?.Value == Pilot.Faction)
							where (upgrade.Element ("Ship")?.Value == null || shipsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute("id") == upgrade.Element ("Ship")?.Value)?.Element ("Name")?.Value == Pilot.Ship.Name)
							where (upgrade.Element ("Name")?.Value != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
							where (upgrade.Element ("Name")?.Value != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"))
							select new Upgrade {
					Name = upgrade.Element ("Name")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction = factionsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute ("id") == (string)upgrade.Element ("Faction"))?.Value,
					PilotSkill = upgrade.Element ("PilotSkill") != null ? (int) upgrade.Element ("PilotSkill") : 0,
					Attack = upgrade.Element ("Attack") != null ? (int) upgrade.Element ("Attack") : 0,
					Agility = upgrade.Element ("Agility") != null ? (int) upgrade.Element ("Agility") : 0,
					Hull = upgrade.Element ("Hull") != null ? (int) upgrade.Element ("Hull") : 0,
					Shields = upgrade.Element ("Shields") != null ? (int) upgrade.Element ("Shields") : 0,
					SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool) upgrade.Element ("SecondaryWeapon") : false,
					Dice = upgrade.Element ("Dice") != null ? (int) upgrade.Element ("Dice") : 0,
					Range = upgrade.Element ("Range")?.Value,
					Unique = upgrade.Element ("Unique") != null ? (bool) upgrade.Element ("Unique") : false,
					Limited = upgrade.Element ("Limited") != null ? (bool) upgrade.Element ("Limited") : false,
					SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool) upgrade.Element ("SmallOnly") : false,
					LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool) upgrade.Element ("LargeOnly") : false,
					HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool) upgrade.Element ("HugeOnly") : false,
					AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
						select upgr.Value).ToList ())
				}).OrderBy (u => u.Name).OrderBy (u => u.Cost);

			if (Pilot.Ship.LargeBase)
				return new ObservableCollection<Upgrade> (upgrades.Where (u => !u.HugeOnly && !u.SmallOnly));
			else if (Pilot.Ship.Huge)
				return new ObservableCollection<Upgrade> (upgrades.Where (u => !u.SmallOnly && !u.LargeOnly));
					
			return new ObservableCollection <Upgrade> (upgrades.Where (u => !u.LargeOnly && !u.HugeOnly));		
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

