using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;

#if __IOS__
using Dropbox.CoreApi.iOS;
using UIKit;
#endif

namespace SquadBuilder
{
	public class SettingsViewModel : ViewModel
	{
		public bool AllowCustom {
			get { return Settings.AllowCustom; }
			set {
				Settings.AllowCustom = value;
				NotifyPropertyChanged ("AllowCustom");
				MessagingCenter.Send <SettingsViewModel> (this, "AllowCustom changed");
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

		public bool HideUnavailable {
			get { return Settings.HideUnavailable; }
			set {
				Settings.HideUnavailable = value;
				NotifyPropertyChanged ("HideUnavailable");
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

//		RelayCommand dropboxLogin;
//		public RelayCommand DropboxLogin {
//			get {
//				if (dropboxLogin == null)
//					dropboxLogin = new RelayCommand (() => {
//						if (!Session.SharedSession.IsLinked) {
//							// Ask for linking the app
//#if __IOS__
//							Session.SharedSession.LinkFromController (UIApplication.SharedApplication.KeyWindow.RootViewController);
//#endif
//						}
//					});	
//
//				return dropboxLogin;
//			}
//		}
	}
}