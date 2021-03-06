﻿using System;

using Xamarin.Forms;

namespace SquadBuilder
{
	public class RootPageViewModel : ViewModel
	{
		public RootPageViewModel ()
		{
			MasterPage = new MenuView ();
			DetailPage = new NavigationPage (new MainView ());
		}

		MenuView masterPage;
		public MenuView MasterPage {
			get { return masterPage; }
			set { SetProperty (ref masterPage, value); }
		}

		Page detailPage;
		public Page DetailPage {
			get { return detailPage; }
			set { SetProperty (ref detailPage, value); }
		}
	}
}

