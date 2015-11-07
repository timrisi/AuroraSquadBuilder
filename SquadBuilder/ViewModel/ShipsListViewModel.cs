﻿using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using SquadBuilder.iOS;
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

		ObservableCollection <PilotGroup> pilotGroups = new ObservableCollection <PilotGroup> ();
		public ObservableCollection <PilotGroup> PilotGroups {
			get {
				return pilotGroups;
			}
			set {
				SetProperty (ref pilotGroups, value);
			}
		}

		Ship selectedShip = null;
		public Ship SelectedShip {
			get { return selectedShip; }
			set { 
				SetProperty (ref selectedShip, value); 

				if (value != null)
					Navigation.PushAsync <PilotsListViewModel> ((vm, p) => {
						vm.Faction = Faction;
						vm.Ship = selectedShip;
					});
//				if (value != null)
//					MessagingCenter.Send <PilotsListViewModel, Pilot> (this, "Ship selected", SelectedPilot);
			}
		}

		void GetAllShips ()
		{
			if (Settings.AllowCustom)
				allShips = Cards.SharedInstance.AllShips;
			else
				allShips = Cards.SharedInstance.Ships;
		}

		void filterShips ()
		{
			if (faction == null || faction.Name == "Mixed")
				return;

			ObservableCollection <Pilot> pilots;
			if (Settings.AllowCustom)
				pilots = Cards.SharedInstance.AllPilots;
			else
				pilots = Cards.SharedInstance.Pilots;

			Ships = new ObservableCollection <Ship> (allShips.Where (s => pilots.Count (p => p.Faction == Faction && p.Ship == s) > 0).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge));
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllShips ();

			filterShips ();
		}
	}
}
