using System;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using Dropbox.Api;
using System.Runtime.Remoting.Lifetime;

namespace SquadBuilder
{
	public class App : Application
	{
		public static DropboxClient DropboxClient;

		public App ()
		{
			RegisterViews();
			MainPage = new RootPage ();
		}

		protected override async void OnStart ()
		{
			// Handle when your app starts
			if (Settings.UpdateOnLaunch) {
				Settings.CheckForUpdates ();
			}

			if (Application.Current.Properties.ContainsKey (SettingsViewModel.AccessTokenKey)) {
				DropboxClient = new DropboxClient (Application.Current.Properties [SettingsViewModel.AccessTokenKey].ToString ());
				SettingsViewModel.SyncDropbox ();
				var userAccount = await App.DropboxClient.Users.GetCurrentAccountAsync ();
				Application.Current.Properties [SettingsViewModel.AccountKey] = userAccount.Name.DisplayName;
			}
		}

		protected override void OnSleep ()
		{
			MessagingCenter.Send <App> (this, "Save Squadrons");
		}

		protected override void OnResume ()
		{
			if (DropboxClient != null)
				SettingsViewModel.SyncDropbox ();
		}

		void RegisterViews ()
		{
			ViewFactory.Register <MainView, MainViewModel> ();
			ViewFactory.Register <CreateSquadronView, CreateSquadronViewModel> ();
			ViewFactory.Register <SquadronView, SquadronViewModel> ();
			ViewFactory.Register <PilotView, PilotViewModel> ();
			ViewFactory.Register <PilotsListView, PilotsListViewModel> ();
			ViewFactory.Register <UpgradesListView, UpgradesListViewModel> ();
			ViewFactory.Register <EditSquadronView, EditSquadronViewModel> ();
			ViewFactory.Register <MenuView, MenuViewModel> ();
			ViewFactory.Register <RootPage, RootPageViewModel> ();
			ViewFactory.Register <CreateFactionView, CreateFactionViewModel> ();
			ViewFactory.Register <CreateShipView, CreateShipViewModel> ();
			ViewFactory.Register <CreatePilotView, CreatePilotViewModel> ();
			ViewFactory.Register <CreateUpgradeView, CreateUpgradeViewModel> ();
			ViewFactory.Register <EditPilotView, EditPilotViewModel> ();
			ViewFactory.Register <EditShipView, EditShipViewModel> ();
			ViewFactory.Register <EditUpgradeView, EditUpgradeViewModel> ();
			ViewFactory.Register <ShipsListView, ShipsListViewModel> ();
			ViewFactory.Register <ExpansionsView, ExpansionsViewModel> ();
			ViewFactory.Register <ShipsCollectionView, ShipsCollectionViewModel> ();
			ViewFactory.Register <PilotsCollectionView, PilotsCollectionViewModel> ();
			ViewFactory.Register <UpgradesCollectionView, UpgradesCollectionViewModel> ();
			ViewFactory.Register <PilotsCollectionShipsListView, PilotsCollectionShipsListViewModel> ();
			ViewFactory.Register <UpgradesCollectionCategoryListView, UpgradesCollectionCategoryListViewModel> ();
			ViewFactory.Register <ImportView, ImportViewModel> ();
		}
	}
}

