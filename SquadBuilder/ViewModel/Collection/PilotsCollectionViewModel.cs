using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class PilotsCollectionViewModel : ViewModel
	{
		ObservableCollection <PilotGroup> allPilots;

		public PilotsCollectionViewModel ()
		{
			allPilots = getAllPilots ();

			PilotGroups = new ObservableCollection <PilotGroup> (allPilots);
			filterPilots ();
		}

		public string PageName { get { return "Pilots Owned"; } }

		ObservableCollection <PilotGroup> pilotGroups = new ObservableCollection <PilotGroup> ();
		public ObservableCollection <PilotGroup> PilotGroups {
			get {
				return pilotGroups;
			}
			set {
				SetProperty (ref pilotGroups, value);
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

		ObservableCollection <PilotGroup> getAllPilots ()
		{
			var allPilotGroups = new List <PilotGroup>();

			var pilotList = Pilot.Pilots.Where (p => !p.CCL).ToList ();

			foreach (var pilot in pilotList) {
				var pilotGroup = allPilotGroups.FirstOrDefault (g => g.Ship?.Name == pilot.Ship?.Name && g.Faction.Id == pilot.Faction.Id);

				if (pilotGroup == null) {
					pilotGroup = new PilotGroup (pilot.Ship) { Faction = pilot.Faction };
					allPilotGroups.Add (pilotGroup);
				}

				pilotGroup.Add (pilot);
			}

			return new ObservableCollection <PilotGroup> (allPilotGroups.OrderBy (g => g.Ship.Name).OrderBy (g => g.Ship.LargeBase).OrderBy (g => g.Ship.Huge));
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			allPilots = getAllPilots ();

			PilotGroups = new ObservableCollection <PilotGroup> (allPilots);
			filterPilots ();
		}

		void filterPilots ()
		{
			PilotGroups.Clear ();

			var filteredPilotGroups = allPilots.Where (p => (Ship == null || p?.Ship?.Id == Ship?.Id)).ToList ();

			foreach (var pilotGroup in filteredPilotGroups)
				PilotGroups.Add (pilotGroup);
		}
	}
}