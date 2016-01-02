using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class PilotView : BaseView
	{
		public PilotView ()
		{
			InitializeComponent ();

			MessagingCenter.Subscribe <PilotViewModel, string[]> (this, "Select Ordnance Tubes Type", async (vm, upgradeTypes) => {
				var upgrade = await DisplayActionSheet ("Select Secondary Weapon Type", "Cancel", null, upgradeTypes);
				MessagingCenter.Send <PilotView, string> (this, "Ordnance Type Selected", upgrade);
			});
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
		}
	}
}

