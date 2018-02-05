using System;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class ExploreCardsViewModel : ViewModel
	{
		Command viewExpansions;
		public Command ViewExpansions {
			get {
				if (viewExpansions == null) {
					viewExpansions = new Command (() => {
						NavigationService.PushAsync (new ExploreExpansionsViewModel ());
					});
				}

				return viewExpansions;
			}
		}

		Command viewShipsAndPilots;
		public Command ViewShipsAndPilots {
			get {
				if (viewShipsAndPilots == null) {
					viewShipsAndPilots = new Command (() => {
						NavigationService.PushAsync (new ExploreShipsViewModel ());
					});
				}

				return viewShipsAndPilots;
			}
		}

		Command viewUpgrades;
		public Command ViewUpgrades {
			get {
				if (viewUpgrades == null) {
					viewUpgrades = new Command (() => {
						NavigationService.PushAsync (new ExploreUpgradesCategoryListViewModel ());
					});
				}

				return viewUpgrades;
			}
		}
	}
}

