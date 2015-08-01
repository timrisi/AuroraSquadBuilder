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
			XElement customShipsXML = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			Ships = new ObservableCollection <Ship> (from ship in customShipsXML.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false
				});

			MessagingCenter.Subscribe <Ship> (this, "Remove Ship", ship => {
				Ships.Remove (ship);
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
			}
		}

		RelayCommand createShip;
		public RelayCommand CreateShip {
			get {
				if (createShip == null)
					createShip = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateShipViewModel, Ship> (this, "Ship Created", (vm, ship) => {
							Ships.Add (ship);
							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <CreateShipViewModel, Ship> (this, "Ship Created");
						});

						Navigation.PushAsync <CreateShipViewModel> ();
					});

				return createShip;
			}
		}
	}
}

