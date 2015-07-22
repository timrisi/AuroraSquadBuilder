using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class UpgradesListView : BaseView
	{
		public UpgradesListView ()
		{
			InitializeComponent ();

			MessagingCenter.Subscribe <PilotViewModel> (this, "Select Scyk Upgrade", async vm => {
				var upgrade = await DisplayActionSheet ("Select Upgrade Type", "Cancel", null, "Cannon", "Torpedo", "Missile");
				MessagingCenter.Send <UpgradesListView, string> (this, "Scyk Upgrade Selected", upgrade);
				MessagingCenter.Unsubscribe <PilotViewModel> (this, "Select Scyk Upgrade");
			});
		}
	}
}

