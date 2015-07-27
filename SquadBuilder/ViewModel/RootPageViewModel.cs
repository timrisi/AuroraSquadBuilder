using System;
using XLabs.Forms.Mvvm;
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

		Page masterPage;
		public Page MasterPage {
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

