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

		string faction = "";
		public string Faction {
			get { return faction; }
			set { 
				SetProperty (ref faction, value); 
				if (!string.IsNullOrEmpty (value))
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

			XElement pilotsXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Pilots.xml"));
			XElement factionsXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Factions.xml"));
			XElement shipsXml = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Ships.xml"));

			var allPilots = new ObservableCollection <Pilot> (from pilot in pilotsXml.Elements ()
				select new Pilot {
					Name = pilot.Element ("Name").Value,
					Faction = factionsXml.Descendants ().FirstOrDefault (e => (string)e.Attribute ("id") == (string)pilot.Attribute ("faction"))?.Value,
					Ship = (from ship in shipsXml.Elements ()
						where ship.Attribute ("id").Value == pilot.Attribute ("ship")?.Value
						select new Ship {
							Name = ship.Element ("Name").Value,
							Actions = ship.Element ("Actions").Elements ("Action").Select (e => e.Value).ToList (),
							LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
							Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false
						}).SingleOrDefault (),
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
				});
			
			foreach (var pilot in allPilots) {
				while (pilot.UpgradesEquipped.Count () < pilot.UpgradeTypes.Count ())
					pilot.UpgradesEquipped.Add (null);
				
				var pilotGroup = allPilotGroups.FirstOrDefault (g => g.Ship?.Name == pilot.Ship?.Name && g.Faction == pilot.Faction);

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

			var filteredPilotGroups = allPilots.Where (p => p.Faction == Faction).ToList ();

			foreach (var pilotGroup in filteredPilotGroups)
				PilotGroups.Add (pilotGroup);
		}

		public void SearchPilots (string text)
		{
			var filteredPilotGroups = allPilots.Where (p => p.Faction == Faction).ToList ();
			var searchPilots = new ObservableCollection <PilotGroup> ();

			foreach (var grp in filteredPilotGroups) {
				if (grp.Ship.Name.ToLower ().Contains (text.ToLower ())) {
					searchPilots.Add (grp);
					continue;
				}

				var filteredPilots = grp.Where (p => p.Name.ToLower ().Contains (text.ToLower ()));
				if (filteredPilots.Count () > 0) {
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