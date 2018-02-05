using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class BaseView : ContentPage
	{
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			var vm = BindingContext as ViewModel;
			if (vm != null)
				vm.OnViewAppearing ();
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			var vm = BindingContext as ViewModel;
			if (vm != null)
				vm.OnViewDisappearing ();
		}
	}
}
