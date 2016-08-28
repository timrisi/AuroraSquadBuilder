﻿using System;
using XLabs.Forms.Mvvm;
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

		public string PageName { get { return "Ships"; } }

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
				if (selectedShip != null) {
					Navigation.PushAsync<ExplorePilotsViewModel> ((vm, p) => {
						vm.Ship = selectedShip.Copy ();
					});
				}
			}
		}

		void GetAllShips ()
		{
			if (Settings.AllowCustom)
				allShips = Cards.SharedInstance.AllShips;
			else
				allShips = new ObservableCollection<Ship> (Cards.SharedInstance.Ships.Where (s => !s.IsCustom));

			if (!Settings.CustomCardLeague)
				allShips = new ObservableCollection<Ship> (allShips.Where (s => !s.CCL));
		}

		void filterShips ()
		{
			ObservableCollection<Pilot> pilots;
			if (Settings.AllowCustom)
				pilots = Cards.SharedInstance.AllPilots;
			else
				pilots = Cards.SharedInstance.Pilots;

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
