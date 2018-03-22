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
						NavigationService.PushAsync (new ExpansionsViewModel ()).ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
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
						NavigationService.PushAsync (new ShipsCollectionViewModel ()).ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
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
						NavigationService.PushAsync (new PilotsCollectionShipsListViewModel ()).ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
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
						NavigationService.PushAsync (new UpgradesCollectionCategoryListViewModel ()).ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
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