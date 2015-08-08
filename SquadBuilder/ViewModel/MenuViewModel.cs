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
			if (Application.Current.Properties.ContainsKey ("AllowCustom"))
				Application.Current.Properties ["AllowCustom"] = IsToggled;
			else
				Application.Current.Properties.Add ("AllowCustom", IsToggled);
		}

		bool isToggled;
		public bool IsToggled {
			get {
				return isToggled;
			}
			set {
				SetProperty (ref isToggled, value);

				if (Application.Current.Properties.ContainsKey ("AllowCustom"))
					Application.Current.Properties ["AllowCustom"] = IsToggled;
				else
					Application.Current.Properties.Add ("AllowCustom", IsToggled);
			}
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