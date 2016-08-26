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

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Collection", vm => {
				IsPresented = false;
				Detail = CollectionView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Reference Cards", vm => {
				IsPresented = false;
				Detail = ReferenceCardsListView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Explore Cards", vm => {
				IsPresented = false;
				Detail = ExploreCardsView;
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
				if (mainView == null) {
					var tb = new TabbedPage ();
					tb.Title = "Squadrons";
					tb.Children.Add (new MainView ());
					var factions = Settings.AllowCustom ? Cards.SharedInstance.AllFactions : Cards.SharedInstance.Factions;
					foreach (var faction in factions)
						tb.Children.Add (new MainView (faction.Name));

					mainView = new NavigationPage (tb);
				}

				return mainView;
			}
		}

		NavigationPage collectionView;
		public NavigationPage CollectionView {
			get {
				if (collectionView == null)
					collectionView = new NavigationPage (new CollectionView ());

				return collectionView;
			}
		}

		NavigationPage exploreCardsView;
		public NavigationPage ExploreCardsView {
			get {
				if (exploreCardsView == null)
					exploreCardsView = new NavigationPage (new ExploreCardsView ());

				return exploreCardsView;
			}
		}

		NavigationPage referenceCardsListView;
		public NavigationPage ReferenceCardsListView {
			get {
				if (referenceCardsListView == null)
					referenceCardsListView = new NavigationPage (new ReferenceCardListView ());

				return referenceCardsListView;
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

