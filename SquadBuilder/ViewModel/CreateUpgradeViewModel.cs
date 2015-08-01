using System;
using XLabs.Forms.Mvvm;
using XLabs;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CreateUpgradeViewModel : ViewModel
	{
		public CreateUpgradeViewModel ()
		{
			XElement element = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
			var factions = (from faction in element.Elements ()
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

			Factions = new ObservableCollection<Faction> (factions);

			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships.xml")));
			var ships = (
			     from ship in shipsElement.Elements ()
				 select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = (
					    from action in ship.Element ("Actions").Elements ()
					  select action.Value).ToList (),
			}).ToList ();

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			var customShips = (
            	from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = (
						from action in ship.Element ("Actions").Elements ()
						select action.Value).ToList (),

				}
        	);
			ships.AddRange (customShips);

			Ships = new ObservableCollection <Ship> (ships);

			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades.xml")));
			var upgrades = (
				from upgrade in upgradesXml.Elements ()
				select upgrade.Attribute ("type").Value
			);

			UpgradeTypes = new ObservableCollection<string> (upgrades);
		}

		string name;
		public string Name {
			get { return name; }
			set {
				SetProperty (ref name, value);
			}
		}

		int cost;
		public int Cost {
			get { return cost; }
			set {
				SetProperty (ref cost, value);
			}
		}

		string text;
		public string Text {
			get { return text; }
			set {
				SetProperty (ref text, value);
			}
		}

		bool isLimited;
		public bool IsLimited {
			get { return isLimited; }
			set {
				SetProperty (ref isLimited, value);
				if (value)
					IsUnique = false;
			}
		}

		bool isUnique;
		public bool IsUnique {
			get { return isUnique; }
			set {
				SetProperty (ref isUnique, value);
				if (value)
					IsLimited = false;
			}
		}

		bool smallOnly;
		public bool SmallOnly {
			get { return smallOnly; }
			set {
				SetProperty (ref smallOnly, value);
				if (value) {
					LargeOnly = false;
					HugeOnly = false;
				}
			}
		}

		bool largeOnly;
		public bool LargeOnly {
			get { return largeOnly; }
			set { 
				SetProperty (ref largeOnly, value); 
				if (value) {
					SmallOnly = false;
					HugeOnly = false;
				}
			}
		}

		bool hugeOnly;
		public bool HugeOnly {
			get { return hugeOnly; }
			set {
				SetProperty (ref hugeOnly, value);
				if (value) {
					SmallOnly = false;
					LargeOnly = false;
				}
			}
		}

		bool secondaryWeapon;
		public bool SecondaryWeapon {
			get { return secondaryWeapon; }
			set {
				SetProperty (ref secondaryWeapon, value);
			}
		}

		int dice;
		public int Dice {
			get { return dice; }
			set {
				SetProperty (ref dice, value);
			}
		}

		string range;
		public string Range {
			get { return range; }
			set {
				SetProperty (ref range, value);
			}
		}

		int pilotSkill;
		public int PilotSkill {
			get { return pilotSkill; }
			set {
				SetProperty (ref pilotSkill, value);
			}
		}

		int attack;
		public int Attack {
			get { return attack; }
			set {
				SetProperty (ref attack, value);
			}
		}

		int agility;
		public int Agility {
			get { return agility; }
			set {
				SetProperty (ref agility, value);
			}
		}

		int hull;
		public int Hull {
			get { return hull; }
			set {
				SetProperty (ref hull, value);
			}
		}

		int shields;
		public int Shields {
			get { return shields; }
			set {
				SetProperty (ref shields, value);
			}
		}

		int factionIndex;
		public int FactionIndex {
			get { return factionIndex; }
			set {
				SetProperty (ref factionIndex, value);
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set {
				SetProperty (ref shipIndex, value);
			}
		}

		int upgradeTypeIndex;
		public int UpgradeTypeIndex {
			get { return upgradeTypeIndex; }
			set {
				SetProperty (ref upgradeTypeIndex, value);
			}
		}

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get { return factions; }
			set {
				SetProperty (ref factions, value);
			}
		}

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { return ships; }
			set {
				SetProperty (ref ships, value);
			}
		}

		ObservableCollection <string> upgradeTypes;
		public ObservableCollection <string> UpgradeTypes {
			get { return upgradeTypes; }
			set {
				SetProperty (ref upgradeTypes, value);
			}
		}

		RelayCommand saveUpgrade;
		public RelayCommand SaveUpgrade {
			get {
				if (saveUpgrade == null)
					saveUpgrade = new RelayCommand (() => {
						if (string.IsNullOrWhiteSpace (Name))
							return;

						XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades.xml")));
						XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

						if (upgradesXml.Descendants ().FirstOrDefault (e => e?.Value == Name) != null)
							return;

						if (customUpgradesXml.Descendants ().FirstOrDefault (e => e.Value == Name) != null)
							return;
//
						char[] arr = Name.ToCharArray();
//
						arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
						var id = new string(arr);

						var element = new XElement ("Upgrade",
							new XAttribute ("id", id),
							new XElement ("Name", Name),
							new XElement ("Cost", Cost),
							new XElement ("Text", Text),
							new XElement ("Unique", IsUnique),
							new XElement ("Limited", IsLimited),
							new XElement ("Ship", ShipIndex > 0 ? Ships [ShipIndex - 1].Id : null),
							new XElement ("Faction", FactionIndex > 0 ? Factions [FactionIndex - 1].Id : null),
							new XElement ("SmallOnly", SmallOnly),
							new XElement ("LargeOnly", LargeOnly),
							new XElement ("HugeOnly", HugeOnly),
//							new XElement ("ExtraSlots", 
//								new XElement ("Upgrade", )
//							),
//							new XElement ("AdditionalUpgrades", 
//								new XElement ("Upgrade", )
//							),
							new XElement ("PilotSkill", PilotSkill),
							new XElement ("Attack", Attack),
							new XElement ("Agility", Agility),
							new XElement ("Hull", Hull),
							new XElement ("Shields", Shields),
							new XElement ("SecondaryWeapon", SecondaryWeapon),
							new XElement ("Dice", Dice),
							new XElement ("Range", Range)
						);

						customUpgradesXml.Elements ().FirstOrDefault (e => e.Attribute ("type")?.Value == UpgradeTypes [UpgradeTypeIndex])?.Add (element);
						DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customUpgradesXml.ToString ());
//
						MessagingCenter.Send <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", 
							new Upgrade {
								Id = id,
								Name = Name,
								Cost = Cost,
								Text = Text,
								Unique = IsUnique,
								Limited = IsLimited,
								Ship = ShipIndex > 0 ? Ships [ShipIndex - 1] : null,
								Faction = FactionIndex > 0 ? Factions [FactionIndex - 1] : null,
								SmallOnly = SmallOnly,
								LargeOnly = LargeOnly,
								HugeOnly = HugeOnly,
								PilotSkill = PilotSkill,
								Attack = Attack,
								Agility = Agility,
								Hull = Hull,
								Shields = Shields,
								SecondaryWeapon = SecondaryWeapon,
								Dice = Dice,
								Range = Range,
//								AdditionalUpgrades = 
//								Slots = 
							}
						);
					});

				return saveUpgrade;
			}
		}
	}
}

