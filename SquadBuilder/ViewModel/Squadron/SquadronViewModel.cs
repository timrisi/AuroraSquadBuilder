﻿using System;

using System.Collections.ObjectModel;

using Xamarin.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace SquadBuilder
{
	public class SquadronViewModel : ViewModel
	{	
		Squadron squadron;
		public Squadron Squadron {
			get { return squadron; }
			set { 
				SetProperty (ref squadron, value);
			}
		}

		public ObservableCollection <Pilot> Pilots {
			get { return Squadron?.Pilots; }
			set { 
				Squadron.Pilots = value;
			}
		}

		Pilot selectedPilot = null;
		public Pilot SelectedPilot {
			get { return selectedPilot; }
			set { 
				SetProperty (ref selectedPilot, value); 
				if (value != null) {
					NavigationService.PushAsync (new PilotViewModel { Pilot = selectedPilot }).ContinueWith ((task) => { SelectedPilot = null; });
				}
			}
		}

		public int Wins {
			get { return Squadron.Wins; }
			set { Squadron.Wins = value; }
		}

		public int Losses {
			get { return Squadron.Losses; }
			set { Squadron.Losses = value; }
		}

		public int Draws { 
			get { return Squadron.Draws; }
			set { Squadron.Draws = value; }
		}

		public string NavigateToPilotsListText { get { return "+"; } }

		Command navigateToPilotsList;
		public Command NavigateToPilotsList {
			get {
				if (navigateToPilotsList == null)
					navigateToPilotsList = new Command (() => {
						if (!Settings.FilterPilotsByShip) {
							MessagingCenter.Subscribe <PilotsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
								if (pilot.Id.Contains ("cr90")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Fore")).Copy ();
										pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Aft")).Copy ();
										pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										otherPilot.MultiSectionId = pilot.MultiSectionId;	
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Pilots.Add (pilot);

								if (pilot.Id.Contains ("raider")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Fore")).Copy ();
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else{
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Aft")).Copy ();
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Squadron.SaveSquadrons ();

								MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");
							});
						} else {
							MessagingCenter.Subscribe <ShipsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
								if (pilot.Id.Contains ("cr90")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Fore")).Copy ();
										if (Squadron.Pilots.Any ())
											pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										else
											pilot.MultiSectionId = 0;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Aft")).Copy ();
										if (Squadron.Pilots.Any ())
											pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										else
											pilot.MultiSectionId = 0;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Pilots.Add (pilot);

								if (pilot.Id.Contains ("raider")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Fore")).Copy ();
										if (Squadron.Pilots.Any ())
											pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										else
											pilot.MultiSectionId = 0;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else{
										var otherPilot = Pilot.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Aft")).Copy ();
										if (Squadron.Pilots.Any ())
											pilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
										else
											pilot.MultiSectionId = 0;
										otherPilot.MultiSectionId = pilot.MultiSectionId;
										//pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										//otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								MessagingCenter.Unsubscribe <ShipsListViewModel, Pilot> (this, "Pilot selected");
							});
						}

						if (Settings.FilterPilotsByShip)
							NavigationService.PushAsync (new ShipsListViewModel { Faction = Squadron.Faction });
						else
							NavigationService.PushAsync (new PilotsListViewModel { Faction = Squadron.Faction });
					});

				return navigateToPilotsList;
			}
		}

		Command exportToClipboard;
		public Command ExportToClipboard {
			get {
				if (exportToClipboard == null)
					exportToClipboard = new Command (() => {
						var builder = new StringBuilder ();
						builder.AppendLine (Squadron.Name + " (" + Squadron.Points + ")");
						builder.AppendLine ();

						foreach (var pilot in Squadron.Pilots) {
							builder.AppendLine (pilot.Name + " (" + pilot.Cost + ")" + " - " + pilot.Ship.Name);
							builder.AppendLine (string.Join (", ", pilot.UpgradesEquipped.Where (u => u != null).Select (u => u.Name + " (" + u.Cost + ")")));
							builder.AppendLine ();
						}

						DependencyService.Get <IClipboardService> ().CopyToClipboard (builder.ToString ());
						MessagingCenter.Send <SquadronViewModel> (this, "Squadron Copied");
					});

				return exportToClipboard;
			}
		}

		Command exportXws;
		public Command ExportXws {
			get {
				if (exportXws == null)
					exportXws = new Command (() => {
						var json = Squadron.CreateXws ();

						DependencyService.Get <IClipboardService> ().CopyToClipboard (json.ToString ());
						MessagingCenter.Send <SquadronViewModel> (this, "Squadron Copied as XWS data");
					});

				return exportXws;
			}
		}

		Command exportCompactXws;
		public Command ExportCompactXws {
			get {
				if (exportCompactXws == null)
					exportCompactXws = new Command (() => {
						var json = Squadron.CreateXws ();
						json = Regex.Replace (json, @"\s+", "");
						DependencyService.Get<IClipboardService> ().CopyToClipboard (json.ToString ());
						MessagingCenter.Send<SquadronViewModel> (this, "Squadron Copied as XWS data");
					});

				return exportCompactXws;
			}
		}

		Command decrementWins;
		public Command DecrementWins {
			get {
				if (decrementWins == null) {
					decrementWins = new Command (() => {
						Squadron.Wins --;
						NotifyPropertyChanged ("Wins");
					});
				}

				return decrementWins;
			}
		}

		Command incrementWins;
		public Command IncrementWins {
			get {
				if (incrementWins == null) {
					incrementWins = new Command (() => {
						Squadron.Wins ++;
						NotifyPropertyChanged ("Wins");
					});
				}

				return incrementWins;
			}
		}

		Command decrementLosses;
		public Command DecrementLosses {
			get {
				if (decrementLosses == null) {
					decrementLosses = new Command (() => {
						Squadron.Losses --;
						NotifyPropertyChanged ("Losses");
					});
				}

				return decrementLosses;
			}
		}

		Command incrementLosses;
		public Command IncrementLosses {
			get {
				if (incrementLosses == null) {
					incrementLosses = new Command (() => {
						Squadron.Losses ++;
						NotifyPropertyChanged ("Losses");
					});
				}

				return incrementLosses;
			}
		}

		Command decrementDraws;
		public Command DecrementDraws {
			get {
				if (decrementDraws == null) {
					decrementDraws = new Command (() => {
						Squadron.Draws --;
						NotifyPropertyChanged ("Draws");
					});
				}

				return decrementDraws;
			}
		}

		Command incrementDraws;
		public Command IncrementDraws {
			get {
				if (incrementDraws == null) {
					incrementDraws = new Command (() => {
						Squadron.Draws ++;
						NotifyPropertyChanged ("Draws");
					});
				}

				return incrementDraws;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			MessagingCenter.Subscribe <Pilot> (this, "Remove Pilot", 
				(pilot) => {
					Pilots.Remove (pilot);
					if (pilot.MultiSectionId >= 0)
						Pilots.Remove (Pilots.First (p => p.MultiSectionId == pilot.MultiSectionId));
					//if (pilot.LinkedPilotCardGuid != Guid.Empty)
					//	Pilots.Remove (Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid));
				}
			);
			MessagingCenter.Subscribe <Pilot> (this, "Copy Pilot",
				(pilot) => {
					var pilotCopy = pilot.Copy ();

					if (pilot.MultiSectionId >= 0 && pilotCopy.Name.Contains ("Aft")) {
						var otherPilot = Pilots.First (p => p.MultiSectionId == pilot.MultiSectionId && p.Name.Contains ("Fore")).Copy ();
						otherPilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
						pilotCopy.MultiSectionId = otherPilot.MultiSectionId;
						Pilots.Add (otherPilot);
					}

					//if (pilot.LinkedPilotCardGuid != Guid.Empty && pilotCopy.Name.Contains ("Aft")) {
					//	var otherPilot = Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid && p.Name.Contains ("Fore")).Copy ();
					//	otherPilot.LinkedPilotCardGuid = Guid.NewGuid ();
					//	pilotCopy.LinkedPilotCardGuid = otherPilot.LinkedPilotCardGuid;
					//	Pilots.Add (otherPilot);
					//}
							
					Pilots.Add (pilotCopy);

					if (pilot.MultiSectionId >= 0 && pilotCopy.Name.Contains ("Fore")) {
						var otherPilot = Pilots.First (p => p.MultiSectionId == pilot.MultiSectionId && p.Name.Contains ("Aft")).Copy ();
						otherPilot.MultiSectionId = Squadron.Pilots.Max (p => p.MultiSectionId) + 1;
						pilotCopy.MultiSectionId = otherPilot.MultiSectionId;
						Pilots.Add (otherPilot);
					}

					//if (pilot.LinkedPilotCardGuid != Guid.Empty && pilotCopy.Name.Contains ("Fore")) {
					//	var otherPilot = Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid && p.Name.Contains ("Aft")).Copy ();
					//	otherPilot.LinkedPilotCardGuid = Guid.NewGuid ();
					//	pilotCopy.LinkedPilotCardGuid = otherPilot.LinkedPilotCardGuid;
					//	Pilots.Add (otherPilot);
					//}
				}
			);

			Pilots = new ObservableCollection <Pilot> (Pilots);

			NotifyPropertyChanged ("Squadron");
			NotifyPropertyChanged ("Pilots");
			NotifyPropertyChanged ("SelectedPilot");

			MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");
			MessagingCenter.Unsubscribe <ShipsListViewModel, Pilot> (this, "Pilot selected");
		}

		public override void OnViewDisappearing ()
		{
			base.OnViewDisappearing ();

			MessagingCenter.Send<SquadronViewModel> (this, "Squadron updated");
			MessagingCenter.Unsubscribe <Pilot> (this, "Remove Pilot");
			MessagingCenter.Unsubscribe <Pilot> (this, "Copy Pilot");
		}
	}
}

