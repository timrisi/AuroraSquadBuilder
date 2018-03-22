using System;

using Xamarin.Forms;

using Dropbox.Api;
using System.Runtime.Remoting.Lifetime;
using PerpetualEngine.Storage;
using Xamarin;

namespace SquadBuilder
{
	public class App : Application {
		public const string VersionsFilename = "Versions.xml";
		public const string ReferenceCardsFilename = "ReferenceCards.xml";

		public static DropboxClient DropboxClient;
		public static SimpleStorage Storage;

		public App ()
		{
			Storage = SimpleStorage.EditGroup ("AuroraSB");
			Settings.ShowManeuversInShipList = Storage.Get<bool> ("ShowManeuversInShipList", true);
			Settings.ShowManeuversInSquadronList = Storage.Get<bool> ("ShowManeuversInSquadronList", false);
			Settings.ShowManeuversInPilotView = Storage.Get<bool> ("ShowManeuversInPilotView", true);
			Settings.ShowManeuversInPilotSelection = Storage.Get<bool> ("ShowManeuversInPilotSelection", false);
			Settings.CustomCardLeague = Storage.Get<bool> ("CustomCardLeague", false);
			Settings.IncludeHotac = Storage.Get<bool> ("IncludeHotac", false);
			RegisterViews();
			//MainPage = new RootPage ();
			NavigationService.SetRoot (new RootPageViewModel (), false);
		}

		protected override async void OnStart ()
		{
			// Handle when your app starts
			if (Settings.UpdateOnLaunch) {
				Settings.CheckForUpdates ();
			}

			//if (Application.Current.Properties.ContainsKey (SettingsViewModel.AccessTokenKey)) {
			if (App.Storage.HasKey (SettingsViewModel.AccessTokenKey)) {
				try {
					DropboxClient = new DropboxClient (App.Storage.Get<string> (SettingsViewModel.AccessTokenKey));
					await SettingsViewModel.SyncDropbox ();
					var userAccount = await App.DropboxClient.Users.GetCurrentAccountAsync ();
					App.Storage.Put<string> (SettingsViewModel.AccountKey, userAccount.Name.DisplayName);
				} catch (Exception e) {
					Console.WriteLine (e.Message);
					//Insights.Report (e, Insights.Severity.Warning);
				}
			}

			MessagingCenter.Subscribe <Application> (this, "BrowseCards", async (obj) => {
				await ((MainPage as MasterDetailPage).Detail as NavigationPage).PopToRootAsync ();

				MessagingCenter.Send (this, "Show Browse Cards");
			});

			MessagingCenter.Subscribe<Application> (this, "Create Rebel", async (obj) => {
				await ((MainPage as MasterDetailPage).Detail as NavigationPage).PopToRootAsync ();

				MessagingCenter.Send (this, "Create Rebel");
			});

			MessagingCenter.Subscribe<Application> (this, "Create Imperial", async (obj) => {
				await ((MainPage as MasterDetailPage).Detail as NavigationPage).PopToRootAsync ();

				MessagingCenter.Send (this, "Create Imperial");
			});

			MessagingCenter.Subscribe<Application> (this, "Create Scum", async (obj) => {
				await ((MainPage as MasterDetailPage).Detail as NavigationPage).PopToRootAsync ();

				MessagingCenter.Send (this, "Create Scum");
			});
		}

		protected override void OnSleep ()
		{
			MessagingCenter.Send <App> (this, "Save Squadrons");
		}

		protected override void OnResume ()
		{
			if (DropboxClient != null)
				SettingsViewModel.SyncDropbox ().ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
		}

		void RegisterViews ()
		{
			SimpleIoC.RegisterPage<MainViewModel, MainView> ();
			SimpleIoC.RegisterPage<CreateSquadronViewModel, CreateSquadronView> ();
			SimpleIoC.RegisterPage<SquadronViewModel, SquadronView> ();
			SimpleIoC.RegisterPage<PilotViewModel, PilotView> ();
			SimpleIoC.RegisterPage<PilotsListViewModel, PilotsListView> ();
			SimpleIoC.RegisterPage<UpgradesListViewModel, UpgradesListView> ();
			SimpleIoC.RegisterPage<EditSquadronViewModel, EditSquadronView> ();
			SimpleIoC.RegisterPage<MenuViewModel, MenuView> ();
			SimpleIoC.RegisterPage<RootPageViewModel, RootPage> ();
			SimpleIoC.RegisterPage<CreateFactionViewModel, CreateFactionView> ();
			SimpleIoC.RegisterPage<EditPilotViewModel, EditPilotView> ();
			SimpleIoC.RegisterPage<EditShipViewModel, EditShipView> ();
			SimpleIoC.RegisterPage<EditUpgradeViewModel, EditUpgradeView> ();
			SimpleIoC.RegisterPage<ShipsListViewModel, ShipsListView> ();
			SimpleIoC.RegisterPage<ExpansionsViewModel, ExpansionsView> ();
			SimpleIoC.RegisterPage<ShipsCollectionViewModel, ShipsCollectionView> ();
			SimpleIoC.RegisterPage<PilotsCollectionViewModel, PilotsCollectionView> ();
			SimpleIoC.RegisterPage<UpgradesCollectionViewModel, UpgradesCollectionView> ();
			SimpleIoC.RegisterPage<PilotsCollectionShipsListViewModel, PilotsCollectionShipsListView> ();
			SimpleIoC.RegisterPage<UpgradesCollectionCategoryListViewModel, UpgradesCollectionCategoryListView> ();
			SimpleIoC.RegisterPage<ImportViewModel, ImportView> ();
			SimpleIoC.RegisterPage<ReferenceCardViewModel, ReferenceCardView> ();
			SimpleIoC.RegisterPage<ExploreShipsViewModel, ExploreShipsView> ();
			SimpleIoC.RegisterPage<ExplorePilotsViewModel, ExplorePilotsView> ();
			SimpleIoC.RegisterPage<ExploreUpgradesViewModel, ExploreUpgradesView> ();
			SimpleIoC.RegisterPage<ExploreUpgradesCategoryListViewModel, ExploreUpgradesCategoryListView> ();
			SimpleIoC.RegisterPage<ExploreExpansionsViewModel, ExploreExpansionsView> ();
			SimpleIoC.RegisterPage<ExploreExpansionContentsViewModel, ExploreExpansionContentsView> ();
			SimpleIoC.RegisterPage<SinglePilotViewModel, SinglePilotView> ();
			SimpleIoC.RegisterPage<SingleUpgradeViewModel, SingleUpgradeView> ();
		}
	}
}