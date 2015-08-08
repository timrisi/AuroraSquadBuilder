using System;
using System.Collections.ObjectModel;
using XLabs.Data;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using XLabs;
using System.Linq;
using System.Xml.Serialization;

namespace SquadBuilder
{
	public class Upgrade : ObservableObject
	{
		public Upgrade ()
		{
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public int Cost { get; set; }
		public Ship Ship { get; set; }
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
		public bool Preview { get; set; }

		Faction faction;
		public Faction Faction {
			get { return faction; }
			set { 
				SetProperty (ref faction, value);
				NotifyPropertyChanged ("FactionRestricted");
			}
		}

		public bool FactionRestricted {
			get { return Faction != null; }
		}

		public ObservableCollection <string> Slots { get; set; }
		public ObservableCollection <string> AdditionalUpgrades { get; set; }

		[XmlIgnore]
		public RelayCommand deleteUpgrade;
		[XmlIgnore]
		public RelayCommand DeleteUpgrade {
			get {
				if (deleteUpgrade == null)
					deleteUpgrade = new RelayCommand (() => {
						XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

						var categoryElement = customUpgradesXml.Elements ().FirstOrDefault (e => e.Attribute ("type")?.Value == Category);

						if (categoryElement == null)
							return;
						
						var upgradeElement = categoryElement.Elements ().FirstOrDefault (e => e.Element ("Name")?.Value == Name);

						if (upgradeElement == null)
							return;

						upgradeElement.Remove ();

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customUpgradesXml.ToString ());

						MessagingCenter.Send <Upgrade> (this, "Remove Upgrade");
					});

				return deleteUpgrade;
			}
		}
	}
}

