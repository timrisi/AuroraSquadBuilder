using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class SettingsViewModel : ViewModel
	{
		public bool AllowCustom {
			get { return Settings.AllowCustom; }
			set {
				Settings.AllowCustom = value;
				NotifyPropertyChanged ("AllowCustom");
			}
		}
				
		public bool FilterPilotsByShip {
			get { return Settings.FilterPilotsByShip; }
			set {
				Settings.FilterPilotsByShip = value;
				NotifyPropertyChanged ("FilterPilotsByShip");
			}
		}

		public bool UpdateOnLaunch {
			get { return Settings.UpdateOnLaunch; }
			set {
				Settings.UpdateOnLaunch = value;
				NotifyPropertyChanged ("UpdateOnLaunch");
			}
		}

		RelayCommand checkForUpdates;
		public RelayCommand CheckForUpdates {
			get {
				if (checkForUpdates == null)
					checkForUpdates = new RelayCommand (() => {
						Settings.CheckForUpdates ();
					});	

				return checkForUpdates;
			}
		}
	}
}