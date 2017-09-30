using System;
using System.Collections.ObjectModel;
using XLabs.Data;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using XLabs;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class Upgrade : ObservableObject
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string CategoryId { get; set; }

		public static Dictionary<string, string> UpgradeSymbolDictionary = new Dictionary<string, string> {
			{ "Astromech Droid", "A" },
			{ "Bomb", "B" },
			{ "Cannon", "C" },
			{ "Cargo", "G" },
			{ "Crew", "W" },
			{ "Elite Pilot Talent", "E" },
			{ "Hardpoint", "H"},
			{ "Illicit", "I" },
			{ "Missile", "M" },
			{ "Modification", "m" },
			{ "Salvaged Astromech", "V" },
			{ "System Upgrade", "S" },
			{ "Team", "T" },
			{ "Tech", "X" },
			{ "Title", "t" },
			{ "Torpedo", "P" },
			{ "Turret Weapon", "U" },
		};

		public static object CreateUpgradeSlot (string upgradeType) 
		{
			string symbol = "";

			UpgradeSymbolDictionary.TryGetValue(upgradeType, out symbol);

			return new { Name = upgradeType, Symbol = symbol, IsUpgrade = false };
		}

		string category;
		public string Category
		{
			get { return category; }
			set
			{
				SetProperty(ref category, value);

				Symbol = GetSymbol(category);
			}
		}

		public static string GetSymbol (string category)
		{
			string symbol = "";

			UpgradeSymbolDictionary.TryGetValue(category, out symbol);

			return symbol;
		}

		public string Symbol { get; set; }

		int cost;
		public int Cost { 
			get {
				if (Pilot?.UpgradesEquipped?.Any (u => u?.Name.Contains("Vaksai") ?? false) ?? false)
					return Math.Max (0, cost - 1);
				return cost;
			}
			set {
				cost = value;
			} 
		}

		public bool SmallOnly { get; set; }
		public bool LargeOnly { get; set; }
		public bool HugeOnly { get; set; }
		public string Text { get; set; }
		public int PilotSkill { get; set; }
		public int Energy { get; set; }
		public int Attack { get; set; }
		public int Agility { get; set; }
		public int Hull { get; set; }
		public int Shields { get; set; }
		public bool SecondaryWeapon { get; set; }
		public int Dice { get; set; }
		public string Range { get; set; }
		public bool Limited { get; set; }

		public bool ShowExtras {
			get {
				return Limited ||
					ShowShipRequirement ||
					SmallOnly ||
					LargeOnly ||
					HugeOnly ||
					FactionRestricted ||
					Preview ||
					IsCustom ||
					CCL ||
					HotAC ||
					ShowEnergy;
			}
		}

		bool unique;
		public bool Unique
		{
			get { return unique; }
			set
			{
				SetProperty(ref unique, value);

				if (unique)
				{
					if (!Name.Contains("•"))
						Name = $"•{Name}";
				}
				else
					Name = Name.Replace("•", "");
			}
		}

		public bool Preview { get; set; }
		public string RequiredAction { get; set; }
		public int MinPilotSkill { get; set; }
		public int? MaxPilotSkill { get; set; }
		public bool IsCustom { get; set; }
		public bool CCL { get; set; }
		public string ModifiedManeuverDial { get; set; }
		public int? MinAgility { get; set; }
		public int? MaxAgility { get; set; }
		public int? ShieldRequirement { get; set; }
		public bool HotAC { get; set; }
		public Pilot Pilot { get; set; }

		public Ship Ship { get; set; }

		string canonicalName;
		public string CanonicalName {
			get { return canonicalName ?? Id; }
			set {
				SetProperty (ref canonicalName, value);
			}
		}

		string oldId;
		public string OldId {
			get { return oldId; }
			set { SetProperty (ref oldId, value); }
		}

		[XmlIgnore]
		public int owned;
		[XmlIgnore]
		public int Owned { 
			get { return owned; }
			set {
				if (value < 0)
					value = 0;
				
				SetProperty (ref owned, value);

				var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
				var upgradesElement = collectionXml.Element ("Upgrades");

				var upgradesOwned = Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.Id == Id).Owned;

				if (upgradesElement.Elements ().Any (e => e.Attribute ("id").Value == Id)) {
					if (upgradesOwned == 0)
						upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id)?.Remove ();
					else
						upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id).SetValue (upgradesOwned);
				} else {
					var element = new XElement ("Upgrade", upgradesOwned);
					element.SetAttributeValue ("id", Id);
					upgradesElement.Add (element);
				}

				DependencyService.Get<ISaveAndLoad> ().SaveText ("Collection.xml", collectionXml.ToString ());
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

		//Faction faction;
		//public Faction Faction {
		//	get { return faction; }
		//	set { 
		//		SetProperty (ref faction, value);
		//		NotifyPropertyChanged ("FactionRestricted");
		//	}
		//}

		List<Faction> factions = new List<Faction>();
		public List<Faction> Factions {
			get { return factions; }
			set {
				SetProperty (ref factions, value);
				NotifyPropertyChanged ("FactionRestricted");
				NotifyPropertyChanged ("FactionsString");
			}
		}

		public string FactionsString {
			get {
				return Factions != null ? string.Join (", ", Factions) : null;
			}
		}

		public bool FactionRestricted {
			get { return Factions != null && Factions.Count > 0; }
		}

		public bool ShowEnergy {
			get { return Energy != 0; }
		}

		public ObservableCollection<string> Slots { get; set; } = new ObservableCollection<string>();
		public ObservableCollection <string> AdditionalUpgrades { get; set; } = new ObservableCollection<string>();
		public ObservableCollection <string> AdditionalActions { get; set; } = new ObservableCollection<string>();
		public ObservableCollection <string> RemovedUpgrades { get; set; } = new ObservableCollection<string>();
		public ObservableCollection <string> RequiredSlots { get; set; } = new ObservableCollection<string>();
		public ObservableCollection <string> UpgradeOptions { get; set; } = new ObservableCollection<string>();

		public Color TextColor {
			get { return IsAvailable ? Color.Black : Color.Gray; }
		}

		public bool IsAvailable {
			get {
				var count = 0;

				if (Cards.SharedInstance.CurrentSquadron == null)
					return Owned > 0;
				
				foreach (var pilot in Cards.SharedInstance.CurrentSquadron.Pilots)
					count += pilot.UpgradesEquipped.Count (u => u != null && u.Id == Id);

				if (Unique && count > 0)
					return false;
				
				if (Cards.SharedInstance.Upgrades.Sum (u => u.Owned) == 0)
					return true;
				
				return Owned > count;
			}
		}

		[XmlIgnore]
		string expansions;
		public string Expansions {
			get {
				if (string.IsNullOrEmpty (expansions))
					expansions = string.Join (", ", Cards.SharedInstance.Expansions.Where (e => e.Upgrades.Any (u => u == Id)).Select (e => e.Name));

				return expansions;
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
				CanonicalName = CanonicalName,
				OldId = OldId,
				Category = Category,
				CategoryId = CategoryId,
				Cost = cost,
				ShipRequirement = ShipRequirement,
				Factions = Factions != null ? new List<Faction>(Factions) : null,
				SmallOnly = SmallOnly,
				LargeOnly = LargeOnly,
				HugeOnly = HugeOnly,
				Text = Text,
				PilotSkill = PilotSkill,
				Energy = Energy,
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
				MinPilotSkill = MinPilotSkill,
				MaxPilotSkill = MaxPilotSkill,
				MinAgility = MinAgility,
				MaxAgility = MaxAgility,
				ShieldRequirement = ShieldRequirement,
				IsCustom = IsCustom,
				CCL = CCL,
				ModifiedManeuverDial = ModifiedManeuverDial,
				Slots = new ObservableCollection<string> (Slots),
				AdditionalUpgrades = new ObservableCollection<string> (AdditionalUpgrades),
				AdditionalActions = new ObservableCollection<string> (AdditionalActions),
				RemovedUpgrades = new ObservableCollection<string> (RemovedUpgrades),
				RequiredSlots = new ObservableCollection<string> (RequiredSlots),
				UpgradeOptions = new ObservableCollection<string> (UpgradeOptions),
				HotAC = HotAC,
				Pilot = Pilot,
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

		public override bool Equals (object obj)
		{
			if (!(obj is Upgrade))
				return false;

			var upgrade = obj as Upgrade;
			return Id == upgrade.Id && 
				CategoryId == upgrade.CategoryId;
		}

		public override int GetHashCode ()
		{
			return ((Id + CategoryId).GetHashCode ());
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

