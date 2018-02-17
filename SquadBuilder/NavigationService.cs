using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SquadBuilder
{
	public static class NavigationService {
		public static async Task PushAsync (ViewModel viewModel)
		{
			var view = SimpleIoC.GetPage (viewModel.GetType ());
			view.BindingContext = viewModel;
			await Navigation.PushAsync (view, true);
		}

		public static async Task PopAsync (bool animated = true)
		{
			await Navigation.PopAsync (animated);
		}

		public static async Task PushModalAsync (ViewModel viewModel, bool wrapInNavigation = true)
		{
			var view = SimpleIoC.GetPage (viewModel.GetType ());
			view.BindingContext = viewModel;
			await Navigation.PushModalAsync (wrapInNavigation ? new NavigationPage (view) : view);
		}

		public static async Task PopModalAsync ()
		{
			await Navigation.PopModalAsync (true);
		}

		public static void SetRoot (object viewModel, bool wrapInNavigation = true)
		{
			var view = SimpleIoC.GetPage (viewModel.GetType ());
			view.BindingContext = viewModel;
			Application.Current.MainPage = wrapInNavigation ? new NavigationPage (view) : view;
		}

		static INavigation Navigation {
			get {
				//If the tab page has Navigation controllers as the contents, we need to use those.
				var tabbed = Application.Current.MainPage as TabbedPage;
				if (tabbed != null)
					return tabbed.CurrentPage?.Navigation;

				var masterDetail = Application.Current.MainPage as MasterDetailPage;
				if (masterDetail != null)
					return masterDetail.IsPresented ? masterDetail.Master.Navigation : masterDetail.Detail.Navigation;

				return Application.Current.MainPage.Navigation;
			}
		}
	}
}
