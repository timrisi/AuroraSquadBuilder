using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs.Forms;
using XLabs;
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
	
		RelayCommand editExpansions;
		public RelayCommand EditExpansions {
			get {
				if (editExpansions == null) {
					editExpansions = new RelayCommand (() => {
						Navigation.PushAsync <ExpansionsViewModel> ();
					});
				}

				return editExpansions;
			}
		}

		RelayCommand editShips;
		public RelayCommand EditShips {
			get {
				if (editShips == null) {
					editShips = new RelayCommand (() => {
						Navigation.PushAsync <ShipsCollectionViewModel> ();
					});
				}

				return editShips;
			}
		}

		RelayCommand editPilots;
		public RelayCommand EditPilots {
			get {
				if (editPilots == null) {
					editPilots = new RelayCommand (() => {
						Navigation.PushAsync <PilotsCollectionViewModel> ();
					});
				}

				return editPilots;
			}
		}

		RelayCommand editUpgrades;
		public RelayCommand EditUpgrades {
			get {
				if (editUpgrades == null) {
					editUpgrades = new RelayCommand (() => {
						Navigation.PushAsync <UpgradesCollectionViewModel> ();
					});
				}

				return editUpgrades;
			}
		}

		RelayCommand clearCollection;
		public RelayCommand ClearCollection {
			get {
				if (clearCollection == null) {
					clearCollection = new RelayCommand (() => {
						MessagingCenter.Send <CollectionViewModel> (this, "Clear Collection");
					});
				}

				return clearCollection;
			}
		}
	}
}