using System;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.IO;
using System.Linq;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public class CustomShipsViewModel : ViewModel
	{
		public CustomShipsViewModel ()
		{
			MessagingCenter.Subscribe <Ship> (this, "Remove Ship", ship => {
				Ships.Remove (ship);
				Cards.SharedInstance.CustomShips.Remove (ship);
			});

			MessagingCenter.Subscribe <Ship> (this, "Edit Ship", ship => {
				string shipName = ship.Name;
				MessagingCenter.Subscribe <EditShipViewModel, Ship> (this, "Finished Editing", (vm, updatedShip) => {
						Ships [Ships.IndexOf (ship)] = updatedShip;
						Cards.SharedInstance.CustomShips [Cards.SharedInstance.CustomShips.IndexOf (ship)] = updatedShip;

					Navigation.RemoveAsync <EditShipViewModel> (vm);
					NotifyPropertyChanged ("Ships");
					MessagingCenter.Unsubscribe <EditShipViewModel, Ship> (this, "Finished Editing");
				});

				Navigation.PushAsync<EditShipViewModel> ((vm, p) => {
					vm.Ship = ship.Copy ();
				});
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

		RelayCommand createShip;
		public RelayCommand CreateShip {
			get {
				if (createShip == null)
					createShip = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateShipViewModel, Ship> (this, "Ship Created", (vm, ship) => {
							Ships.Add (ship);
							Cards.SharedInstance.CustomShips.Add (ship);
							Navigation.RemoveAsync <CreateShipViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateShipViewModel, Ship> (this, "Ship Created");
						});

						Navigation.PushAsync <CreateShipViewModel> ();
					});

				return createShip;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Ships = new ObservableCollection <Ship> (Cards.SharedInstance.CustomShips);

			NotifyPropertyChanged ("Ships");
		}
	}
}

