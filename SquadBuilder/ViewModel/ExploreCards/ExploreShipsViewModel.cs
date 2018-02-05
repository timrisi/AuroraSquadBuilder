using System;

using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class ExploreShipsViewModel : ViewModel
	{
		ObservableCollection<Ship> allShips;

		public ExploreShipsViewModel ()
		{
			GetAllShips ();
		}

		public string PageName { get { return "Ships and Pilots"; } }

		ObservableCollection<Ship> ships = new ObservableCollection<Ship> ();
		public ObservableCollection<Ship> Ships {
			get {
				return ships;
			}
			set {
				SetProperty (ref ships, value);
				ships.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Ships");
			}
		}

		Ship selectedShip = null;
		public Ship SelectedShip {
			get { return selectedShip; }
			set {
				SetProperty (ref selectedShip, value);
				if (selectedShip != null)
					NavigationService.PushAsync (new ExplorePilotsViewModel { Ship = selectedShip.Copy () });
			}
		}

		void GetAllShips ()
		{
			if (Settings.AllowCustom)
				allShips = Ship.AllShips;
			else
				allShips = new ObservableCollection<Ship> (Ship.Ships.Where (s => !s.IsCustom));

			if (!Settings.CustomCardLeague)
				allShips = new ObservableCollection<Ship> (allShips.Where (s => !s.CCL));
		}

		void filterShips ()
		{
			ObservableCollection<Pilot> pilots;
			if (Settings.AllowCustom)
				pilots = Pilot.AllPilots;
			else
				pilots = Pilot.Pilots;

			if (!Settings.CustomCardLeague)
				pilots = new ObservableCollection<Pilot> (pilots.Where (p => !p.CCL));

			Ships = new ObservableCollection<Ship> (allShips.OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge));
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

