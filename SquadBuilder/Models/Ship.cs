using System;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class Ship
	{
		public Ship ()
		{
		}

		public string Name { get; set; }
		public bool LargeBase { get; set; }
		public bool Huge { get; set; }
		public List <string> Actions { get; set; }
		public string ActionsString { 
			get {
				return string.Join (", ", Actions);
			}
		}
	}
}

