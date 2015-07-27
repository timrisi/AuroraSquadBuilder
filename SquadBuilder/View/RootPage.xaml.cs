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
			Detail = new NavigationPage (MainView);

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Custom Factions", vm => {
				IsPresented = false;
				Detail = new NavigationPage (CustomFactionsView);
			});

			MessagingCenter.Subscribe <MenuViewModel> (this, "Show Squadrons", vm => {
				IsPresented = false;
				Detail = new NavigationPage (MainView);
			});
		}

		MainView mainView;
		public MainView MainView {
			get { 
				if (mainView == null)
					mainView = new MainView ();

				return mainView;
			}
		}

		CustomFactionsView customFactionsView;
		public CustomFactionsView CustomFactionsView {
			get { 
				if (customFactionsView == null)
					customFactionsView = new CustomFactionsView ();

				return customFactionsView;
			}
		}
	}
}

