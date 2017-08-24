using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using Dropbox.Api.Files;
using System.Threading.Tasks;
using System.Text;
using Dropbox.Api;
using Xamarin.Auth;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace SquadBuilder
{
	public class Cards : ObservableObject
	{
		public const string SquadronsFilename = "squadrons.xml";
		public const string XwcFilename = "squadrons.xwc";
		public const string FactionsFilename = "Factions.xml";
		public const string ShipsFilename = "Ships.xml";
		public const string PilotsFilename = "Pilots.xml";
		public const string UpgradesFilename = "Upgrades.xml";
		public const string ExpansionsFilename = "Expansions.xml";
		public const string SettingsFilename = "Settings.xml";
		public const string VersionsFilename = "Versions.xml";
		public const string ReferenceCardsFilename = "ReferenceCards.xml";

		public static Dictionary<string, string> CategoryToID = new Dictionary<string, string> {
			{ "Astromech Droid", "amd" },
			{ "Bomb", "bomb" },
			{ "Cannon", "cannon" },
			{ "Cargo", "cargo" },
			{ "Crew", "crew" },
			{ "Elite Pilot Talent", "ept" },
			{ "Hardpoint", "hardpoint" },
			{ "Illicit", "illicit" },
			{ "Missile", "missile" },
			{ "Modification", "mod" },
			{ "Salvaged Astromech", "samd" },
			{ "System Upgrade", "system" },
			{ "Team", "team" },
			{ "Title", "title" },
			{ "Torpedo", "torpedo" },
			{ "Turret Weapon", "turret" },
			{ "Tech", "tech" }
		};

		public Cards ()
		{
			MessagingCenter.Subscribe <App> (this, "Save Squadrons", app => SaveSquadrons ());
		}

		static Cards sharedInstance;
		public static Cards SharedInstance {
			get {
				if (sharedInstance == null)
					sharedInstance = new Cards ();

				return sharedInstance;
			}
		}

		Squadron currentSquadron;
		public Squadron CurrentSquadron {
			get { return currentSquadron; }
			set { SetProperty (ref currentSquadron, value); }
		}

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get { 
				if (factions == null)
					GetAllFactions ();
				
				return factions; }
			set { 
				SetProperty (ref factions, value); 
				factions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		ObservableCollection <Faction> customFactions;
		public ObservableCollection <Faction> CustomFactions {
			get { 
				if (customFactions == null)
					GetAllFactions ();
				
				return customFactions; }
			set { 
				SetProperty (ref customFactions, value); 
				customFactions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		ObservableCollection <Faction> allFactions;
		public ObservableCollection <Faction> AllFactions {
			get {
				if (allFactions == null)
					updateAllFactions ();

				return allFactions;
			}
		}

		void updateAllFactions ()
		{
			var temp = Factions.ToList ();
			temp.AddRange (customFactions);
			allFactions = new ObservableCollection <Faction> (temp);
		}

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { 
				if (ships == null)
					GetAllShips ();

				return ships; }
			set { 
				SetProperty (ref ships, value);
				ships.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		ObservableCollection <Ship> customShips;
		public ObservableCollection <Ship> CustomShips {
			get {
				if (customShips == null)
					GetAllShips ();
				
				return customShips; }
			set { 
				SetProperty (ref customShips, value);
				customShips.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		ObservableCollection <Ship> allShips;
		public ObservableCollection <Ship> AllShips {
			get {
				if (allShips == null)
					updateAllShips ();

				return allShips;
			}
		}

		void updateAllShips ()
		{
			var temp = Ships.ToList ();
			temp.AddRange (customShips);
			allShips = new ObservableCollection <Ship> (temp);
		}

		ObservableCollection <Pilot> pilots;
		public ObservableCollection <Pilot> Pilots {
			get {
				if (pilots == null)
					GetAllPilots ();

				return pilots; }
			set { 
				SetProperty (ref pilots, value); 
				pilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		ObservableCollection <Pilot> customPilots;
		public ObservableCollection <Pilot> CustomPilots {
			get { 
				if (customPilots == null)
					GetAllPilots ();
				
				return customPilots; }
			set {
				SetProperty (ref customPilots, value);
				customPilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		ObservableCollection <Pilot> allPilots;
		public ObservableCollection <Pilot> AllPilots {
			get {
				if (allPilots == null)
					updateAllPilots ();

				return allPilots;
			}
		}

		void updateAllPilots ()
		{
			var temp = Pilots.ToList ();
			temp.AddRange (customPilots);
			allPilots = new ObservableCollection <Pilot> (temp);
		}

		ObservableCollection <Upgrade> upgrades;
		public ObservableCollection <Upgrade> Upgrades {
			get {
				if (upgrades == null)
					GetAllUpgrades ();

				return upgrades; }
			set { 
				SetProperty (ref upgrades, value);
				upgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		ObservableCollection <Upgrade> customUpgrades;
		public ObservableCollection <Upgrade> CustomUpgrades {
			get {
				if (customUpgrades == null)
					GetAllUpgrades ();
				
				return customUpgrades; }
			set { 
				SetProperty (ref customUpgrades, value);
				customUpgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		ObservableCollection <Upgrade> allUpgrades;
		public ObservableCollection <Upgrade> AllUpgrades {
			get {
				if (allUpgrades == null)
					updateAllUpgrades ();

				return allUpgrades;
			}
		}

		void updateAllUpgrades ()
		{
			var temp = Upgrades.ToList ();
			temp.AddRange (customUpgrades);
			allUpgrades = new ObservableCollection <Upgrade> (temp);
		}

		ObservableCollection <Expansion> expansions;
		public ObservableCollection <Expansion> Expansions {
			get { 
				if (expansions == null)
					GetAllExpansions ();
				
				return expansions; }
			set { SetProperty (ref expansions, value); }
		}

		ObservableCollection <Squadron> squadrons;
		public ObservableCollection <Squadron> Squadrons {
			get {
				if (squadrons == null)
					GetAllSquadrons ();
				
				return squadrons; }
			set { SetProperty (ref squadrons, value); }
		}

		public void GetAllFactions ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.FactionsFilename))
				return;
			
			XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.FactionsFilename)));
			factions = new ObservableCollection <Faction> ((from faction in factionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					CanonicalName = faction.Element ("CanonicalName")?.Value,
					OldId = faction.Element ("OldId")?.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				})
			);

			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			customFactions = new ObservableCollection <Faction> ((from faction in customFactionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					CanonicalName = faction.Element ("CanonicalName")?.Value,
					OldId = faction.Element ("OldId")?.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				})
			);

			updateAllFactions ();
		}

		public void GetAllShips ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.ShipsFilename))
				return;
			
			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var shipsCollectionElement = collectionXml.Element ("Ships");

			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.ShipsFilename)));
			ships = new ObservableCollection<Ship> ((
				from ship in shipsElement.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					OldId = ship.Element ("OldId")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection<string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					IsCustom = ship.Element ("Custom") != null ? (bool)ship.Element ("Custom") : false,
					CCL = ship.Element ("CCL") != null ? (bool)ship.Element ("CCL") : false,
					IsPreview = ship.Element ("Preview") != null ? (bool)ship.Element ("Preview") : false,
					ManeuverGridImage = ship.Element ("ManeuverGridImage")?.Value ?? "",
					Symbol = ship.Element ("Symbol")?.Value ?? "",
					//StraightOne = ship.Element("StraightOne")?.Value ?? "",
					//StraightTwo = ship.Element("StraightOne")?.Value ?? "",
					//StraightThree = ship.Element("StraightOne")?.Value ?? "",
					//StraightFour = ship.Element("StraightOne")?.Value ?? "",
					//StraightFive = ship.Element("StraightOne")?.Value ?? "",
					owned = shipsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == ship.Attribute ("id").Value) != null ?
							(int)shipsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == ship.Attribute ("id").Value) : 0
				}).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge)
			);

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			customShips = new ObservableCollection <Ship> ((
				from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					OldId = ship.Element ("OldId")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					IsPreview = ship.Element ("Preview") != null ? (bool)ship.Element ("Preview") : false,
					IsCustom = ship.Element ("Custom") != null ? (bool)ship.Element ("Custom") : false,
					CCL = false,
					ManeuverGridImage = "",
					owned = 0
				})
			);

			updateAllShips ();
		}

		public void GetAllPilots ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.PilotsFilename))
				return;

			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var pilotsCollectionElement = collectionXml.Element ("Pilots");

			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.PilotsFilename)));
			pilots = new ObservableCollection <Pilot> (from pilot in pilotsXml.Elements ()
			                                           select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				CanonicalName = pilot.Element ("CanonicalName")?.Value,
			    OldId = pilot.Element ("OldId")?.Value,
				Faction = Factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = Ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value)?.Copy (),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseEnergy = pilot.Element ("Energy") != null ? (int)pilot.Element ("Energy") : 0,
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = pilot.Element ("Custom") != null ? (bool)pilot.Element ("Custom") : false,
				CCL = pilot.Element ("CCL") != null ? (bool)pilot.Element ("CCL") : false,
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
				owned = pilotsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == pilot.Attribute ("id").Value) != null ?
							(int)pilotsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == pilot.Attribute ("id").Value) : 0
			});

			XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));
			customPilots = new ObservableCollection <Pilot> (from pilot in customPilotsXml.Elements ()
			                                                 select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				CanonicalName = pilot.Element ("CanonicalName")?.Value,
				OldId = pilot.Element ("OldId")?.Value,
				Faction = allFactions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = allShips.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value)?.Copy (),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseEnergy = pilot.Element ("Energy") != null ? (int)pilot.Element ("Energy") : 0,
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = (bool)pilot.Element ("Custom"),
				CCL = false,
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
				owned = 0
			});

			updateAllPilots ();
		}

		public void GetAllUpgrades ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.UpgradesFilename))
				return;

			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var upgradesElement = collectionXml.Element ("Upgrades");

			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.UpgradesFilename)));
			List <Upgrade> allUpgrades = new List <Upgrade> ();

			foreach (var category in upgradesXml.Elements ()) {
				var categoryUpgrades = (from upgrade in category.Elements ()
										select new Upgrade {
											Id = upgrade.Attribute ("id").Value,
											Name = upgrade.Element ("Name")?.Value,
											CanonicalName = upgrade.Element ("CanonicalName")?.Value,
											OldId = upgrade.Element ("OldId")?.Value,
											CategoryId = upgrade.Parent.Attribute ("id").Value,
											Category = upgrade.Parent.Attribute ("type")?.Value,
											Cost = (int)upgrade.Element ("Cost"),
											Text = upgrade.Element ("Text")?.Value,
											Factions = Factions.Where (f => (upgrade.Element ("Faction")?.Value?.Split (',')?.Contains (f.Id) ?? false)).ToList (),
											//Factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
											//Ship = Ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value)?.Copy (),
											ShipRequirement = upgrade.Element ("ShipRequirement")?.Value,
											PilotSkill = upgrade.Element ("PilotSkill") != null ? (int)upgrade.Element ("PilotSkill") : 0,
											Energy = upgrade.Element ("Energy") != null ? (int)upgrade.Element ("Energy") : 0,
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
											AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List<XElement> ()
																									select upgr.Value).ToList ()),
											AdditionalActions = new ObservableCollection<string> ((from action in upgrade.Element ("AdditionalActions") != null ? upgrade.Element ("AdditionalActions").Elements () : new List<XElement> ()
																		select action.Value).ToList ()),
											Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List<XElement> ()
																					   select upgr.Value).ToList ()),
											RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List<XElement> ()
																								 select upgr.Value).ToList ()),
											RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List<XElement> ()
																							   select upgr.Value).ToList ()),
											UpgradeOptions = new ObservableCollection<string> ((from upgr in upgrade.Element ("UpgradeOptions") != null ? upgrade.Element ("UpgradeOptions").Elements () : new List<XElement> ()
																								select upgr.Value).ToList ()),
											RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
											owned = upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == upgrade.Attribute ("id").Value) != null ?
											 (int)upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == upgrade.Attribute ("id").Value) : 0,
											MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int)upgrade.Element ("MinPilotSkill") : 0,
											MaxPilotSkill = upgrade.Element ("MaxPilotSkill") != null ? (int?)upgrade.Element ("MaxPilotSkill") : 0,
											MinAgility = upgrade.Element ("MinAgility") != null ? (int?)upgrade.Element ("MinAgility") : null,
											MaxAgility = upgrade.Element ("MaxAgility") != null ? (int?)upgrade.Element ("MaxAgility") : null,
											ShieldRequirement = upgrade.Element ("ShieldRequirement") != null ? (int?)upgrade.Element ("ShieldRequirement") : null,
											IsCustom = upgrade.Element ("Custom") != null ? (bool)upgrade.Element ("Custom") : false,
											CCL = upgrade.Element ("CCL") != null ? (bool)upgrade.Element ("CCL") : false,
											ModifiedManeuverDial = upgrade.Element ("ModifiedManeuverDial")?.Value,
											HotAC = bool.Parse (upgrade.Element ("HotAC")?.Value ?? "false"),
				});

				if (category.Attribute ("id").Value == "ept") {
					var hotacPilotEPTs = (from p in Pilots
					                      where p.Unique &&
					                      !categoryUpgrades.Any (u => u.Id == (p.CanonicalName + "pilot" + p.Expansions)) && 
					                      !p.CCL && !p.IsCustom
										  select new Upgrade {
											  Cost = p.PilotSkill,
											  Text = p.Ability,
											  Factions = p.Faction != null ? new List<Faction> { p.Faction } : null,
											  HotAC = true,
											  Category = "Elite Pilot Talent",
											  CategoryId = "ept",
											  Id = p.CanonicalName + "pilot" + p.Expansions,
											  Name = p.Name + " (Pilot)",
											  Unique = true,
						AdditionalUpgrades = new ObservableCollection<string> (),
						AdditionalActions = new ObservableCollection <string> (),
						UpgradeOptions = new ObservableCollection<string> (),
						RequiredSlots = new ObservableCollection<string> (),
						RemovedUpgrades = new ObservableCollection<string> (),
						Slots = new ObservableCollection<string> ()
					}).OrderBy (u => u.Cost).GroupBy (u => u.Text).Select (g => g.First ()).ToList ();

					var upgrades = categoryUpgrades.ToList ();
					upgrades.AddRange (hotacPilotEPTs);
					categoryUpgrades = upgrades.AsEnumerable ();
				}

				allUpgrades.AddRange (categoryUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));

			}

			upgrades = new ObservableCollection <Upgrade> (allUpgrades);

			XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml"))); 
			List <Upgrade> allCustomUpgrades = new List <Upgrade> ();

			foreach (var customCategory in customUpgradesXml.Elements ()) {

				var categoryCustomUpgrades = (from upgrade in customCategory.Elements ()
				                              select new Upgrade {
					Id = upgrade.Attribute ("id")?.Value,
					Name = upgrade.Element ("Name")?.Value,
					CanonicalName = upgrade.Element ("CanonicalName")?.Value,
					OldId = upgrade.Element ("OldId")?.Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					CategoryId = upgrade.Parent.Attribute ("id")?.Value ?? CategoryToID [upgrade.Parent.Attribute ("type")?.Value],
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Factions = allFactions.Where (f => (upgrade.Element ("Faction")?.Value?.Split (',')?.Contains (f.Id) ?? false)).ToList (),
					Ship = allShips.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value)?.Copy (),
					ShipRequirement = upgrade.Element ("ShipRequirement")?.Value,
					PilotSkill = upgrade.Element ("PilotSkill") != null ? (int)upgrade.Element ("PilotSkill") : 0,
					Energy = upgrade.Element ("Energy") != null ? (int)upgrade.Element ("Energy") : 0,
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
			     		AdditionalActions = new ObservableCollection<string> ((from action in upgrade.Element ("AdditionalActions") != null ? upgrade.Element ("AdditionalActions").Elements () : new List<XElement> ()
															     select action.Value).ToList ()),

		     			Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
					                                            select upgr.Value).ToList ()),
					RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List <XElement> ()
																		 select upgr.Value).ToList ()),
					RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List <XElement> ()
																	   select upgr.Value).ToList ()),
					RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
					UpgradeOptions = new ObservableCollection <string> (),
					owned = 0,
					MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int)upgrade.Element ("MinPilotSkill") : 0,
					MaxPilotSkill = upgrade.Element ("MaxPilotSkill") != null ? (int?)upgrade.Element ("MaxPilotSkill") : 0,
					MinAgility = upgrade.Element ("MinAgility") != null ? (int?)upgrade.Element ("MinAgility") : null,
					MaxAgility = upgrade.Element ("MaxAgility") != null ? (int?)upgrade.Element ("MaxAgility") : null,
					ShieldRequirement = upgrade.Element ("ShieldRequirement") != null ? (int?)upgrade.Element ("ShieldRequirement") : null,
					IsCustom = upgrade.Element ("Custom") != null ? (bool)upgrade.Element ("Custom") : false,
					CCL = upgrade.Element ("CCL") != null ? (bool)upgrade.Element ("CCL") : false,
				});

				allCustomUpgrades.AddRange (categoryCustomUpgrades);
			}

			customUpgrades = new ObservableCollection <Upgrade> (allCustomUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));

			updateAllUpgrades ();
		}

		public void GetAllExpansions ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.ExpansionsFilename))
				return;
			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var expansionsElement = collectionXml.Element ("Expansions");

			XElement expansionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.ExpansionsFilename)));
			expansions = new ObservableCollection <Expansion> (from expansion in expansionsXml.Elements ()
			                                                    select new Expansion {
					Id = expansion.Attribute ("id").Value,
					Name = expansion.Element ("Name").Value,
					Wave = expansion.Element ("Wave").Value,
					Ships = (from ship in expansion.Element ("Ships").Elements ()
						select ship.Value).ToList (),
					Pilots = (from pilot in expansion.Element ("Pilots").Elements ()
						select pilot.Value).ToList (),
					Upgrades = (from upgrade in expansion.Element ("Upgrades").Elements ()
						select upgrade.Value).ToList (),
					owned = expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == expansion.Attribute ("id").Value) != null ? 
				                             (int)expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == expansion.Attribute ("id").Value) : 0
			});
		}

		public void GetAllSquadrons ()
		{
			var service = DependencyService.Get<ISaveAndLoad> ();

			if (service.FileExists (XwcFilename)) {
				var xwcText = service.LoadText (XwcFilename);
				if (string.IsNullOrEmpty (xwcText)) {
					Squadrons = new ObservableCollection<Squadron> ();
					return;
				}

				var json = JObject.Parse (xwcText);


				if (json ["container"] == null)
					throw new ArgumentException ("Container key is missing");

				var squads = new List<Squadron> ();

				foreach (var squadXws in json ["container"])
					squads.Add (Squadron.FromXws (squadXws.ToString ()));

				Squadrons = new ObservableCollection<Squadron> (squads);
			} else if (service.FileExists (SquadronsFilename)) {
				var serializedXml = service.LoadText (SquadronsFilename);
				serializedXml.Replace ("<Owned>0</Owned>", "");
				serializedXml.Replace ("<owned>0</owned>", "");
				var serializer = new XmlSerializer (typeof (ObservableCollection<Squadron>));

				using (TextReader reader = new StringReader (serializedXml)) {
					var squads = (ObservableCollection<Squadron>)serializer.Deserialize (reader);

					foreach (var squad in squads) {
						squad.Faction = AllFactions.FirstOrDefault (f => f.Id == squad.Faction?.Id);

						foreach (var pilot in squad.Pilots) {
							pilot.Ship = AllShips.FirstOrDefault (f => f.Id == pilot.Ship.Id)?.Copy ();
							if (pilot.Ship.ManeuverGridImage == null) {
								pilot.Ship.ManeuverGridImage = "";
							}

							if (squad.Faction?.Id == "scum") {
								if (pilot.Id == "bobafett")
									pilot.Id = "bobafettscum";
								if (pilot.Id == "kathscarlet")
									pilot.Id = "kathscarletscum";
							}
							if (pilot.Id == "Ello Asty")
								pilot.Id = "elloasty";
							if (pilot.Id == "4lom")
								pilot.Id = "fourlom";

							if (CustomPilots.Any (p => p.Id == pilot.Id))
								pilot.IsCustom = true;

							pilot.Preview = AllPilots.FirstOrDefault (p => p.Id == pilot.Id)?.Preview ?? false;

							foreach (var upgrade in pilot.UpgradesEquipped) {
								if (upgrade == null)
									continue;

								upgrade.Preview = AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id)?.Preview ?? false;

								if (upgrade.Id == "r2d2" && upgrade.Category == "Crew")
									upgrade.Id = "r2d2crew";
								if (upgrade.Id == "4lom")
									upgrade.Id = "fourlom";

								if (upgrade.CategoryId == null)
									upgrade.CategoryId = AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id && u.Category == upgrade.Category)?.CategoryId;
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "emperorpalpatine") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "emperorpalpatine"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "wookieecommandos") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "wookieecommandos"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "unguidedrockets") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "unguidedrockets"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}
						}
					}

					Squadrons = squads;
				}
			} else {
				Squadrons = new ObservableCollection <Squadron> ();
				return;
			}
		}

		public async Task SaveSquadrons ()
		{
			var service = DependencyService.Get<ISaveAndLoad> ();

			if (Squadrons.Count == 0) {
				service.DeleteFile (XwcFilename);
				return;
			}

			var xwc = CreateXwc ();

			if (!service.FileExists (XwcFilename) || service.LoadText (XwcFilename) != xwc) {
				service.SaveText (XwcFilename, xwc);

				App.Storage.Put<DateTime> (SettingsViewModel.ModifiedDateKey, DateTime.Now);

				if (App.DropboxClient != null)
					await SettingsViewModel.SaveToDropbox ();
			}
		}

		public string CreateXwc ()
		{
			try {
				var squads = new JArray ();
				foreach (var s in Squadrons) {
					if (s == null)
						Console.WriteLine ("Foo");
					var squad = s.CreateXwsObject ();
					if (squad == null)
						continue;
					{ }
					squads.Add (squad);
				}

				var json = new JObject (
					new JProperty ("container", squads),
					new JProperty ("vendor",
				new JObject (
							new JProperty ("aurora",
					    new JObject (
									new JProperty ("builder", "Aurora Squad Builder"),
						    new JProperty ("builder_url", "https://itunes.apple.com/us/app/aurora-squad-builder/id1020767927?mt=8")
							    )
				    )
				    )
			)
				);

				return json.ToString ();
			}catch (Exception e) {
				return "";
			}
		}
	}
}