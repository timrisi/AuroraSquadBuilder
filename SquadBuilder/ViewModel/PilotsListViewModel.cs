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

		Ship ship;
		public Ship Ship {
			get { return ship; }
			set {
				SetProperty (ref ship, value);
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
					MessagingCenter.Send <PilotsListViewModel, Pilot> (this, "Pilot selected", SelectedPilot.Copy ());
			}
		}

		ObservableCollection <PilotGroup> GetAllPilots ()
		{
			var allPilotGroups = new ObservableCollection <PilotGroup>();

			var allPilots = Cards.SharedInstance.Pilots.ToList ();

			if (Settings.AllowCustom)
				allPilots.AddRange (Cards.SharedInstance.CustomPilots);

			foreach (var pilot in allPilots) {
				while (pilot.UpgradesEquipped.Count () < pilot.UpgradeTypes.Count ())
					pilot.UpgradesEquipped.Add (null);
				
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

			var filteredPilotGroups = allPilots.Where (p => (Faction == null || Faction?.Name != "Mixed" ? p?.Faction?.Name == Faction?.Name : p != null) &&
															(Ship == null || p?.Ship == Ship)).ToList ();

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

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			allPilots = GetAllPilots ();

			PilotGroups = new ObservableCollection <PilotGroup> (allPilots);
			filterPilots ();
		}
	}
}