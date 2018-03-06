using System;

using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace SquadBuilder
{
	public class CollectionViewModel : ViewModel
	{
		ObservableCollection <string> collectionTypes = new ObservableCollection <string> (new [] {
			"Expansions",
			"Ships",
			"Pilots",
			"Upgrades"
		});
		public ObservableCollection <string> CollectionTypes {
			get { return collectionTypes; }
			set { SetProperty (ref collectionTypes, value); }
		}
	
		Command editExpansions;
		public Command EditExpansions {
			get {
				if (editExpansions == null) {
					editExpansions = new Command (() => {
						NavigationService.PushAsync (new ExpansionsViewModel ());
					});
				}

				return editExpansions;
			}
		}

		Command editShips;
		public Command EditShips {
			get {
				if (editShips == null) {
					editShips = new Command (() => {
						NavigationService.PushAsync (new ShipsCollectionViewModel ());
					});
				}

				return editShips;
			}
		}

		Command editPilots;
		public Command EditPilots {
			get {
				if (editPilots == null) {
					editPilots = new Command (() => {
						NavigationService.PushAsync (new PilotsCollectionShipsListViewModel ());
					});
				}

				return editPilots;
			}
		}

		Command editUpgrades;
		public Command EditUpgrades {
			get {
				if (editUpgrades == null) {
					editUpgrades = new Command (() => {
						NavigationService.PushAsync (new UpgradesCollectionCategoryListViewModel ());
					});
				}

				return editUpgrades;
			}
		}

		Command clearCollection;
		public Command ClearCollection {
			get {
				if (clearCollection == null) {
					clearCollection = new Command (() => {
						MessagingCenter.Send <CollectionViewModel> (this, "Clear Collection");
					});
				}

				return clearCollection;
			}
		}
	}
}