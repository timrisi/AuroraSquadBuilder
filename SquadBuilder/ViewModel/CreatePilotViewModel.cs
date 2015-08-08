using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using XLabs;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CreatePilotViewModel : ViewModel
	{
		public CreatePilotViewModel ()
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
			factions.RemoveAll (f => f.Name == "Mixed");

			Factions = new ObservableCollection<Faction> (factions);

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

			Ships = new ObservableCollection <Ship> (ships);
		}

		string name;
		public string Name {
			get { return Pilot.Name; }
			set {
				Pilot.Name = value;

				char[] arr = Name.ToCharArray();

				arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
				Pilot.Id = new string(arr).ToLower ();
				NotifyPropertyChanged ("Name");
			}
		}

		Pilot pilot = new Pilot ();
		public Pilot Pilot {
			get { return pilot; }
			set {
				SetProperty (ref pilot, value); 
				AstromechDroidSlots = pilot.UpgradeTypes.Count (u => u == "Astromech Droid");
				BombSlots = pilot.UpgradeTypes.Count (u => u == "Bomb");
				CannonSlots = pilot.UpgradeTypes.Count (u => u == "Cannon");
				CargoSlots = pilot.UpgradeTypes.Count (u => u == "Cargo");
				CrewSlots = pilot.UpgradeTypes.Count (u => u == "Crew");
				EPTSlots = pilot.UpgradeTypes.Count (u => u == "Elite Pilot Talent");
				HardpointSlots = pilot.UpgradeTypes.Count (u => u == "Hardpoint");
				IllicitSlots = pilot.UpgradeTypes.Count (u => u == "Illicit");
				MissileSlots = pilot.UpgradeTypes.Count (u => u == "Missile");
				SalvagedAstromechSlots = pilot.UpgradeTypes.Count (u => u == "Salvaged Astromech");
				SystemUpgradeSlots = pilot.UpgradeTypes.Count (u => u == "System Upgrade");
				TeamSlots = pilot.UpgradeTypes.Count (u => u == "Team");
				TorpedoSlots = pilot.UpgradeTypes.Count (u => u == "Torpedo");
				TurretWeaponSlots = pilot.UpgradeTypes.Count (u => u == "Turret Weapon");
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set { 
				SetProperty (ref shipIndex, value); 
				Pilot.Ship = Ships [ShipIndex];
			}
		}

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { return ships; }
			set { SetProperty (ref ships, value); }
		}

		int factionIndex;
		public int FactionIndex {
			get { return factionIndex; }
			set { 
				SetProperty (ref factionIndex, value); 

				Pilot.Faction = Factions [FactionIndex];
			}
		}

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get { return factions; }
			set { SetProperty (ref factions, value); }
		}

		int astromechDroidSlots;
		public int AstromechDroidSlots {
			get { return astromechDroidSlots; }
			set {
				SetProperty (ref astromechDroidSlots, value);
			}
		}

		int bombSlots;
		public int BombSlots {
			get { return bombSlots; }
			set { SetProperty (ref bombSlots, value); }
		}

		int cannonSlots;
		public int CannonSlots {
			get { return cannonSlots; }
			set { SetProperty (ref cannonSlots, value); }
		}

		int cargoSlots;
		public int CargoSlots {
			get { return cargoSlots; }
			set { SetProperty (ref cargoSlots, value); }
		}

		int crewSlots;
		public int CrewSlots {
			get { return crewSlots; }
			set { SetProperty (ref crewSlots, value); }
		}

		int eptSlots;
		public int EPTSlots {
			get { return eptSlots; }
			set { SetProperty (ref eptSlots, value); }
		}

		int hardpointSlots;
		public int HardpointSlots {
			get { return hardpointSlots; }
			set { SetProperty (ref hardpointSlots, value); }
		}

		int illicitSlots;
		public int IllicitSlots {
			get { return illicitSlots; }
			set { SetProperty (ref illicitSlots, value); }
		}

		int missileSlots;
		public int MissileSlots {
			get { return missileSlots; }
			set { SetProperty (ref missileSlots, value); }
		}

		int salvagedAstromechSlots;
		public int SalvagedAstromechSlots {
			get { return salvagedAstromechSlots; }
			set { SetProperty (ref salvagedAstromechSlots, value); }
		}

		int systemUpgradeSlots;
		public int SystemUpgradeSlots {
			get { return systemUpgradeSlots; }
			set { SetProperty (ref systemUpgradeSlots, value); }
		}

		int teamSlots;
		public int TeamSlots {
			get { return teamSlots; }
			set { SetProperty (ref teamSlots, value); }
		}

		int torpedoSlots;
		public int TorpedoSlots {
			get { return torpedoSlots; }
			set { SetProperty (ref torpedoSlots, value); }
		}

		int turretWeaponSlots;
		public int TurretWeaponSlots {
			get { return turretWeaponSlots; }
			set { SetProperty (ref turretWeaponSlots, value); }
		}

		RelayCommand savePilot;
		public RelayCommand SavePilot {
			get {
				if (savePilot == null)
					savePilot = new RelayCommand (() => {
						if (string.IsNullOrWhiteSpace (Name))
							return;

						XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots.xml")));
						XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

						if (pilotsXml.Elements ().FirstOrDefault (e => 
							e.Element ("Name").Value == Pilot.Name &&
							e.Element ("Faction").Value == Pilot.Faction.Id &&
							e.Element ("Ship").Value == Pilot.Ship.Id) != null)
							return;

						if (customPilotsXml.Elements ().FirstOrDefault (e => 
							e.Element ("Name").Value == Pilot.Name &&
							e.Element ("Faction").Value == Pilot.Faction.Id &&
							e.Element ("Ship").Value == Pilot.Ship.Id) != null) {

						} else {
							List <string> upgrades = new List<string> ();

							for (int i = 0; i < EPTSlots; i++)
								upgrades.Add ("Elite Pilot Talent");
							for (int i = 0; i < AstromechDroidSlots; i++)
								upgrades.Add ("Astromech Droid");
							for (int i = 0; i < BombSlots; i++)
								upgrades.Add ("Bomb");
							for (int i = 0; i < CannonSlots; i++)
								upgrades.Add ("Cannon");
							for (int i = 0; i < CargoSlots; i++)
								upgrades.Add ("Cargo");
							for (int i = 0; i < CrewSlots; i++)
								upgrades.Add ("Crew");
							for (int i = 0; i < HardpointSlots; i++)
								upgrades.Add ("Hardpoint");
							for (int i = 0; i < IllicitSlots; i++)
								upgrades.Add ("Illicit");
							for (int i = 0; i < MissileSlots; i++)
								upgrades.Add ("Missile");
							for (int i = 0; i < SalvagedAstromechSlots; i++)
								upgrades.Add ("Salvaged Astromech");
							for (int i = 0; i < SystemUpgradeSlots; i++)
								upgrades.Add ("System Upgrade");
							for (int i = 0; i < TeamSlots; i++)
								upgrades.Add ("Team");
							for (int i = 0; i < TorpedoSlots; i++)
								upgrades.Add ("Torpedo");
							for (int i = 0; i < TurretWeaponSlots; i++)
								upgrades.Add ("Turret Weapon");
							upgrades.Add ("Title");
							upgrades.Add ("Modification");

							var element = new XElement ("Pilot", 
								new XAttribute ("id", Pilot.Id),
								new XAttribute ("faction", Pilot.Faction.Id),
								new XAttribute ("ship", Pilot.Ship.Id),
								new XElement ("Name", Pilot.Name),
								new XElement ("Unique", Pilot.Unique),
								new XElement ("PilotSkill", Pilot.BasePilotSkill),
								new XElement ("Attack", Pilot.BaseAttack),
								new XElement ("Agility", Pilot.BaseAgility),
								new XElement ("Hull", Pilot.BaseHull),
								new XElement ("Shields", Pilot.BaseShields),
								new XElement ("Cost", Pilot.BaseCost),
								new XElement ("Ability", Pilot.Ability),
								new XElement ("Upgrades", 
									from upgrade in upgrades
									select new XElement ("Upgrade", upgrade)),
								new XElement ("Custom", true)
							);

							customPilotsXml.Add (element);

							DependencyService.Get <ISaveAndLoad> ().SaveText ("Pilots_Custom.xml", customPilotsXml.ToString ());

							MessagingCenter.Send <CreatePilotViewModel, Pilot> (this, "Pilot Created", Pilot);
						}



//						var element = new XElement ("Upgrade",
//							new XAttribute ("id", Upgrade.Id),
//							new XElement ("Name", Name),
//							new XElement ("Cost", Cost),
//							new XElement ("Text", Text),
//							new XElement ("Unique", IsUnique),
//							new XElement ("Limited", IsLimited),
//							new XElement ("Ship", Upgrade.Ship),
//							new XElement ("Faction", Upgrade.Faction.Id),
//							new XElement ("SmallOnly", SmallOnly),
//							new XElement ("LargeOnly", LargeOnly),
//							new XElement ("HugeOnly", HugeOnly),
//							//							new XElement ("ExtraSlots", 
//							//								new XElement ("Upgrade", )
//							//							),
//							//							new XElement ("AdditionalUpgrades", 
//							//								new XElement ("Upgrade", )
//							//							),
//							new XElement ("PilotSkill", PilotSkill),
//							new XElement ("Attack", Attack),
//							new XElement ("Agility", Agility),
//							new XElement ("Hull", Hull),
//							new XElement ("Shields", Shields),
//							new XElement ("SecondaryWeapon", SecondaryWeapon),
//							new XElement ("Dice", Dice),
//							new XElement ("Range", Range)
//						);
//
//						customPilotsXml.Elements ().FirstOrDefault (e => e.Attribute ("type")?.Value == PilotUpgradeTypes [UpgradeTypeIndex])?.Add (element);
//						DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customPilotsXml.ToString ());
//						//
//						MessagingCenter.Send <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", Upgrade);
					});

				return savePilot;
			}
		}
	}
}

