using System;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public class App : Application
	{
		public App ()
		{
			RegisterViews();
			MainPage = new RootPage ();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			MessagingCenter.Send <App> (this, "Save Squadrons");
		}

		protected override void OnResume ()
		{
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
		}
	}
}

