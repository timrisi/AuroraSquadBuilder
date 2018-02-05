using System;

using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

using System.IO;

namespace SquadBuilder
{
	public class PilotsListViewModel : ViewModel
	{
		ObservableCollection <PilotGroup> allPilots;

		public PilotsListViewModel ()
		{
			allPilots = getAllPilots ();

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

				if (value != null) {
					MessagingCenter.Send <PilotsListViewModel, Pilot> (this, "Pilot selected", SelectedPilot.Copy ());
					NavigationService.PopAsync (); // <PilotsListViewModel> (this);
				}
			}
		}

		public string PointsDescription {
			get { return Squadron.CurrentSquadron.PointsDescription; }
		}

		ObservableCollection <PilotGroup> getAllPilots ()
		{
			var allPilotGroups = new ObservableCollection <PilotGroup>();

			var allPilots = Pilot.Pilots.ToList ();

			if (Settings.AllowCustom)
				allPilots.AddRange (Pilot.CustomPilots);
			else
				allPilots.RemoveAll (p => p.IsCustom);

			if (Settings.HideUnavailable)
				allPilots = allPilots.Where (p => p.IsAvailable).ToList ();

			if (!Settings.CustomCardLeague)
				allPilots = allPilots.Where (p => !p.CCL).ToList ();

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
				if (value == null)
					value = "";
				
				SetProperty (ref searchText, value); 

				SearchPilots (value);
			}
		}

		void filterPilots ()
		{
			PilotGroups.Clear ();

			var filteredPilotGroups = allPilots.Where (p => (Faction == null || Faction?.Name != "Mixed" ? p?.Faction?.Name == Faction?.Name : p != null) &&
			                                           (Ship == null || p?.Ship.Id == Ship.Id)).ToList ();

			foreach (var pilotGroup in filteredPilotGroups) {
				PilotGroups.Add (new PilotGroup (pilotGroup.Ship, pilotGroup.Faction, pilotGroup.OrderByDescending (p => p.PilotSkill)));
			}
		}

		public void SearchPilots (string text)
		{
			text = text.ToLower ();
			var filteredPilotGroups = allPilots.Where (p => Faction.Name != "Mixed" ? p.Faction.Name == Faction.Name : p != null).ToList ();

			var searchPilots = new ObservableCollection <PilotGroup> ();

			foreach (var grp in filteredPilotGroups) {
				if (Ship == null) {
					if (grp.Ship.Name.ToLower ().Contains (text) ||
						grp.Ship.ActionsString.ToLower ().Contains (text)) {
						searchPilots.Add (grp);
						continue;
					}
				} else if (grp.Ship.Name != Ship.Name)
					continue;

				var filteredPilots = grp.Where (p => p.Name.ToLower ().Contains (text) ||
												(!string.IsNullOrEmpty (p.Ability) && p.Ability.ToLower ().Contains (text)) ||
				                        					p.UpgradeTypes.Any (u => u.ToLower ().Contains (text)) ||
				                               					p.Keywords.Contains (text));
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

			allPilots = getAllPilots ();

			PilotGroups = new ObservableCollection <PilotGroup> (allPilots);
			filterPilots ();
		}
	}
}