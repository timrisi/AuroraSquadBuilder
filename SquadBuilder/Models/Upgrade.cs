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
		public string Id { get; set; }
		public string Name { get; set; }
		public string CategoryId { get; set; }
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

		int owned;
		public int Owned { 
			get { return owned; }
			set {
				if (value < 0)
					value = 0;
				
				SetProperty (ref owned, value);
			}
		}

		string shipRequirement;
		public string ShipRequirement { 
			get { return shipRequirement; }
			set {
				SetProperty (ref shipRequirement, value);
				NotifyPropertyChanged ("ShowShipRequirement");
			}
		}

		public bool ShowShipRequirement { 
			get { 
				return !string.IsNullOrEmpty (ShipRequirement); 
			} 
		}

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
		public ObservableCollection <string> RemovedUpgrades { get; set; }
		public ObservableCollection <string> RequiredSlots { get; set; }

		public Color TextColor {
			get { return IsAvailable ? Color.Black : Color.Gray; }
		}

		public bool IsAvailable {
			get {
				if (Cards.SharedInstance.Upgrades.Sum (u => u.Owned) == 0)
					return true;

				var count = 0;
				foreach (var pilot in Cards.SharedInstance.CurrentSquadron.Pilots)
					count += pilot.UpgradesEquipped.Count (u => u != null && u.Id == Id);

				if (Unique && count > 0)
					return false;
			
				return Owned > count;
			}
		}

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

		[XmlIgnore]
		RelayCommand editUpgrade;
		[XmlIgnore]
		public RelayCommand EditUpgrade {
			get {
				if (editUpgrade == null)
					editUpgrade = new RelayCommand (() => {
						MessagingCenter.Send <Upgrade> (this, "Edit Upgrade");
					});

				return editUpgrade;
			}
		}

		public Upgrade Copy ()
		{
			return new Upgrade {
				Id = Id,
				Name = Name,
				Category = Category,
				Cost = Cost,
				ShipRequirement = ShipRequirement,
				Faction = Faction,
				SmallOnly = SmallOnly,
				LargeOnly = LargeOnly,
				HugeOnly = HugeOnly,
				Text = Text,
				PilotSkill = PilotSkill,
				Attack = Attack,
				Agility = Agility,
				Hull = Hull,
				Shields = Shields,
				SecondaryWeapon = SecondaryWeapon,
				Dice = Dice,
				Range = Range,
				Limited = Limited,
				Unique = Unique, 
				Preview = Preview,
				Slots = new ObservableCollection <string> (Slots),
				AdditionalUpgrades = new ObservableCollection <string> (AdditionalUpgrades),
				RemovedUpgrades = new ObservableCollection <string> (RemovedUpgrades),
				RequiredSlots = new ObservableCollection <string> (RequiredSlots)
			};
		}

		RelayCommand increment;
		public RelayCommand Increment {
			get {
				if (increment == null)
					increment = new RelayCommand (() => Owned++);

				return increment;
			}
		}

		RelayCommand decrement;
		public RelayCommand Decrement {
			get {
				if (decrement == null)
					decrement = new RelayCommand (() => Owned--);

				return decrement;
			}
		}
	}
}

