using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
		public int Owned { get; set; }
	}
}