using System;


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

		Command showSquadrons;
		public Command ShowSquadrons {
			get {
				if (showSquadrons == null)
					showSquadrons = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Squadrons");
					});

				return showSquadrons;
			}
		}

		Command showCollection;
		public Command ShowCollection {
			get {
				if (showCollection == null)
					showCollection = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Collection");
					});

				return showCollection;
			}
		}

		Command showReferenceCards;
		public Command ShowReferenceCards {
			get {
				if (showReferenceCards == null)
					showReferenceCards = new Command (() => {
						MessagingCenter.Send<MenuViewModel> (this, "Show Reference Cards");
					});

				return showReferenceCards;
			}
		}

		Command showExploreCards;
		public Command ShowExploreCards {
			get {
				if (showExploreCards == null)
					showExploreCards = new Command (() => {
						MessagingCenter.Send<MenuViewModel> (this, "Show Explore Cards");
					});

				return showExploreCards;
			}
		}

		Command showCustomCards;
		public Command ShowCustomCards {
			get {
				if (showCustomCards == null) {
					showCustomCards = new Command (() => {
						MessagingCenter.Send<MenuViewModel> (this, "Show Custom Cards");
					});
				}

				return showCustomCards;
			}
		}

		Command showCustomFactions;
		public Command ShowCustomFactions {
			get {
				if (showCustomFactions == null)
					showCustomFactions = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Factions");
					});

				return showCustomFactions;
			}
		}

		Command showCustomShips;
		public Command ShowCustomShips {
			get {
				if (showCustomShips == null)
					showCustomShips = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Ships");
					});

				return showCustomShips;
			}
		}

		Command showCustomPilots;
		public Command ShowCustomPilots {
			get {
				if (showCustomPilots == null)
					showCustomPilots = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Pilots");
					});

				return showCustomPilots;
			}
		}

		Command showCustomUpgrades;
		public Command ShowCustomUpgrades {
			get {
				if (showCustomUpgrades == null)
					showCustomUpgrades = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Custom Upgrades");
					});

				return showCustomUpgrades;
			}
		}

		Command showSettings;
		public Command ShowSettings {
			get {
				if (showSettings == null)
					showSettings = new Command (() => {
						MessagingCenter.Send <MenuViewModel> (this, "Show Settings");
					});

				return showSettings;
			}
		}

		Command sendFeedback;
		public Command SendFeedback {
			get {
				if (sendFeedback == null)
					sendFeedback = new Command (() => {
						DependencyService.Get <ISendMail> ().SendFeedback ();
					});

				return sendFeedback;
			}
		}
	}
}