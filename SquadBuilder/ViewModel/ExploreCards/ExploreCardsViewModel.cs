using System;
using XLabs;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public class ExploreCardsViewModel : ViewModel
	{
		RelayCommand viewExpansions;
		public RelayCommand ViewExpansions {
			get {
				if (viewExpansions == null) {
					viewExpansions = new RelayCommand (() => {
						Navigation.PushAsync<ExploreExpansionsViewModel> ();
					});
				}

				return viewExpansions;
			}
		}

		RelayCommand viewShipsAndPilots;
		public RelayCommand ViewShipsAndPilots {
			get {
				if (viewShipsAndPilots == null) {
					viewShipsAndPilots = new RelayCommand (() => {
						Navigation.PushAsync<ExploreShipsViewModel> ();
					});
				}

				return viewShipsAndPilots;
			}
		}

		RelayCommand viewUpgrades;
		public RelayCommand ViewUpgrades {
			get {
				if (viewUpgrades == null) {
					viewUpgrades = new RelayCommand (() => {
						Navigation.PushAsync<ExploreUpgradesCategoryListViewModel> ();
					});
				}

				return viewUpgrades;
			}
		}
	}
}

