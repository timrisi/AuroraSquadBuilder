﻿using System;
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

		public PilotGroup (Ship ship, Faction faction, IEnumerable<Pilot> pilots) : base (pilots)
		{
			Ship = ship.Copy ();
			Faction = faction;
		}

		public Ship Ship { get; set; }
		public Faction Faction { get; set; }

		public PilotGroup (Ship ship)
		{
			Ship = ship.Copy ();
		}

		public string Header {
			get { return Ship?.Name + " - " + Faction?.Name; }
		}

		public bool ShowManeuvers {
			get {
				return Settings.ShowManeuversInPilotSelection && !string.IsNullOrEmpty (Ship?.ManeuverGridImage);
			}
		}
	}
}

