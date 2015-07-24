using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace SquadBuilder
{
	public class SquadronViewModel : ViewModel
	{
		public SquadronViewModel ()
		{				
			
		}
			
		Squadron squadron;
		public Squadron Squadron {
			get { return squadron; }
			set { 
				SetProperty (ref squadron, value);
				Pilots.CollectionChanged += (sender, e) => 
					NotifyPropertyChanged ("PointsDescription");
			}
		}

		public string PointsDescription {
			get {
				return Squadron.Points + "/" + Squadron.MaxPoints;
			}
		}

		public ObservableCollection <Pilot> Pilots {
			get { return Squadron.Pilots; }
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

		public string NavigateToPilotsListText { get { return "+"; } }

		RelayCommand navigateToPilotsList;
		public RelayCommand NavigateToPilotsList {
			get {
				if (navigateToPilotsList == null)
					navigateToPilotsList = new RelayCommand (() => {
						MessagingCenter.Subscribe <PilotsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
							Pilots.Add (pilot);
							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");
						});

						Navigation.PushAsync <PilotsListViewModel> ((vm, p) => {
							vm.Faction = Squadron.Faction;
						});
					});

				return navigateToPilotsList;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			MessagingCenter.Subscribe <Pilot> (this, "DeletePilot", 
				(pilot) => Pilots.Remove (pilot));
			MessagingCenter.Subscribe <Pilot> (this, "Copy Pilot",
				(pilot) => Pilots.Add (pilot.Copy ()));

			NotifyPropertyChanged ("Squadron");
			NotifyPropertyChanged ("Pilots");
			NotifyPropertyChanged ("SelectedPilot");
			NotifyPropertyChanged ("PointsDescription");
		}

		public override void OnViewDisappearing ()
		{
			base.OnViewDisappearing ();

			MessagingCenter.Unsubscribe <Pilot> (this, "DeletePilot");
			MessagingCenter.Unsubscribe <Pilot> (this, "Copy Pilot");
		}
	}
}

