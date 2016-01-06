using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class MenuViewModel : ViewModel
	{
		public MenuViewModel ()
		{
			MessagingCenter.Subscribe <SettingsViewModel> (this, "AllowCustom changed", vm => NotifyPropertyChanged ("AllowCustom"));
		}

		public bool AllowCustom {
			get { return Settings.AllowCustom; }
		}

		RelayCommand showSquadrons;
		public RelayCommand ShowSquadrons {
			get {
				if (showSquadrons == null)
					showSquadrons = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Squadrons");
					});

				return showSquadrons;
			}
		}

		RelayCommand showCollection;
		public RelayCommand ShowCollection {
			get {
				if (showCollection == null)
					showCollection = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Collection");
					});

				return showCollection;
			}
		}

		RelayCommand showCustomFactions;
		public RelayCommand ShowCustomFactions {
			get {
				if (showCustomFactions == null)
					showCustomFactions = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Factions");
					});

				return showCustomFactions;
			}
		}

		RelayCommand showCustomShips;
		public RelayCommand ShowCustomShips {
			get {
				if (showCustomShips == null)
					showCustomShips = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Ships");
					});

				return showCustomShips;
			}
		}

		RelayCommand showCustomPilots;
		public RelayCommand ShowCustomPilots {
			get {
				if (showCustomPilots == null)
					showCustomPilots = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Pilots");
					});

				return showCustomPilots;
			}
		}

		RelayCommand showCustomUpgrades;
		public RelayCommand ShowCustomUpgrades {
			get {
				if (showCustomUpgrades == null)
					showCustomUpgrades = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Upgrades");
					});

				return showCustomUpgrades;
			}
		}

		RelayCommand showSettings;
		public RelayCommand ShowSettings {
			get {
				if (showSettings == null)
					showSettings = new RelayCommand (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Settings");
					});

				return showSettings;
			}
		}

		RelayCommand sendFeedback;
		public RelayCommand SendFeedback {
			get {
				if (sendFeedback == null)
					sendFeedback = new RelayCommand (() => {
						DependencyService.Get <ISendMail> ().SendFeedback ();
					});

				return sendFeedback;
			}
		}
	}
}