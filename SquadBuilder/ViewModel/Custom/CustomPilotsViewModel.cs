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
			MessagingCenter.Subscribe <Pilot> (this, "DeletePilot", pilot => {
				var pilotGroup = PilotGroups.FirstOrDefault (g => g.Contains (pilot));
				if (pilotGroup != null) {
					pilotGroup.Remove (pilot);
					if (pilotGroup.Count == 0)
						PilotGroups.Remove (pilotGroup);
				}
			});

			MessagingCenter.Subscribe <Pilot> (this, "Edit Pilot", pilot => {
				MessagingCenter.Subscribe <EditPilotViewModel, Pilot> (this, "Finished Editing", (vm, updatedPilot) => {
					Cards.SharedInstance.CustomPilots [Cards.SharedInstance.CustomPilots.IndexOf (pilot)] = updatedPilot;

					var originalGroup = PilotGroups.FirstOrDefault (g => g.ToList ().Exists (p => p.Name == pilot.Name && p.Ship.Name == pilot.Ship.Name && p.Faction.Name == pilot.Faction.Name));
					var pilotGroup = PilotGroups.FirstOrDefault (g => g.Ship?.Name == updatedPilot.Ship?.Name && g.Faction.Id == updatedPilot.Faction.Id);

					if (originalGroup != null && pilotGroup == originalGroup) {
						pilotGroup [pilotGroup.IndexOf (pilot)] = updatedPilot;
					} else if (originalGroup != null) {
						originalGroup.Remove (pilot);

						if (originalGroup.Count == 0)
							PilotGroups.Remove (originalGroup);

						if (pilotGroup != null)
							pilotGroup.Add (updatedPilot);
					}

					if (pilotGroup == null) {
						pilotGroup = new PilotGroup (updatedPilot?.Ship) { Faction = updatedPilot?.Faction }; 
						PilotGroups?.Add (pilotGroup);
						pilotGroup.Add (updatedPilot);
					}

					foreach (var squad in Cards.SharedInstance.Squadrons) {
						if (squad.Pilots.Any (p => p.Id == updatedPilot.Id)) {
							for (int i = 0; i < squad.Pilots.Count; i++) {
								var squadPilot = squad.Pilots [i];

								if (squadPilot.Id != updatedPilot.Id)
									continue;

								var upgradeTypes = squadPilot.UpgradeTypes;
								var upgradesEquipped = squadPilot.UpgradesEquipped;
								squad.Pilots [i] = updatedPilot.Copy ();
								squad.Pilots [i].UpgradesEquipped = new ObservableCollection<Upgrade> ();
								for (int k = 0; k < squad.Pilots [i].UpgradeTypes.Count (); k++) {
									squad.Pilots [i].UpgradesEquipped.Add (null);
								}

								for (int j = 0; j < upgradesEquipped.Count; j++) {
									var equippedUpgrade = upgradesEquipped [j];
									if (equippedUpgrade != null) {
										var index = squad.Pilots [i].UpgradeTypes.IndexOf (upgradeTypes [j]);
										if (index >= 0)
											squad.Pilots [i].UpgradesEquipped [index] = equippedUpgrade;
									}
								}
							}
						}
					}
						
					Navigation.RemoveAsync <EditPilotViewModel> (vm);
					PilotGroups = new ObservableCollection <PilotGroup> (PilotGroups);
					MessagingCenter.Unsubscribe <EditPilotViewModel, Pilot> (this, "Finished Editing");
				});

				Navigation.PushAsync<EditPilotViewModel> ((vm, p) => {
					vm.Pilot = pilot.Copy ();
				});
			});
		}

		ObservableCollection <PilotGroup> pilotGroups = new ObservableCollection <PilotGroup> ();
		public ObservableCollection <PilotGroup> PilotGroups {
			get { return pilotGroups; }
			set {
				SetProperty (ref pilotGroups, value);
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
						MessagingCenter.Subscribe <EditPilotViewModel, Pilot> (this, "Pilot Created", (vm, pilot) => {
							while (pilot.UpgradesEquipped.Count () < pilot.UpgradeTypes.Count ())
								pilot.UpgradesEquipped.Add (null);

							var pilotGroups = new ObservableCollection<PilotGroup> (PilotGroups);
							var pilotGroup = pilotGroups.FirstOrDefault (g => g.Ship?.Name == pilot.Ship?.Name && g.Faction.Id == pilot.Faction.Id);

							if (pilotGroup == null) {
								pilotGroup = new PilotGroup (pilot.Ship) { Faction = pilot.Faction };
								pilotGroups.Add (pilotGroup);
							}

							pilotGroup.Add (pilot);

							PilotGroups = new ObservableCollection<PilotGroup> (pilotGroups);
							Cards.SharedInstance.CustomPilots.Add (pilot);
							Cards.SharedInstance.GetAllPilots ();
							Navigation.RemoveAsync <EditPilotViewModel> (vm);
							MessagingCenter.Unsubscribe <EditPilotViewModel, Pilot> (this, "Pilot Created");
						});

						Navigation.PushAsync<EditPilotViewModel> ((vm, p) => {
							vm.Pilot = new Pilot ();
							vm.Create = true;
						});
					});

				return createPilot;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			var allPilotGroups = new ObservableCollection <PilotGroup>();

			var allPilots = Cards.SharedInstance.CustomPilots;

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


			NotifyPropertyChanged ("PilotGroups");
		}
	}
}

