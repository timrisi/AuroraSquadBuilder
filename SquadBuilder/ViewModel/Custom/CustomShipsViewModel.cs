using System;
using System.Xml.Linq;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using System.IO;
using System.Linq;


namespace SquadBuilder
{
	public class CustomShipsViewModel : ViewModel
	{
		public CustomShipsViewModel ()
		{
			MessagingCenter.Subscribe <Ship> (this, "Remove Ship", ship => {
				Ships.Remove (ship);
				Ship.CustomShips.Remove (ship);
			});

			MessagingCenter.Subscribe <Ship> (this, "Edit Ship", ship => {
				string shipName = ship.Name;
				MessagingCenter.Subscribe <EditShipViewModel, Ship> (this, "Finished Editing", (vm, updatedShip) => {
					Ships [Ships.IndexOf (ship)] = updatedShip;
					Ship.CustomShips [Ship.CustomShips.IndexOf (ship)] = updatedShip;

					NavigationService.PopAsync (); // <EditShipViewModel> (vm);
					NotifyPropertyChanged ("Ships");
					MessagingCenter.Unsubscribe <EditShipViewModel, Ship> (this, "Finished Editing");
				});

				NavigationService.PushAsync (new EditShipViewModel { Ship = ship?.Copy () });
			});
		}

		public string PageName { get { return "Ships"; } }

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get {
				return ships;
			}
			set {
				SetProperty (ref ships, value);
				ships.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Ships");
			}
		}

		Command createShip;
		public Command CreateShip {
			get {
				if (createShip == null)
					createShip = new Command (() => {
						MessagingCenter.Subscribe <EditShipViewModel, Ship> (this, "Ship Created", (vm, ship) => {
							Ships.Add (ship);
							Ship.CustomShips.Add (ship);
							Ship.GetAllShips ();
							NavigationService.PopAsync (); // <EditShipViewModel> (vm);
							MessagingCenter.Unsubscribe <EditShipViewModel, Ship> (this, "Ship Created");
						});

						NavigationService.PushAsync (new EditShipViewModel { Ship = new Ship (), Create = true });
					});

				return createShip;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Ships = new ObservableCollection <Ship> (Ship.CustomShips);

			NotifyPropertyChanged ("Ships");
		}
	}
}

