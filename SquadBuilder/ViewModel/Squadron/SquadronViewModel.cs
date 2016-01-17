using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Security.AccessControl;

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
					Navigation.PushAsync <PilotViewModel> ((vm,p) => {
						vm.Pilot = selectedPilot;
						SelectedPilot = null;
					});
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

		RelayCommand navigateToPilotsList;
		public RelayCommand NavigateToPilotsList {
			get {
				if (navigateToPilotsList == null)
					navigateToPilotsList = new RelayCommand (() => {
						if (!Settings.FilterPilotsByShip) {
							MessagingCenter.Subscribe <PilotsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
								if (pilot.Id.Contains ("cr90")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Fore")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Aft")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Pilots.Add (pilot);

								if (pilot.Id.Contains ("raider")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Fore")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else{
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Aft")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Navigation.RemoveAsync <PilotsListViewModel> (vm);

								MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");
							});
						} else {
							MessagingCenter.Subscribe <ShipsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
								if (pilot.Id.Contains ("cr90")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Fore")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("cr90") && p.Name.Contains ("Aft")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}

								Pilots.Add (pilot);

								if (pilot.Id.Contains ("raider")) {
									if (pilot.Name.Contains ("Aft")) {
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Fore")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
									else{
										var otherPilot = Cards.SharedInstance.Pilots.First (p => p.Id.Contains ("raider") && p.Name.Contains ("Aft")).Copy ();
										pilot.LinkedPilotCardGuid = Guid.NewGuid ();
										otherPilot.LinkedPilotCardGuid = pilot.LinkedPilotCardGuid;
										Pilots.Add (otherPilot);
									}
								}
								
								Navigation.PopAsync ();

								MessagingCenter.Unsubscribe <ShipsListViewModel, Pilot> (this, "Pilot selected");
							});
						}

						if (Settings.FilterPilotsByShip) {
							Navigation.PushAsync <ShipsListViewModel> ((vm, p) => {
								vm.Faction = Squadron.Faction;
							});
						} else {
							Navigation.PushAsync <PilotsListViewModel> ((vm, p) => {
								vm.Faction = Squadron.Faction;
							});
						}
					});

				return navigateToPilotsList;
			}
		}

		RelayCommand exportToClipboard;
		public RelayCommand ExportToClipboard {
			get {
				if (exportToClipboard == null)
					exportToClipboard = new RelayCommand (() => {
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

		RelayCommand exportXws;
		public RelayCommand ExportXws {
			get {
				if (exportXws == null)
					exportXws = new RelayCommand (() => {
						var json = Squadron.CreateXws ();

						DependencyService.Get <IClipboardService> ().CopyToClipboard (json.ToString ());
						MessagingCenter.Send <SquadronViewModel> (this, "Squadron Copied as XWS data");
					});

				return exportXws;
			}
		}

		RelayCommand decrementWins;
		public RelayCommand DecrementWins {
			get {
				if (decrementWins == null) {
					decrementWins = new RelayCommand (() => {
						Squadron.Wins --;
						NotifyPropertyChanged ("Wins");
					});
				}

				return decrementWins;
			}
		}

		RelayCommand incrementWins;
		public RelayCommand IncrementWins {
			get {
				if (incrementWins == null) {
					incrementWins = new RelayCommand (() => {
						Squadron.Wins ++;
						NotifyPropertyChanged ("Wins");
					});
				}

				return incrementWins;
			}
		}

		RelayCommand decrementLosses;
		public RelayCommand DecrementLosses {
			get {
				if (decrementLosses == null) {
					decrementLosses = new RelayCommand (() => {
						Squadron.Losses --;
						NotifyPropertyChanged ("Losses");
					});
				}

				return decrementLosses;
			}
		}

		RelayCommand incrementLosses;
		public RelayCommand IncrementLosses {
			get {
				if (incrementLosses == null) {
					incrementLosses = new RelayCommand (() => {
						Squadron.Losses ++;
						NotifyPropertyChanged ("Losses");
					});
				}

				return incrementLosses;
			}
		}

		RelayCommand decrementDraws;
		public RelayCommand DecrementDraws {
			get {
				if (decrementDraws == null) {
					decrementDraws = new RelayCommand (() => {
						Squadron.Draws --;
						NotifyPropertyChanged ("Draws");
					});
				}

				return decrementDraws;
			}
		}

		RelayCommand incrementDraws;
		public RelayCommand IncrementDraws {
			get {
				if (incrementDraws == null) {
					incrementDraws = new RelayCommand (() => {
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
					if (pilot.LinkedPilotCardGuid != Guid.Empty)
						Pilots.Remove (Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid));
				}
			);
			MessagingCenter.Subscribe <Pilot> (this, "Copy Pilot",
				(pilot) => {
					var pilotCopy = pilot.Copy ();

					if (pilot.LinkedPilotCardGuid != Guid.Empty && pilotCopy.Name.Contains ("Aft")) {
						var otherPilot = Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid && p.Name.Contains ("Fore")).Copy ();
						otherPilot.LinkedPilotCardGuid = Guid.NewGuid ();
						pilotCopy.LinkedPilotCardGuid = otherPilot.LinkedPilotCardGuid;
						Pilots.Add (otherPilot);
					}
							
					Pilots.Add (pilotCopy);

					if (pilot.LinkedPilotCardGuid != Guid.Empty && pilotCopy.Name.Contains ("Fore")) {
						var otherPilot = Pilots.First (p => p.LinkedPilotCardGuid == pilot.LinkedPilotCardGuid && p.Name.Contains ("Aft")).Copy ();
						otherPilot.LinkedPilotCardGuid = Guid.NewGuid ();
						pilotCopy.LinkedPilotCardGuid = otherPilot.LinkedPilotCardGuid;
						Pilots.Add (otherPilot);
					}
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

			MessagingCenter.Unsubscribe <Pilot> (this, "Remove Pilot");
			MessagingCenter.Unsubscribe <Pilot> (this, "Copy Pilot");
		}
	}
}

