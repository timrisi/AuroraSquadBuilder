using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SquadBuilder
{
	public class Expansion
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Wave { get; set; }
		public List <string> Ships { get; set; }
		public List <string> Pilots { get; set; }
		public List <string> Upgrades { get; set; }

		public int owned;
		public int Owned { 
			get { return owned; }
			set {
				if (value == owned)
					return;
				
				var previousNumber = owned;
				owned = value;

				foreach (var ship in Ships)
					Cards.SharedInstance.Ships.FirstOrDefault (s => s.Id == ship).Owned += (owned - previousNumber);

				foreach (var pilot in Pilots)
					Cards.SharedInstance.Pilots.FirstOrDefault (p => p.Id == pilot).Owned += (owned - previousNumber);

				foreach (var upgrade in Upgrades) {
					if (upgrade == "r2d2") {
						if (Id == "coreset")
							Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.Id == upgrade && u.CategoryId == "amd").Owned += (owned - previousNumber);
						else if (Id == "tantiveiv")
							Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.Id == upgrade && u.CategoryId == "crew").Owned += (owned - previousNumber);
						continue;
					}

					Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.Id == upgrade).Owned += (owned - previousNumber);
				}
			}
		}
	}
}