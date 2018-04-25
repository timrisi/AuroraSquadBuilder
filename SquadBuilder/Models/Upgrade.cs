using System;
using System.Collections.ObjectModel;

using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;

using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class Upgrade : ObservableObject {
		public const string UpgradesFilename = "Upgrades.xml";

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

				if (CategoryId == "ept" && (Pilot?.UpgradesEquipped?.Any (u => u?.Name.Contains ("Renegade Refit") ?? false) ?? false))
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
		public int? SquadLimit { get; set; }

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
		public string Keywords { get; set; } = "";

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

				var upgradesOwned = Upgrade.Upgrades.FirstOrDefault (u => u.Id == Id).Owned;

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

				if (Squadron.CurrentSquadron == null)
					return Owned > 0;
				
				foreach (var pilot in Squadron.CurrentSquadron.Pilots)
					count += pilot.UpgradesEquipped.Count (u => u != null && u.Id == Id);

				if (Unique && count > 0)
					return false;
				
				if (Upgrade.Upgrades.Sum (u => u.Owned) == 0)
					return true;
				
				return Owned > count;
			}
		}

		[XmlIgnore]
		string expansions;
		public string Expansions {
			get {
				if (string.IsNullOrEmpty (expansions))
					expansions = string.Join (", ", Expansion.Expansions.Where (e => e.Upgrades.Any (u => u == Id)).Select (e => e.Name));

				return expansions;
			}
		}

		[XmlIgnore]
		public Command deleteUpgrade;
		[XmlIgnore]
		public Command DeleteUpgrade {
			get {
				if (deleteUpgrade == null)
					deleteUpgrade = new Command (() => {
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
		Command editUpgrade;
		[XmlIgnore]
		public Command EditUpgrade {
			get {
				if (editUpgrade == null)
					editUpgrade = new Command (() => {
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
				Factions = Factions != null ? new List<Faction> (Factions) : null,
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
				SquadLimit = SquadLimit,
				Keywords = Keywords
			};
		}

		Command increment;
		public Command Increment {
			get {
				if (increment == null)
					increment = new Command (() => Owned++);

				return increment;
			}
		}

		Command decrement;
		public Command Decrement {
			get {
				if (decrement == null)
					decrement = new Command (() => Owned--);

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

		#region Static Methods
		public static Dictionary<string, string> CategoryToID = new Dictionary<string, string> {
			{ "Astromech Droid", "amd" },
			{ "Bomb", "bomb" },
			{ "Cannon", "cannon" },
			{ "Cargo", "cargo" },
			{ "Crew", "crew" },
			{ "Elite Pilot Talent", "ept" },
			{ "Hardpoint", "hardpoint" },
			{ "Illicit", "illicit" },
			{ "Missile", "missile" },
			{ "Modification", "mod" },
			{ "Salvaged Astromech", "samd" },
			{ "System Upgrade", "system" },
			{ "Team", "team" },
			{ "Title", "title" },
			{ "Torpedo", "torpedo" },
			{ "Turret Weapon", "turret" },
			{ "Tech", "tech" }
		};

		static ObservableCollection<Upgrade> upgrades;
		public static ObservableCollection<Upgrade> Upgrades {
			get {
				if (upgrades == null)
					GetAllUpgrades ();

				return upgrades;
			}
			set {
				upgrades = value;
				upgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		static ObservableCollection<Upgrade> customUpgrades;
		public static ObservableCollection<Upgrade> CustomUpgrades {
			get {
				if (customUpgrades == null)
					GetAllUpgrades ();

				return customUpgrades;
			}
			set {
				customUpgrades = value;
				customUpgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		static ObservableCollection<Upgrade> allUpgrades;
		public static ObservableCollection<Upgrade> AllUpgrades {
			get {
				if (allUpgrades == null)
					updateAllUpgrades ();

				return allUpgrades;
			}
		}

		static void updateAllUpgrades ()
		{
			var temp = Upgrades.ToList ();
			temp.AddRange (customUpgrades);
			allUpgrades = new ObservableCollection<Upgrade> (temp);
		}

		public static void GetAllUpgrades ()
		{
			if (!DependencyService.Get<ISaveAndLoad> ().FileExists (Upgrade.UpgradesFilename))
				return;

			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var upgradesElement = collectionXml.Element ("Upgrades");

			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Upgrade.UpgradesFilename)));
			List<Upgrade> allUpgrades = new List<Upgrade> ();

			foreach (var category in upgradesXml.Elements ()) {
				var categoryUpgrades = (from upgrade in category.Elements ()
							select new Upgrade {
								Id = upgrade.Attribute ("id").Value,
								Name = upgrade.Element ("Name")?.Value,
								CanonicalName = upgrade.Element ("CanonicalName")?.Value,
								OldId = upgrade.Element ("OldId")?.Value,
								CategoryId = upgrade.Parent.Attribute ("id").Value,
								Category = upgrade.Parent.Attribute ("type")?.Value,
								Cost = (int) upgrade.Element ("Cost"),
								Text = upgrade.Element ("Text")?.Value,
								Factions = Faction.Factions.Where (f => (upgrade.Element ("Faction")?.Value?.Split (',')?.Contains (f.Id) ?? false)).ToList (),
								//Factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
								//Ship = Ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value)?.Copy (),
								ShipRequirement = upgrade.Element ("ShipRequirement")?.Value,
								PilotSkill = upgrade.Element ("PilotSkill") != null ? (int) upgrade.Element ("PilotSkill") : 0,
								Energy = upgrade.Element ("Energy") != null ? (int) upgrade.Element ("Energy") : 0,
								Attack = upgrade.Element ("Attack") != null ? (int) upgrade.Element ("Attack") : 0,
								Agility = upgrade.Element ("Agility") != null ? (int) upgrade.Element ("Agility") : 0,
								Hull = upgrade.Element ("Hull") != null ? (int) upgrade.Element ("Hull") : 0,
								Shields = upgrade.Element ("Shields") != null ? (int) upgrade.Element ("Shields") : 0,
								SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool) upgrade.Element ("SecondaryWeapon") : false,
								Dice = upgrade.Element ("Dice") != null ? (int) upgrade.Element ("Dice") : 0,
								Range = upgrade.Element ("Range")?.Value,
								Unique = upgrade.Element ("Unique") != null ? (bool) upgrade.Element ("Unique") : false,
								Limited = upgrade.Element ("Limited") != null ? (bool) upgrade.Element ("Limited") : false,
								SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool) upgrade.Element ("SmallOnly") : false,
								LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool) upgrade.Element ("LargeOnly") : false,
								HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool) upgrade.Element ("HugeOnly") : false,
								Preview = upgrade.Element ("Preview") != null ? (bool) upgrade.Element ("Preview") : false,
								AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List<XElement> ()
															select upgr.Value).ToList ()),
								AdditionalActions = new ObservableCollection<string> ((from action in upgrade.Element ("AdditionalActions") != null ? upgrade.Element ("AdditionalActions").Elements () : new List<XElement> ()
														       select action.Value).ToList ()),
								Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List<XElement> ()
													   select upgr.Value).ToList ()),
								RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List<XElement> ()
														     select upgr.Value).ToList ()),
								RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List<XElement> ()
														   select upgr.Value).ToList ()),
								UpgradeOptions = new ObservableCollection<string> ((from upgr in upgrade.Element ("UpgradeOptions") != null ? upgrade.Element ("UpgradeOptions").Elements () : new List<XElement> ()
														    select upgr.Value).ToList ()),
								RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
								owned = upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == upgrade.Attribute ("id").Value) != null ?
								 (int) upgradesElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == upgrade.Attribute ("id").Value) : 0,
								MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int) upgrade.Element ("MinPilotSkill") : 0,
								MaxPilotSkill = upgrade.Element ("MaxPilotSkill") != null ? (int?) upgrade.Element ("MaxPilotSkill") : 0,
								MinAgility = upgrade.Element ("MinAgility") != null ? (int?) upgrade.Element ("MinAgility") : null,
								MaxAgility = upgrade.Element ("MaxAgility") != null ? (int?) upgrade.Element ("MaxAgility") : null,
								SquadLimit = upgrade.Element ("SquadLimit") != null ? (int?) upgrade.Element ("SquadLimit") : null,
								ShieldRequirement = upgrade.Element ("ShieldRequirement") != null ? (int?) upgrade.Element ("ShieldRequirement") : null,
								IsCustom = upgrade.Element ("Custom") != null ? (bool) upgrade.Element ("Custom") : false,
								CCL = upgrade.Element ("CCL") != null ? (bool) upgrade.Element ("CCL") : false,
								ModifiedManeuverDial = upgrade.Element ("ModifiedManeuverDial")?.Value,
								HotAC = bool.Parse (upgrade.Element ("HotAC")?.Value ?? "false"),
								Keywords = upgrade.Element ("Keywords")?.Value ?? "",
							});

				if (category.Attribute ("id").Value == "ept") {
					var hotacPilotEPTs = (from p in Pilot.Pilots
							      where p.Unique &&
							      !categoryUpgrades.Any (u => u.Id == (p.CanonicalName + "pilot" + p.Expansions)) &&
							      !p.CCL && !p.IsCustom
							      select new Upgrade {
								      Cost = p.PilotSkill,
								      Text = p.Ability,
								      Factions = p.Faction != null ? new List<Faction> { p.Faction } : null,
								      HotAC = true,
								      Category = "Elite Pilot Talent",
								      CategoryId = "ept",
								      Id = p.CanonicalName + "pilot" + p.Expansions,
								      Name = p.Name + " (Pilot)",
								      Unique = true,
								      AdditionalUpgrades = new ObservableCollection<string> (),
								      AdditionalActions = new ObservableCollection<string> (),
								      UpgradeOptions = new ObservableCollection<string> (),
								      RequiredSlots = new ObservableCollection<string> (),
								      RemovedUpgrades = new ObservableCollection<string> (),
								      Slots = new ObservableCollection<string> ()
							      }).OrderBy (u => u.Cost).GroupBy (u => u.Text).Select (g => g.First ()).ToList ();

					var upgrades = categoryUpgrades.ToList ();
					upgrades.AddRange (hotacPilotEPTs);
					categoryUpgrades = upgrades.AsEnumerable ();
				}

				allUpgrades.AddRange (categoryUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));

			}

			upgrades = new ObservableCollection<Upgrade> (allUpgrades);

			XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));
			List<Upgrade> allCustomUpgrades = new List<Upgrade> ();

			foreach (var customCategory in customUpgradesXml.Elements ()) {

				var categoryCustomUpgrades = (from upgrade in customCategory.Elements ()
							      select new Upgrade {
								      Id = upgrade.Attribute ("id")?.Value,
								      Name = upgrade.Element ("Name")?.Value,
								      CanonicalName = upgrade.Element ("CanonicalName")?.Value,
								      OldId = upgrade.Element ("OldId")?.Value,
								      Category = upgrade.Parent.Attribute ("type")?.Value,
								      CategoryId = upgrade.Parent.Attribute ("id")?.Value ?? CategoryToID [upgrade.Parent.Attribute ("type")?.Value],
								      Cost = (int) upgrade.Element ("Cost"),
								      Text = upgrade.Element ("Text")?.Value,
								      Factions = Faction.AllFactions.Where (f => (upgrade.Element ("Faction")?.Value?.Split (',')?.Contains (f.Id) ?? false)).ToList (),
								      Ship = Ship.AllShips.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value)?.Copy (),
								      ShipRequirement = upgrade.Element ("ShipRequirement")?.Value,
								      PilotSkill = upgrade.Element ("PilotSkill") != null ? (int) upgrade.Element ("PilotSkill") : 0,
								      Energy = upgrade.Element ("Energy") != null ? (int) upgrade.Element ("Energy") : 0,
								      Attack = upgrade.Element ("Attack") != null ? (int) upgrade.Element ("Attack") : 0,
								      Agility = upgrade.Element ("Agility") != null ? (int) upgrade.Element ("Agility") : 0,
								      Hull = upgrade.Element ("Hull") != null ? (int) upgrade.Element ("Hull") : 0,
								      Shields = upgrade.Element ("Shields") != null ? (int) upgrade.Element ("Shields") : 0,
								      SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool) upgrade.Element ("SecondaryWeapon") : false,
								      Dice = upgrade.Element ("Dice") != null ? (int) upgrade.Element ("Dice") : 0,
								      Range = upgrade.Element ("Range")?.Value,
								      Unique = upgrade.Element ("Unique") != null ? (bool) upgrade.Element ("Unique") : false,
								      Limited = upgrade.Element ("Limited") != null ? (bool) upgrade.Element ("Limited") : false,
								      SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool) upgrade.Element ("SmallOnly") : false,
								      LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool) upgrade.Element ("LargeOnly") : false,
								      HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool) upgrade.Element ("HugeOnly") : false,
								      Preview = upgrade.Element ("Preview") != null ? (bool) upgrade.Element ("Preview") : false,
								      AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List<XElement> ()
															      select upgr.Value).ToList ()),
								      AdditionalActions = new ObservableCollection<string> ((from action in upgrade.Element ("AdditionalActions") != null ? upgrade.Element ("AdditionalActions").Elements () : new List<XElement> ()
															     select action.Value).ToList ()),

								      Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List<XElement> ()
														 select upgr.Value).ToList ()),
								      RemovedUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("RemovedUpgrades") != null ? upgrade.Element ("RemovedUpgrades").Elements () : new List<XElement> ()
															   select upgr.Value).ToList ()),
								      RequiredSlots = new ObservableCollection<string> ((from upgr in upgrade.Element ("RequiredSlots") != null ? upgrade.Element ("RequiredSlots").Elements () : new List<XElement> ()
															 select upgr.Value).ToList ()),
								      RequiredAction = upgrade.Element ("RequiredAction") != null ? upgrade.Element ("RequiredAction").Value : null,
								      UpgradeOptions = new ObservableCollection<string> (),
								      owned = 0,
								      MinPilotSkill = upgrade.Element ("MinPilotSkill") != null ? (int) upgrade.Element ("MinPilotSkill") : 0,
								      MaxPilotSkill = upgrade.Element ("MaxPilotSkill") != null ? (int?) upgrade.Element ("MaxPilotSkill") : 0,
								      MinAgility = upgrade.Element ("MinAgility") != null ? (int?) upgrade.Element ("MinAgility") : null,
								      MaxAgility = upgrade.Element ("MaxAgility") != null ? (int?) upgrade.Element ("MaxAgility") : null,
								      ShieldRequirement = upgrade.Element ("ShieldRequirement") != null ? (int?) upgrade.Element ("ShieldRequirement") : null,
								      IsCustom = upgrade.Element ("Custom") != null ? (bool) upgrade.Element ("Custom") : false,
								      CCL = upgrade.Element ("CCL") != null ? (bool) upgrade.Element ("CCL") : false,
								      Keywords = upgrade.Element ("Keywords")?.Value ?? "",
							      });

				allCustomUpgrades.AddRange (categoryCustomUpgrades);
			}

			customUpgrades = new ObservableCollection<Upgrade> (allCustomUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));

			updateAllUpgrades ();
		}
		#endregion
	}
}

