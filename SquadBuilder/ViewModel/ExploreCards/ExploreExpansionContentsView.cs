using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XLabs.Forms.Mvvm;
using System.Linq;

namespace SquadBuilder
{
	public class ExploreExpansionContentsViewModel : ViewModel
	{
		Expansion expansion;
		public Expansion Expansion {
			get { return expansion; }
			set { 
				SetProperty (ref expansion, value);

				if (expansion == null) 
					return;

				foreach (var shipId in expansion.Ships) {
					var ship = Cards.SharedInstance.Ships.ToList ().FirstOrDefault (s => s.Id == shipId);
					if (ship != null)
						Ships.Add (ship);
				}

				foreach (var pilotId in expansion.Pilots) {
					var pilot = Cards.SharedInstance.Pilots.ToList ().FirstOrDefault (p => p.Id == pilotId);
					if (pilot != null)
						Pilots.Add (pilot);
				}

				foreach (var upgradeId in expansion.Upgrades) {
					var upgrade = Cards.SharedInstance.Upgrades.ToList ().FirstOrDefault (u => u.Id == upgradeId);
					if (upgrade != null)
						Upgrades.Add (upgrade);
				}
			}
		}

		ObservableCollection<Ship> ships = new ObservableCollection<Ship> ();
		public ObservableCollection<Ship> Ships {
			get { return ships; }
			set { SetProperty (ref ships, value); }
		}

		ObservableCollection<Pilot> pilots = new ObservableCollection<Pilot> ();
		public ObservableCollection<Pilot> Pilots {
			get { return pilots; }
			set { SetProperty (ref pilots, value); }
		}

		ObservableCollection<Upgrade> upgrades = new ObservableCollection<Upgrade> ();
		public ObservableCollection<Upgrade> Upgrades {
			get { return upgrades; }
			set { SetProperty (ref upgrades, value); }
		}
	}
}

