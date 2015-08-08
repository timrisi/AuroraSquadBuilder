using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CustomPilotsViewModel : ViewModel
	{
		public CustomPilotsViewModel ()
		{
			var allPilotGroups = new ObservableCollection <PilotGroup>();

			XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

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

			var allPilots = new ObservableCollection <Pilot> (from pilot in customPilotsXml.Elements ()
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

			PilotGroups = new ObservableCollection <PilotGroup> (allPilotGroups);

			MessagingCenter.Subscribe <Pilot> (this, "DeletePilot", pilot => {
				var pilotGroup = PilotGroups.FirstOrDefault (g => g.Contains (pilot));
				if (pilotGroup != null) {
					pilotGroup.Remove (pilot);
					if (pilotGroup.Count == 0)
						PilotGroups.Remove (pilotGroup);
				}
			});
		}

		ObservableCollection <PilotGroup> pilotGroups = new ObservableCollection <PilotGroup> ();
		public ObservableCollection <PilotGroup> PilotGroups {
			get { return pilotGroups; }
			set {
				SetProperty (ref pilotGroups, value);
				PilotGroups.CollectionChanged += (sender, e) => NotifyPropertyChanged ("PilotGroups");
			}
		}

		public string PageName {
			get { return "Pilots"; }
		}

		RelayCommand createPilot;
		public RelayCommand CreatePilot {
			get {
				if (createPilot == null)
					createPilot = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreatePilotViewModel, Pilot> (this, "Pilot Created", (vm, pilot) => {
							while (pilot.UpgradesEquipped.Count () < pilot.UpgradeTypes.Count ())
								pilot.UpgradesEquipped.Add (null);

							var pilotGroup = PilotGroups.FirstOrDefault (g => g.Ship?.Name == pilot.Ship?.Name && g.Faction.Id == pilot.Faction.Id);

							if (pilotGroup == null) {
								pilotGroup = new PilotGroup (pilot.Ship) { Faction = pilot.Faction };
								PilotGroups.Add (pilotGroup);
							}

							pilotGroup.Add (pilot);

							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <CreatePilotViewModel, Pilot> (this, "Pilot Created");
						});

						Navigation.PushAsync <CreatePilotViewModel> ();
					});

				return createPilot;
			}
		}
	}
}

