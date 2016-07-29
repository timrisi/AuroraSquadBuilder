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

			MessagingCenter.Subscribe<PilotViewModel, List<string>> (this, "Select Upgrade Option", async (vm, options) => {
				var upgrade = await DisplayActionSheet ("Select Upgrade Type", "Cancel", null, options.ToArray ());
				MessagingCenter.Send (this, "Upgrade Option Selected", upgrade);
				MessagingCenter.Unsubscribe <PilotViewModel, List<string>> (this, "Select Upgrade Option");
			});

			MessagingCenter.Subscribe <PilotViewModel> (this, "Select Scyk Upgrade", async vm => {
				var upgrade = await DisplayActionSheet ("Select Upgrade Type", "Cancel", null, "Cannon", "Torpedo", "Missile");
				MessagingCenter.Send <UpgradesListView, string> (this, "Scyk Upgrade Selected", upgrade);
				MessagingCenter.Unsubscribe <PilotViewModel> (this, "Select Scyk Upgrade");
			});
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			MessagingCenter.Unsubscribe <PilotViewModel> (this, "Select Scyk Upgrade");
		}
	}
}

