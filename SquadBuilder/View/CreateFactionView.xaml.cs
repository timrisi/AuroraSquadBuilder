using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SquadBuilder
{
	public partial class CreateFactionView : ContentPage
	{
		public CreateFactionView ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			CreateFactionViewModel vm = BindingContext as CreateFactionViewModel;
			if (vm != null) {
				colorPicker.Items.Clear();
				foreach (var color in vm.Colors)
				{
					colorPicker.Items.Add(color);
				}
				colorPicker.SelectedIndex = vm.SelectedIndex;
			}
		}
	}
}

