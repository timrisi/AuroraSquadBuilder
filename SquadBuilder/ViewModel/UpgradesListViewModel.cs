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
			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades.xml")));

			XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
			var factions = (from faction in factionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				}).ToList ();

			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			var customFactions = (from faction in customFactionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				});
			factions.AddRange (customFactions);

			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships.xml")));
			var ships = (
				from ship in shipsElement.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
				}).ToList ();

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			var customShips = (
				from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),

				}
			);
			ships.AddRange (customShips);

			var category = upgradesXml.Elements ("Category").FirstOrDefault (c => c.Attribute ("type").Value == upgradeType);

			if (category == null)
				return new ObservableCollection <Upgrade> ();
			
			var upgrades = (from upgrade in category.Elements ()
			                where (upgrade.Element ("Faction")?.Value == null || factionsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute ("id") == (string)upgrade.Element ("Faction"))?.Value == Pilot.Faction.Name)
			                where (upgrade.Element ("Ship")?.Value == null || ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value).Name == Pilot.Ship.Name)
			                where (upgrade.Element ("Name")?.Value != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
			                where (upgrade.Element ("Name")?.Value != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"))
			                select new Upgrade {
				Name = upgrade.Element ("Name")?.Value,
				Category = upgrade.Parent.Attribute ("type")?.Value,
				Cost = (int)upgrade.Element ("Cost"),
				Text = upgrade.Element ("Text")?.Value,
				Faction = factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
				Ship = ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
				PilotSkill = upgrade.Element ("PilotSkill") != null ? (int)upgrade.Element ("PilotSkill") : 0,
				Attack = upgrade.Element ("Attack") != null ? (int)upgrade.Element ("Attack") : 0,
				Agility = upgrade.Element ("Agility") != null ? (int)upgrade.Element ("Agility") : 0,
				Hull = upgrade.Element ("Hull") != null ? (int)upgrade.Element ("Hull") : 0,
				Shields = upgrade.Element ("Shields") != null ? (int)upgrade.Element ("Shields") : 0,
				SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool)upgrade.Element ("SecondaryWeapon") : false,
				Dice = upgrade.Element ("Dice") != null ? (int)upgrade.Element ("Dice") : 0,
				Range = upgrade.Element ("Range")?.Value,
				Unique = upgrade.Element ("Unique") != null ? (bool)upgrade.Element ("Unique") : false,
				Limited = upgrade.Element ("Limited") != null ? (bool)upgrade.Element ("Limited") : false,
				SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool)upgrade.Element ("SmallOnly") : false,
				LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool)upgrade.Element ("LargeOnly") : false,
				HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool)upgrade.Element ("HugeOnly") : false,
				Preview = upgrade.Element ("Preview") != null ? (bool)upgrade.Element ("Preview") : false,
				AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
				                                                         select upgr.Value).ToList ()),
				Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
				                                            select upgr.Value).ToList ())
			}).ToList ();
				
			if ((bool)Application.Current.Properties ["AllowCustom"]) {
				XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));
				var customCategory = customUpgradesXml.Elements ("Category").FirstOrDefault (c => c.Attribute ("type").Value == upgradeType);

				var customUpgrades = (from upgrade in customCategory.Elements ()
					where (string.IsNullOrEmpty (upgrade.Element ("Faction")?.Value) || factionsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute ("id") == (string)upgrade.Element ("Faction"))?.Value == Pilot.Faction.Name)
					where (string.IsNullOrEmpty (upgrade.Element ("Ship")?.Value) || ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value).Name == Pilot.Ship.Name)
						where (upgrade.Element ("Name")?.Value != "Autothrusters" || Pilot.Ship.Actions.Contains ("Boost"))
				                      where (upgrade.Element ("Name")?.Value != "Stygium Particle Accelerator" || Pilot.Ship.Actions.Contains ("Cloak"))
				                      select new Upgrade {
					Name = upgrade.Element ("Name")?.Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction = factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					Ship = ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
					PilotSkill = upgrade.Element ("PilotSkill") != null ? (int)upgrade.Element ("PilotSkill") : 0,
					Attack = upgrade.Element ("Attack") != null ? (int)upgrade.Element ("Attack") : 0,
					Agility = upgrade.Element ("Agility") != null ? (int)upgrade.Element ("Agility") : 0,
					Hull = upgrade.Element ("Hull") != null ? (int)upgrade.Element ("Hull") : 0,
					Shields = upgrade.Element ("Shields") != null ? (int)upgrade.Element ("Shields") : 0,
					SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool)upgrade.Element ("SecondaryWeapon") : false,
					Dice = upgrade.Element ("Dice") != null ? (int)upgrade.Element ("Dice") : 0,
					Range = upgrade.Element ("Range")?.Value,
					Unique = upgrade.Element ("Unique") != null ? (bool)upgrade.Element ("Unique") : false,
					Limited = upgrade.Element ("Limited") != null ? (bool)upgrade.Element ("Limited") : false,
					SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool)upgrade.Element ("SmallOnly") : false,
					LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool)upgrade.Element ("LargeOnly") : false,
					HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool)upgrade.Element ("HugeOnly") : false,
					Preview = upgrade.Element ("Preview") != null ? (bool)upgrade.Element ("Preview") : false,
					AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
					                                                         select upgr.Value).ToList ()),
					Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
					                                            select upgr.Value).ToList ())
				});
				
				upgrades.AddRange (customUpgrades);
			}

			upgrades = upgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost).ToList ();

			ObservableCollection <Upgrade> valid = new ObservableCollection<Upgrade> ();
			foreach (var upgrade in upgrades) {
				if (upgrade.Slots.Count () == 0) {
					valid.Add (upgrade);	
					continue;
				}

				var pilotUpgrades = new List <object> (Pilot.Upgrades);
				pilotUpgrades.Remove (upgrade.Category);

				bool isValid = true;
				foreach (var slot in upgrade.Slots) {
					if (pilotUpgrades.Contains (slot))
						pilotUpgrades.Remove (slot);
					else {
						isValid = false;
						break;
					}
				}
				if (isValid)
					valid.Add (upgrade);
			}

			if (Pilot.Ship.LargeBase)
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.HugeOnly && !u.SmallOnly));
			else if (Pilot.Ship.Huge)
				return new ObservableCollection<Upgrade> (valid.Where (u => !u.SmallOnly && !u.LargeOnly));
					
			return new ObservableCollection <Upgrade> (valid.Where (u => !u.LargeOnly && !u.HugeOnly));		
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

