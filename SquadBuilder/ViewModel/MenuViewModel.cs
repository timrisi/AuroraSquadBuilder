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
	}
}

