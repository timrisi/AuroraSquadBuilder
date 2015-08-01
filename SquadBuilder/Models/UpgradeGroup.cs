using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class UpgradeGroup : ObservableCollection <Upgrade>
	{
		public string Category { get; set; }

		public UpgradeGroup ()
		{
		}

		public UpgradeGroup (string category)
		{
			Category = category;
		}
	}
}


