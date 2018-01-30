using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class ShipsListViewModel : ViewModel
	{
		ObservableCollection <Ship> allShips;

		public ShipsListViewModel ()
		{
			GetAllShips ();
		}

		public string PageName { get { return "Ships"; } }

		Faction faction;
		public Faction Faction {
			get { return faction; }
			set { 
				SetProperty (ref faction, value); 
				if (value != null)
					filterShips ();
			}
		}

		ObservableCollection <Ship> ships = new ObservableCollection <Ship> ();
		public ObservableCollection <Ship> Ships {
			get {
				return ships;
			} set {
				SetProperty (ref ships, value);
				ships.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Ships");
			}
		}

		Ship selectedShip = null;
		public Ship SelectedShip {
			get { return selectedShip; }
			set { 
				SetProperty (ref selectedShip, value); 

				if (value != null) {
					MessagingCenter.Subscribe <PilotsListViewModel, Pilot> (this, "Pilot selected", (vm, pilot) => {
						MessagingCenter.Unsubscribe <PilotsListViewModel, Pilot> (this, "Pilot selected");

						MessagingCenter.Send <ShipsListViewModel, Pilot> (this, "Pilot selected", pilot);
						Navigation.RemoveAsync <ShipsListViewModel> (this);
					});

					Navigation.PushAsync <PilotsListViewModel> ((vm, p) => {
						vm.Faction = Faction;
						vm.Ship = selectedShip.Copy ();
					});
				}
			}
		}

		public string PointsDescription {
			get { return Squadron.CurrentSquadron?.PointsDescription; }
		}

		void GetAllShips ()
		{
			if (Settings.AllowCustom)
				allShips = Ship.AllShips;
			else
				allShips = new ObservableCollection<Ship> (Ship.Ships.Where (s => !s.IsCustom));

			if (Settings.HideUnavailable)
				allShips = new ObservableCollection<Ship> (allShips.Where (s => s.IsAvailable));

			if (!Settings.CustomCardLeague)
				allShips = new ObservableCollection<Ship> (allShips.Where (s => !s.CCL));
		}

		void filterShips ()
		{
			ObservableCollection <Pilot> pilots;
			if (Settings.AllowCustom)
				pilots = Pilot.AllPilots;
			else
				pilots = Pilot.Pilots;

			if (!Settings.CustomCardLeague)
				pilots = new ObservableCollection <Pilot> (pilots.Where (p => !p.CCL));

			Ships = new ObservableCollection <Ship> (allShips.Where (s => pilots.Any (p => (Faction.Name == "Mixed" || p.Faction.Id == Faction.Id) && p.Ship.Id == s.Id)).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge));
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllShips ();

			filterShips ();

			SelectedShip = null;
		}
	}
}

