using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class CreateSquadronView : BaseView
	{
		public CreateSquadronView ()
		{
			InitializeComponent ();

			MessagingCenter.Subscribe <CreateSquadronViewModel> (this, "Negative Squad Points", vm => {
				DisplayAlert ("Invalid Point Value", "Squadron Points value cannot be negative", "Okay");
			});
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			CreateSquadronViewModel vm = BindingContext as CreateSquadronViewModel;
			if (vm != null) {
				factionPicker.Items.Clear();
				foreach (var faction in vm.Factions)
				{
					factionPicker.Items.Add(faction.Name);
				}
				factionPicker.SelectedIndex = 0;
			}
		}
	}
}

