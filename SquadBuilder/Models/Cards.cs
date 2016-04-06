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

namespace SquadBuilder
{
	public class Cards : ObservableObject
	{
		public const string SquadronsFilename = "squadrons.xml";
		public const string FactionsFilename = "Factions.xml";
		public const string ShipsFilename = "Ships.xml";
		public const string PilotsFilename = "Pilots.xml";
		public const string UpgradesFilename = "Upgrades.xml";
		public const string ExpansionsFilename = "Expansions.xml";
		public const string SettingsFilename = "Settings.xml";
		public const string VersionsFilename = "Versions.xml";

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
			
			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.ShipsFilename)));
			ships = new ObservableCollection <Ship> ((
				from ship in shipsElement.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					Owned = ship.Element ("Owned") != null ? (int)ship.Element ("Owned") : 0
				}).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge)
			);

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			customShips = new ObservableCollection <Ship> ((
				from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					Owned = 0
				})
			);

			updateAllShips ();
		}

		public void GetAllPilots ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.PilotsFilename))
				return;
			
			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.PilotsFilename)));
			pilots = new ObservableCollection <Pilot> (from pilot in pilotsXml.Elements ()
			                                           select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				CanonicalName = pilot.Element ("CanonicalName")?.Value,
				Faction = Factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = Ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = false,
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
				Owned = pilot.Element ("Owned") != null ? (int)pilot.Element ("Owned") : 0
			});

			XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));
			customPilots = new ObservableCollection <Pilot> (from pilot in customPilotsXml.Elements ()
			                                                 select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				CanonicalName = pilot.Element ("CanonicalName")?.Value,
				Faction = allFactions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = allShips.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = (bool)pilot.Element ("Custom"),
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
				Owned = 0
			});

			updateAllPilots ();
		}

		public void GetAllUpgrades ()
		{
			if (!DependencyService.Get <ISaveAndLoad> ().FileExists (Cards.UpgradesFilename))
				return;
			
			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.UpgradesFilename)));
			List <Upgrade> allUpgrades = new List <Upgrade> ();

			foreach (var category in upgradesXml.Elements ()) {
				var categoryUpgrades = (from upgrade in category.Elements ()
				                        select new Upgrade {
					Id = upgrade.Attribute ("id").Value,
					Name = upgrade.Element ("Name")?.Value,
					CanonicalName = upgrade.Element ("CanonicalName")?.Value,
					CategoryId = upgrade.Parent.Attribute ("id").Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction = Factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					Ship = Ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
					ShipRequirement = upgrade.Element ("ShipRequirement")?.Value,
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
					                                           select upgr.Value).ToList ()),
					RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List <XElement> ()
																		 select upgr.Value).ToList ()),
					RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List <XElement> ()
																	   select upgr.Value).ToList ()),
					RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
					Owned = upgrade.Element ("Owned") != null ? (int)upgrade.Element ("Owned") : 0,
					MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int)upgrade.Element ("MinPilotSkill") : 0
				});

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
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction = allFactions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					Ship = allShips.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
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
					                                            select upgr.Value).ToList ()),
					RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List <XElement> ()
																		 select upgr.Value).ToList ()),
					RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List <XElement> ()
																	   select upgr.Value).ToList ()),
					RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
					Owned = 0,
					MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int)upgrade.Element ("MinPilotSkill") : 0
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
					owned = (int)expansion.Element ("Owned")
			});
		}

		public void GetAllSquadrons ()
		{
			var service = DependencyService.Get <ISaveAndLoad> ();

			if (!service.FileExists (SquadronsFilename)) {
				Squadrons = new ObservableCollection <Squadron> ();
				return;
			}

			var serializedXml = service.LoadText (SquadronsFilename);
			var serializer = new XmlSerializer (typeof(ObservableCollection <Squadron>));

			using (TextReader reader = new StringReader (serializedXml)) {
				var squads = (ObservableCollection <Squadron>)serializer.Deserialize (reader);

				foreach (var squad in squads) {
					squad.Faction = AllFactions.FirstOrDefault (f => f.Id == squad.Faction?.Id);

					foreach (var pilot in squad.Pilots) {
						foreach (var upgrade in pilot.UpgradesEquipped) {
							if (upgrade == null)
								continue;

							if (upgrade.CategoryId == null)
								upgrade.CategoryId = AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id && u.Category == upgrade.Category)?.CategoryId;
						}
					}
				}

				Squadrons = squads;
			}
		}

		public async Task SaveSquadrons ()
		{
			if (Squadrons.Count == 0)
				DependencyService.Get <ISaveAndLoad> ().DeleteFile (SquadronsFilename);
			
			var serializer = new XmlSerializer (typeof (ObservableCollection <Squadron>));
			using (var stringWriter = new StringWriter ()) {
				serializer.Serialize (stringWriter, Squadrons);
				string serializedXML = stringWriter.ToString ();

				var service = DependencyService.Get <ISaveAndLoad> ();

				if (!service.FileExists (SquadronsFilename) || DependencyService.Get <ISaveAndLoad> ().LoadText (SquadronsFilename) != serializedXML) {
				DependencyService.Get <ISaveAndLoad> ().SaveText (SquadronsFilename, serializedXML);
					//Application.Current.Properties [SettingsViewModel.ModifiedDateKey] = DateTime.Now;
					App.Storage.Put<DateTime> (SettingsViewModel.ModifiedDateKey, DateTime.Now);

					if (App.DropboxClient != null && Squadrons.Count > 0)
						await SettingsViewModel.SaveToDropbox ();
				}
			}
		}

		public void SaveCollection ()
		{
			XElement expansionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.ExpansionsFilename)));
			XElement shipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.ShipsFilename)));
			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.PilotsFilename)));
			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.UpgradesFilename)));

			foreach (var expansion in Expansions) {
				var element = expansionsXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == expansion.Id);

				if (element == null)
					continue;

				element.SetElementValue ("Owned", expansion.Owned);
			}

			foreach (var ship in Ships) {
				var element = shipsXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == ship.Id);

				if (element == null)
					continue;

				element.SetElementValue ("Owned", ship.Owned);
			}

			foreach (var pilot in Pilots) {
				var element = pilotsXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == pilot.Id && e.Attribute ("ship").Value == pilot.Ship.Id && e.Attribute ("faction").Value == pilot.Faction.Id);

				if (element == null)
					continue;

				element.SetElementValue ("Owned", pilot.Owned);
			}

			foreach (var upgrade in Upgrades) {
				var element = upgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == upgrade.Id && e.Parent.Attribute ("id").Value == upgrade.CategoryId);

				if (element == null)
					continue;

				element.SetElementValue ("Owned", upgrade.Owned);
			}

			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.ExpansionsFilename, expansionsXml.ToString ());
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.ShipsFilename, shipsXml.ToString ());
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.PilotsFilename, pilotsXml.ToString ());
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.UpgradesFilename, upgradesXml.ToString ());

			Cards.SharedInstance.GetAllExpansions ();
		}
	}
}