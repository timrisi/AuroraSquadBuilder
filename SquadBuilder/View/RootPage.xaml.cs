using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public partial class RootPage : MasterDetailPage
	{
		public RootPage ()
		{
			InitializeComponent ();
			Master = new MenuView ();
			Detail = MainView;

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Squadrons", vm => {
				IsPresented = false;
				Detail = MainView;
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Custom Factions", vm => {
				IsPresented = false;
				Detail = CustomFactionsView;
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Custom Ships", vm => {
				IsPresented = false;
				Detail = CustomShipsView;
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Custom Pilots", vm => {
				IsPresented = false;
				Detail = CustomPilotsView;
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Custom Upgrades", vm => {
				IsPresented = false;
				Detail = CustomUpgradesView;
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Settings", vm => {
				IsPresented = false;
				Detail = SettingsView;
			});
		}

		NavigationPage mainView;
		public NavigationPage MainView {
			get { 
				if (mainView == null)
					mainView = new NavigationPage (new MainView ());

				return mainView;
			}
		}

		NavigationPage customFactionsView;
		public NavigationPage CustomFactionsView {
			get { 
				if (customFactionsView == null)
					customFactionsView = new NavigationPage (new CustomFactionsView ());

				return customFactionsView;
			}
		}

		NavigationPage customShipsView;
		public NavigationPage CustomShipsView {
			get {
				if (customShipsView == null)
					customShipsView = new NavigationPage (new CustomShipsView ());

				return customShipsView;
			}
		}

		NavigationPage customPilotsView;
		public NavigationPage CustomPilotsView {
			get {
				if (customPilotsView == null)
					customPilotsView = new NavigationPage (new CustomPilotsView ());

				return customPilotsView;
			}
		}

		NavigationPage customUpgradesView;
		public NavigationPage CustomUpgradesView {
			get {
				if (customUpgradesView == null)
					customUpgradesView = new NavigationPage (new CustomUpgradesView ());

				return customUpgradesView;
			}
		}

		NavigationPage settingsView;
		public NavigationPage SettingsView {
			get {
				if (settingsView == null)
					settingsView = new NavigationPage (new SettingsView ());

				return settingsView;
			}
		}
	}
}

