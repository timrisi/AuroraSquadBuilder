using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
					var ship = Ship.Ships.ToList ().FirstOrDefault (s => s.Id == shipId);
					if (ship != null)
						Ships.Add (ship);
				}

				foreach (var pilotId in expansion.Pilots) {
					var pilot = Pilot.Pilots.ToList ().FirstOrDefault (p => p.Id == pilotId);
					if (pilot != null)
						Pilots.Add (pilot);
				}

				foreach (var upgradeId in expansion.Upgrades) {
					var upgrade = Upgrade.Upgrades.ToList ().FirstOrDefault (u => u.Id == upgradeId);
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

		Pilot selectedPilot;
		public Pilot SelectedPilot {
			get { return selectedPilot; }
			set {
				SetProperty (ref selectedPilot, value);

				if (selectedPilot != null)
					NavigationService.PushAsync (new SinglePilotViewModel { Pilot = selectedPilot });
			}
		}

		Upgrade selectedUpgrade;
		public Upgrade SelectedUpgrade {
			get { return selectedUpgrade; }
			set {
				SetProperty (ref selectedUpgrade, value);

				if (selectedUpgrade != null)
					NavigationService.PushAsync (new SingleUpgradeViewModel { Upgrade = selectedUpgrade });
			}
		}
	}
}

