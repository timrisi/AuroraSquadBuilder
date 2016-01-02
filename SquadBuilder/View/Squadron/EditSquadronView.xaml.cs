using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class EditSquadronView : BaseView
	{
		public EditSquadronView ()
		{
			InitializeComponent ();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			EditSquadronViewModel vm = BindingContext as EditSquadronViewModel;
			if (vm != null) {
				factionPicker.Items.Clear();
				foreach (var faction in vm.Factions)
				{
					factionPicker.Items.Add(faction.Name);
				}
			}
		}
	}
}

