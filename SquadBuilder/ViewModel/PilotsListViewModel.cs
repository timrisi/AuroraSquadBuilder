using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs;
using System.IO;

namespace SquadBuilder
{
	public class PilotsListViewModel : ViewModel
	{
		ObservableCollection <PilotGroup> allPilots;

		public PilotsListViewModel ()
		{
			allPilots = GetAllPilots ();

			PilotGroups = new ObservableCollection <PilotGroup> (allPilots);
		}

		public string PageName { get { return "Pilots"; } }

		Faction faction;
		public Faction Faction {
			get { return faction; }
			set { 
				SetProperty (ref faction, value); 
				if (value != null)
					filterPilots ();
			}
		}

		ObservableCollection <PilotGroup> pilotGroups = new ObservableCollection <PilotGroup> ();
		public ObservableCollection <PilotGroup> PilotGroups {
			get {
				return pilotGroups;
			}
			set {
				SetProperty (ref pilotGroups, value);
			}
		}

		Pilot selectedPilot = null;
		public Pilot SelectedPilot {
			get { return selectedPilot; }
			set { 
				SetProperty (ref selectedPilot, value); 

				if (value != null)
					MessagingCenter.Send <PilotsListViewModel, Pilot> (this, "Pilot selected", SelectedPilot);
			}
		}

		ObservableCollection <PilotGroup> GetAllPilots ()
		{
			var allPilotGroups = new ObservableCollection <PilotGroup>();

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

			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots.xml")));

			var allPilots = new ObservableCollection <Pilot> (from pilot in pilotsXml.Elements ()
			                                                  select new Pilot {
				Name = pilot.Element ("Name").Value,
				Faction = factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
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
			}).ToList ();

			if ((bool)Application.Current.Properties ["AllowCustom"]) {
				XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

				var customPilots = new ObservableCollection <Pilot> (from pilot in customPilotsXml.Elements ()
					select new Pilot {
						Id = pilot.Attribute ("id").Value,
						Name = pilot.Element ("Name").Value,
						Faction = factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
						Ship = ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
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
						Preview = pilot.Element ("Preview") != null ? (bool) pilot.Element ("Preview") : false,
					});

				allPilots.AddRange (customPilots);
			}

			foreach (var pilot in allPilots) {
				while (pilot.UpgradesEquipped.Count () < pilot.UpgradeTypes.Count ())
					pilot.UpgradesEquipped.Add (null);

				if (pilot.Ship == null || pilot.Faction == null)
					Console.WriteLine ("Foo");
				
				var pilotGroup = allPilotGroups.FirstOrDefault (g => g.Ship?.Name == pilot.Ship?.Name && g.Faction.Id == pilot.Faction.Id);

				if (pilotGroup == null) {
					pilotGroup = new PilotGroup (pilot.Ship) { Faction = pilot.Faction };
					allPilotGroups.Add (pilotGroup);
				}

				pilotGroup.Add (pilot);
			}

			return allPilotGroups;
		}

		string searchText;
		public string SearchText {
			get { return searchText; }
			set { 
				SetProperty (ref searchText, value); 

				SearchPilots (value);
			}
		}

		void filterPilots ()
		{
			PilotGroups.Clear ();

			var filteredPilotGroups = allPilots.Where (p => Faction.Name != "Mixed" ? p.Faction.Name == Faction.Name : p != null).ToList ();

			foreach (var pilotGroup in filteredPilotGroups)
				PilotGroups.Add (pilotGroup);
		}

		public void SearchPilots (string text)
		{
			var filteredPilotGroups = allPilots.Where (p => Faction.Name != "Mixed" ? p.Faction.Name == Faction.Name : p != null).ToList ();
			var searchPilots = new ObservableCollection <PilotGroup> ();

			foreach (var grp in filteredPilotGroups) {
				if (grp.Ship.Name.ToLower ().Contains (text.ToLower ())) {
					searchPilots.Add (grp);
					continue;
				}

				var filteredPilots = grp.Where (p => p.Name.ToLower ().Contains (text.ToLower ()));
				if (filteredPilots?.Count () > 0) {
					var newGroup = new PilotGroup (grp.Ship) { Faction = grp.Faction };
					foreach (var pilot in filteredPilots)
						newGroup.Add (pilot);
					searchPilots.Add (newGroup);
				}
			}

			PilotGroups = searchPilots;
		}
	}
}