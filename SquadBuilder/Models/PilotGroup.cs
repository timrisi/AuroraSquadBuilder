using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class PilotGroup : ObservableCollection <Pilot>
	{
		public PilotGroup ()
		{
		}

		public Ship Ship { get; set; }
		public Faction Faction { get; set; }

		public PilotGroup (Ship ship)
		{
			Ship = ship;
		}

		public string Header {
			get { return Ship?.Name + " - " + Faction?.Name; }
		}
	}
}

