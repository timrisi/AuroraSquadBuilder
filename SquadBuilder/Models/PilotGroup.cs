using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class PilotGroup : ObservableCollection <Pilot>
	{
		public Ship Ship { get; set; }
		public string Faction { get; set; }

		public PilotGroup (Ship ship)
		{
			Ship = ship;
		}
	}
}

