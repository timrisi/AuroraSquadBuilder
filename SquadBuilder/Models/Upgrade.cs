using System;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class Upgrade
	{
		public string Name { get; set; }
		public int Cost { get; set; }
		public string Faction { get; set; }
		public bool SmallOnly { get; set; }
		public bool LargeOnly { get; set; }
		public bool HugeOnly { get; set; }
		public string Text { get; set; }
		public int PilotSkill { get; set; }
		public int Attack { get; set; }
		public int Agility { get; set; }
		public int Hull { get; set; }
		public int Shields { get; set; }
		public bool SecondaryWeapon { get; set; }
		public int Dice { get; set; }
		public string Range { get; set; }
		public bool Limited { get; set; }
		public bool Unique { get; set; }

		public ObservableCollection <string> AdditionalUpgrades { get; set; }
	}
}

