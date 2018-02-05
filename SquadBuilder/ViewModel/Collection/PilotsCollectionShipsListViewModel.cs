using System;

using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class PilotsCollectionShipsListViewModel : ViewModel
	{
		ObservableCollection <Ship> allShips;

		public PilotsCollectionShipsListViewModel ()
		{
			GetAllShips ();
		}

		public string PageName { get { return "Pilots"; } }

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
					NavigationService.PushAsync (new PilotsCollectionViewModel { Ship = SelectedShip.Copy () });
				}
			}
		}

		void GetAllShips ()
		{
			allShips = Ship.Ships;
			Ships = new ObservableCollection <Ship> (allShips.Where (s => !s.CCL).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge));
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllShips ();

			SelectedShip = null;
		}
	}
}

