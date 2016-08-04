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
			var factions = Cards.SharedInstance.AllFactions.ToList ();
			factions.RemoveAll (f => f.Name == "Mixed");
			Factions = new ObservableCollection<Faction> (factions);

			Ships = new ObservableCollection <Ship> (Cards.SharedInstance.AllShips);
		}

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
				ShipIndex = Ships.IndexOf (Ships.FirstOrDefault (s => s.Id == Pilot.Ship.Id));
				NotifyPropertyChanged ("ShipIndex");
				FactionIndex = Factions.IndexOf (Factions.FirstOrDefault (f => f.Id == Pilot.Faction.Id));
				NotifyPropertyChanged ("FactionIndex");
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set { 
				SetProperty (ref shipIndex, value); 
				if (value > -1)
					Pilot.Ship = Ships [value].Copy ();
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
				if (value > -1)
					Pilot.Faction = Factions [value];
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

		int modificationSlots = 1;
		public int ModificationSlots {
			get { return modificationSlots; }
			set { SetProperty (ref modificationSlots, value); }
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

		int techSlots;
		public int TechSlots {
			get { return techSlots; }
			set { SetProperty (ref techSlots, value); }
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
		
						if (Pilot.Ship == null || Pilot.Faction == null)
							return;
						
						XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

						if (Cards.SharedInstance.Pilots.Count (p => p.Name == Pilot.Name && p.Faction.Id == Pilot.Faction.Id && p.Ship.Id == Pilot.Ship.Id) > 0)
							return;

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
						for (int i = 0; i < ModificationSlots; i++)
							upgrades.Add ("Modification");
						for (int i = 0; i < SalvagedAstromechSlots; i++)
							upgrades.Add ("Salvaged Astromech");
						for (int i = 0; i < SystemUpgradeSlots; i++)
							upgrades.Add ("System Upgrade");
						for (int i = 0; i < TeamSlots; i++)
							upgrades.Add ("Team");
						for (int i = 0; i < techSlots; i++)
							upgrades.Add ("Tech");
						for (int i = 0; i < TorpedoSlots; i++)
							upgrades.Add ("Torpedo");
						for (int i = 0; i < TurretWeaponSlots; i++)
							upgrades.Add ("Turret Weapon");
						upgrades.Add ("Title");

						Pilot.UpgradeTypes = new ObservableCollection<string> (upgrades);

						var customElement = customPilotsXml.Elements ().FirstOrDefault (e => 
							e.Element ("Name")?.Value == Pilot.Name &&
							e.Attribute ("faction")?.Value == Pilot.Faction.Id &&
							e.Attribute ("ship")?.Value == Pilot.Ship.Id);
						
						if (customElement != null) {
							customElement.SetElementValue ("Unique", Pilot.Unique);
							customElement.SetElementValue ("PilotSkill", Pilot.BasePilotSkill);
							customElement.SetElementValue ("Attack", Pilot.BaseAttack);
							customElement.SetElementValue ("Agility", Pilot.BaseAgility);
							customElement.SetElementValue ("Hull", Pilot.Hull);
							customElement.SetElementValue ("Shields", Pilot.BaseShields);
							customElement.SetElementValue ("Cost", Pilot.BaseCost);
							customElement.SetElementValue ("Ability", Pilot.Ability);
							customElement.SetElementValue ("Upgrades",
								from upgrade in upgrades
								select new XElement ("Upgrade", upgrade));

							DependencyService.Get <ISaveAndLoad> ().SaveText ("Pilots_Custom.xml", customPilotsXml.ToString ());

							MessagingCenter.Send <CreatePilotViewModel, Pilot> (this, "Finished Editing", Pilot);
						} else {
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
					});

				return savePilot;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			var factions = Cards.SharedInstance.AllFactions.ToList ();
			factions.RemoveAll (f => f.Name == "Mixed");
			Factions = new ObservableCollection<Faction> (factions);

			Ships = new ObservableCollection <Ship> (Cards.SharedInstance.AllShips);
		}
	}
}

