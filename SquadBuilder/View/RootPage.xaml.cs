using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;

namespace SquadBuilder {
	public partial class RootPage : MasterDetailPage {
		public RootPage ()
		{
			InitializeComponent ();
			Master = new MenuView ();
			Detail = MainView;

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Squadrons", vm => {
				IsPresented = false;
				Detail = MainView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Collection", vm => {
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

			MessagingCenter.Subscribe<App> (this, "Show Browse Cards", vm => {
				IsPresented = false;
				Detail = ExploreCardsView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Custom Cards", vm => {
				IsPresented = false;
				Detail = CustomView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Custom Factions", vm => {
				IsPresented = false;
				Detail = CustomFactionsView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Custom Ships", vm => {
				IsPresented = false;
				Detail = CustomShipsView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Custom Pilots", vm => {
				IsPresented = false;
				Detail = CustomPilotsView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Custom Upgrades", vm => {
				IsPresented = false;
				Detail = CustomUpgradesView;
			});

			MessagingCenter.Subscribe<MenuViewModel> (this, "Show Settings", vm => {
				IsPresented = false;
				Detail = SettingsView;
			});
		}

		NavigationPage mainView;
		public NavigationPage MainView {
			get {
				if (mainView == null) {
					mainView = new NavigationPage (new MainView ());
					var tb = new TabbedPage ();
					tb.Title = "Squadrons";
					tb.Children.Add (new MainView ());
					var factions = Settings.AllowCustom ? Faction.AllFactions : Faction.Factions;
					foreach (var faction in factions) {
						var factionView = new MainView (faction.Name);

						tb.Children.Add (factionView);
						//if (faction.Name == "Imperial")
						//tb.CurrentPage = factionView;
					}

					//if (tb.Children.FirstOrDefault (p => (p as MainView)?))
					////tb.CurrentPage = tb.Children [factions.IndexOf(factions.First(f => f.Name == "Imperial")) + 1];
					mainView = new NavigationPage (tb);
				}

				return mainView;
			}
		}

		NavigationPage collectionView;
		public NavigationPage CollectionView {
			get {
				if (collectionView == null) {
					var tb = new TabbedPage {
						Title = "Collection"
					};
					tb.ToolbarItems.Add (new ToolbarItem ("Clear Collection", null, async () => {
						var accept = await DisplayAlert ("Clear Collection Info", "Are you sure you want to erase all collection info?", "Ok", "Cancel");

						if (!accept)
							return;

						foreach (var expansion in Expansion.Expansions)
							expansion.Owned = 0;

						foreach (var ship in Ship.Ships)
							ship.Owned = 0;

						foreach (var pilot in Pilot.Pilots)
							pilot.Owned = 0;

						foreach (var upgrade in Upgrade.Upgrades)
							upgrade.Owned = 0;
					}, ToolbarItemOrder.Secondary));

					tb.Children.Add (new ExpansionsView ());
					tb.Children.Add (new ShipsCollectionView { BindingContext = new ShipsCollectionViewModel () });
					tb.Children.Add (new PilotsCollectionShipsListView { BindingContext = new PilotsCollectionShipsListViewModel () });
					tb.Children.Add (new UpgradesCollectionCategoryListView { BindingContext = new UpgradesCollectionCategoryListViewModel () });

					collectionView = new NavigationPage (tb);
				}

				return collectionView;
			}
		}

		NavigationPage exploreCardsView;
		public NavigationPage ExploreCardsView {
			get {
				if (exploreCardsView == null) {
					var tb = new TabbedPage {
						Title = "Browse Cards"
					};

					tb.Children.Add (new ExploreExpansionsView { BindingContext = new ExploreExpansionsViewModel () });
					tb.Children.Add (new ExploreShipsView { BindingContext = new ExploreShipsViewModel () });
					tb.Children.Add (new ExploreUpgradesCategoryListView { BindingContext = new ExploreUpgradesCategoryListViewModel () });

					exploreCardsView = new NavigationPage (tb);
				}
				//exploreCardsView = new NavigationPage (new ExploreCardsView ());

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

		NavigationPage customView;
		public NavigationPage CustomView {
			get {
				if (customView == null) {
					var tb = new TabbedPage ();
					tb.Title = "Custom Cards";
					tb.Children.Add (new CustomFactionsView ());
					tb.Children.Add (new CustomShipsView ());
					tb.Children.Add (new CustomPilotsView ());
					tb.Children.Add (new CustomUpgradesView ());

					customView = new NavigationPage (tb);
				}

				return customView;
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

