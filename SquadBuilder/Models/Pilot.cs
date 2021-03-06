﻿﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace SquadBuilder
{
	public class Pilot : ObservableObject {
		public const string PilotsFilename = "Pilots.xml";

		string id;
		public string Id {
			get { return id; }
			set { SetProperty (ref id, value); }
		}

		string name;
		public string Name {
			get { return name; }
			set { SetProperty (ref name, value); }
		}

		string canonicalName;
		public string CanonicalName {
			get { return canonicalName ?? id; }
			set {
				SetProperty (ref canonicalName, value);
			}
		}

		string oldId;
		public string OldId {
			get { return oldId; }
			set { SetProperty (ref oldId, value); }
		}

		public string Keywords { get; set; } = "";

		public Faction Faction { get; set; }
		public Ship Ship { get; set; }

		bool unique;
		public bool Unique {
			get { return unique; }
			set {
				SetProperty (ref unique, value);

				if (unique) {
					if (!name.Contains ("•"))
						name = $"•{name}";
				} else
					name = name.Replace ("•", "");
			}
		}

		public int BasePilotSkill { get; set; }
		public int BaseEnergy { get; set; }
		public int BaseAttack { get; set; }
		public int BaseAgility { get; set; }
		public int BaseHull { get; set; }
		public int BaseShields { get; set; }
		public int BaseCost { get; set; }
		public string Ability { get; set; }

		public bool IsCustom { get; set; }
		public bool CCL { get; set; }
		public bool Preview { get; set; }

		public bool ShowAbility {
			get { return !string.IsNullOrEmpty (Ability); }
		}

		//public bool ShowUpgradesEquipped {
		//	get { return !string.IsNullOrEmpty (UpgradesEquippedString); }
		//}

		public bool ShowExtras {
			get {
				return IsCustom || CCL || Preview;
			}
		}

		public Guid LinkedPilotCardGuid { get; set; }
		public int MultiSectionId { get; set; } = -1;

		public string FactionSymbol {
			get {
				string symbol;
				switch (Faction.Id) {
				case "rebel":
					symbol = "!";
					break;
				case "imperial":
					symbol = "@";
					break;
				case "scum":
					symbol = "#";
					break;
				default:
					symbol = "";
					break;
				}

				return symbol;
			}
		}

		public string EnergySymbol {
			get { return "("; }
		}

		public string AttackSymbol {
			get { return Ship?.AttackSymbol ?? "%"; }
		}

		public string AgilitySymbol {
			get { return "^"; }
		}

		public string HullSymbol {
			get { return "&"; }
		}

		public string ShieldsSymbol {
			get { return "*"; }
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
				var pilotsElement = collectionXml.Element ("Pilots");

				var pilotsOwned = Pilot.Pilots.FirstOrDefault (p => p.Id == Id).Owned;

				if (pilotsElement.Elements ().Any (e => e.Attribute ("id").Value == Id)) {
					if (pilotsOwned == 0)
						pilotsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id)?.Remove ();
					else
						pilotsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id).SetValue (pilotsOwned);
				} else {
					var element = new XElement ("Pilot", pilotsOwned);
					element.SetAttributeValue ("id", Id);
					pilotsElement.Add (element);
				}

				DependencyService.Get<ISaveAndLoad> ().SaveText ("Collection.xml", collectionXml.ToString ());
			}
		}

		ObservableCollection<string> upgradeTypes = new ObservableCollection<string> ();
		public ObservableCollection<string> UpgradeTypes {
			get {
				return upgradeTypes;
			}
			set {
				SetProperty (ref upgradeTypes, value);

				try {
					if (value != null && value.Count () > 0) {
						var upgradesString = Upgrade.UpgradeSymbolDictionary [UpgradeTypes [0]];
						for (int i = 1; i < UpgradeTypes.Count; i++)
							upgradesString += " " + Upgrade.UpgradeSymbolDictionary [UpgradeTypes [i]];

						UpgradeTypesString = upgradesString;
					}
				} catch (Exception e) {
					Console.WriteLine ("Foo");
				}
			}
		}

		ObservableCollection<Upgrade> upgradesEquipped = new ObservableCollection<Upgrade> ();
		public ObservableCollection<Upgrade> UpgradesEquipped {
			get {
				return upgradesEquipped;
			}
			set {
				SetProperty (ref upgradesEquipped, value);

				UpgradesEquipped.CollectionChanged += (sender, e) => {
					NotifyPropertyChanged ("Upgrades");
					NotifyPropertyChanged ("UpgradesString");
					NotifyPropertyChanged ("PilotSkill");
					NotifyPropertyChanged ("Attack");
					NotifyPropertyChanged ("Agility");
					NotifyPropertyChanged ("Hull");
					NotifyPropertyChanged ("Shields");
					NotifyPropertyChanged ("Cost");
					NotifyPropertyChanged ("UpgradesEquippedString");
					NotifyPropertyChanged ("Energy");
				};
			}
		}

		public bool AbilityVisible {
			get {
				return !string.IsNullOrEmpty (Ability);
			}
		}

		[XmlIgnore]
		string expansions;
		public string Expansions {
			get {
				if (string.IsNullOrEmpty (expansions))
					expansions = string.Join (", ", Expansion.Expansions.Where (e => e.Pilots.Any (p => p == Id)).Select (e => e.Name));

				return expansions;
			}
		}

		[XmlIgnore]
		public string UpgradesEquippedString {
			get {
				return UpgradesEquipped != null && UpgradesEquipped.Count () > 0 ? string.Join (", ", UpgradesEquipped.Where (u => u != null).Select (u => u.Name)) : "b";
			}
		}

		string upgradeTypesString;
		[XmlIgnore]
		public string UpgradeTypesString {
			get {
				//var upgradesString = Upgrade.UpgradeSymbolDictionary [UpgradeTypes[0]];
				//for (int i = 1; i < UpgradeTypes.Count; i++)
				//	upgradesString += " " + Upgrade.UpgradeSymbolDictionary[UpgradeTypes[i]];

				//return upgradesString;
				//return string.Join (", ", UpgradeTypes);
				return upgradeTypesString;
			} set {
				SetProperty (ref upgradeTypesString, value);
			}
		}

		[XmlIgnore]
		public ObservableCollection<object> Upgrades {
			get {
				var u = new ObservableCollection<object> ();

				for (int i = 0; i < UpgradeTypes.Count (); i++) {
					var upgradeType = UpgradeTypes [i];
					Upgrade upgrade;

					if (upgradesEquipped.Count () > i)
						upgrade = UpgradesEquipped [i];
					else
						upgrade = null;

					if (upgrade != null)
						u.Add(upgrade);
					else
						u.Add(Upgrade.CreateUpgradeSlot (upgradeType));
				}

				return u;
			}
		}

		[XmlIgnore]
		public string UpgradesString {
			get {
				return string.Join (", ", Upgrades);
			}
		}

		[XmlIgnore]
		public int PilotSkill {
			get {
				return BasePilotSkill + UpgradesEquipped.Sum (u => u?.PilotSkill ?? 0);
			}

		}

		[XmlIgnore]
		public int Energy {
			get {
				return BaseEnergy + UpgradesEquipped.Sum (u => u?.Energy ?? 0);
			}
		}

		[XmlIgnore]
		public int Attack {
			get {
				return BaseAttack + UpgradesEquipped.Sum (u => u?.Attack ?? 0);
			}

		}
		[XmlIgnore]

		public int Agility {
			get {
				return BaseAgility + UpgradesEquipped.Sum (u => u?.Agility ?? 0);
			}
		}

		[XmlIgnore]
		public int Hull {
			get {
				return BaseHull + UpgradesEquipped.Sum (u => u?.Hull ?? 0);
			}
		}

		[XmlIgnore]
		public int Shields {
			get {
				return BaseShields + UpgradesEquipped.Sum (u => u?.Shields ?? 0);
			}
		}

		[XmlIgnore]
		public int Cost {
			get {
				float cost = BaseCost;

				foreach (var upgrade in UpgradesEquipped) {
					if (upgrade != null)
						cost += upgrade.Cost;
				}

				if (UpgradesEquipped != null && UpgradesEquipped.Count (u => u?.Name == "TIE/x1") != 0) {
					var index = UpgradeTypes.IndexOf ("System Upgrade");
					if (index > -1 && UpgradesEquipped.Count > index && UpgradesEquipped [index] != null)
						cost -= (int)Math.Min (4, upgradesEquipped [index].Cost);
				}
				return (int)cost;
			}
		}

		[XmlIgnore]
		public Color AbilityColor {
			get {
				return Device.OnPlatform<Color> (Color.Navy, Color.Teal, Color.Navy);
			}
		}

		public Color TextColor {
			get {
				return IsAvailable ? Color.Black : Color.Gray;
			}
		}

		public bool IsAvailable {
			get {
				if (Unique && Squadron.CurrentSquadron != null && Squadron.CurrentSquadron.Pilots.Any (p => p.Id == Id))
					return false;

				if (Pilot.Pilots.Sum (p => p.Owned) == 0)
					return true;

				return Owned > (Squadron.CurrentSquadron != null ? Squadron.CurrentSquadron.Pilots.Count (p => p.Id == Id) : 0);
			}
		}

		public bool ShowManeuversInPilotView {
			get { return Settings.ShowManeuversInPilotView && !string.IsNullOrEmpty (Ship.ManeuverGridImage); }
		}

		public bool ShowManeuversInSquadronList {
			get { return Settings.ShowManeuversInSquadronList && !string.IsNullOrEmpty (Ship.ManeuverGridImage); }
		}

		public bool ShowEnergy {
			get { return Energy > 0; }
		}

		public bool ShowAttack {
			get { return Attack > 0; }
		}

		public void EquipUpgrade (int index, Upgrade upgrade)
		{
			var oldUpgrade = UpgradesEquipped [index];

			UpgradesEquipped [index] = upgrade;

			if (oldUpgrade != null) {
				foreach (var additionalUpgrade in oldUpgrade.AdditionalUpgrades) {
					var oldIndex = UpgradeTypes.ToList ().LastIndexOf (additionalUpgrade);
					UpgradeTypes.RemoveAt (oldIndex);
					UpgradesEquipped.RemoveAt (oldIndex);
				}

				foreach (var removedUpgrade in oldUpgrade.RemovedUpgrades) {
					UpgradeTypes.Add (removedUpgrade);
					UpgradesEquipped.Add (null);
				}

				foreach (var additionalAction in oldUpgrade.AdditionalActions) {
					if (Ship.Actions.Contains (additionalAction))
						Ship.Actions.Remove (additionalAction);
				}

				for (int i = 0; i < oldUpgrade.Slots.Count (); i++) {
					UpgradeTypes.Add (oldUpgrade.Slots [i]);
					UpgradesEquipped.Add (null);
				}

				if (oldUpgrade.UpgradeOptions != null && oldUpgrade.UpgradeOptions.Any ()) {
					foreach (var upgradeType in oldUpgrade.UpgradeOptions) {
						var typeIndex = UpgradeTypes.IndexOf (upgradeType);
						if (typeIndex >= 0) {
							UpgradeTypes.RemoveAt (typeIndex);
							upgradesEquipped.RemoveAt (typeIndex);
						}
					}
				}

				if (oldUpgrade.Id == "ordnancetubes") {
					for (int i = 0; i < UpgradeTypes.Count; i++) {
						var type = UpgradeTypes [i];
						if (type == "Missile" || type == "Torpedo") {
							UpgradeTypes [i] = "Hardpoint";
							UpgradesEquipped [i] = null;
						}
					}

					if (MultiSectionId >= 0) {
						var otherPilot = Squadron.CurrentSquadron.Pilots.First (p => p.name != Name && p.MultiSectionId == MultiSectionId);
						for (int i = 0; i < otherPilot.upgradeTypes.Count; i++) {
							var type = otherPilot.upgradeTypes [i];
							if (type == "Missile" || type == "Torpedo") {
								otherPilot.UpgradeTypes [i] = "Hardpoint";
								otherPilot.UpgradesEquipped [i] = null;
							}
						}
					}

					//if (LinkedPilotCardGuid != Guid.Empty) {
					//	var otherPilot = Squadron.CurrentSquadron.Pilots.First (p => p.Name != Name && p.LinkedPilotCardGuid == LinkedPilotCardGuid);
					//	for (int i = 0; i < otherPilot.UpgradeTypes.Count; i++) {
					//		var type = otherPilot.UpgradeTypes [i];
					//		if (type == "Missile" || type == "Torpedo") {
					//			otherPilot.UpgradeTypes [i] = "Hardpoint";
					//			otherPilot.UpgradesEquipped [i] = null;
					//		}
					//	}
					//}
				}

				if (!string.IsNullOrEmpty (oldUpgrade.ModifiedManeuverDial))
					Ship.ManeuverGridImage = Ship.ManeuverGridImage.Replace (oldUpgrade.ModifiedManeuverDial, "");
			}
			if (upgrade != null) {
				upgrade.Pilot = this;

				foreach (var newUpgrade in upgrade.AdditionalUpgrades) {
					UpgradeTypes.Add (newUpgrade);
					UpgradesEquipped.Add (null);
				}

				foreach (var removedUpgrade in upgrade.RemovedUpgrades) {
					var removedIndex = UpgradeTypes.ToList ().LastIndexOf (removedUpgrade);
					if (removedIndex < 0)
						continue;
					UpgradeTypes.RemoveAt (removedIndex);
					UpgradesEquipped.RemoveAt (removedIndex);
				}

				foreach (var newAction in upgrade.AdditionalActions) {
					if (!Ship.Actions.Contains (newAction))
						Ship.Actions.Add (newAction);
				}

				for (int i = 0; i < upgrade.Slots.Count (); i++) {
					var extraIndex = Upgrades.ToList ().LastIndexOf (Upgrade.CreateUpgradeSlot(upgrade.Slots [i]));
					if (index < 0)
						continue;
					UpgradeTypes.RemoveAt (extraIndex);
					UpgradesEquipped.RemoveAt (extraIndex);
				}

				if (upgrade.Id == "misthunter")
					UpgradesEquipped [UpgradesEquipped.Count - 1] = Upgrade.Upgrades.First (u => u.Name == "Tractor Beam");

				if (!string.IsNullOrEmpty (upgrade.ModifiedManeuverDial))
					Ship.ManeuverGridImage = Path.GetFileNameWithoutExtension (Ship.ManeuverGridImage) + upgrade.ModifiedManeuverDial + Path.GetExtension (Ship.ManeuverGridImage);
			}
		}

		[XmlIgnore]
		Command deletePilot;
		[XmlIgnore]
		public Command DeletePilot {
			get {
				if (deletePilot == null)
					deletePilot = new Command (() => {
						XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

						var pilotElement = customPilotsXml.Elements ().FirstOrDefault (e =>
							e.Element ("Name")?.Value == Name &&
							(Faction == null || e.Attribute ("faction")?.Value == Faction?.Id) &&
							(Ship == null || e.Attribute ("ship")?.Value == Ship?.Id));

						if (pilotElement == null)
							return;

						pilotElement.Remove ();

						DependencyService.Get<ISaveAndLoad> ().SaveText ("Pilots_Custom.xml", customPilotsXml.ToString ());

						MessagingCenter.Send<Pilot> (this, "DeletePilot");
					});

				return deletePilot;
			}
		}

		[XmlIgnore]
		Command editPilot;
		[XmlIgnore]
		public Command EditPilot {
			get {
				if (editPilot == null)
					editPilot = new Command (() => {
						MessagingCenter.Send<Pilot> (this, "Edit Pilot");
					});

				return editPilot;
			}
		}

		[XmlIgnore]
		Command removePilot;
		[XmlIgnore]
		public Command RemovePilot {
			get {
				if (removePilot == null)
					removePilot = new Command (() => {
						MessagingCenter.Send<Pilot> (this, "Remove Pilot");
					});

				return removePilot;
			}
		}

		[XmlIgnore]
		Command copyPilot;
		[XmlIgnore]
		public Command CopyPilot {
			get {
				if (copyPilot == null)
					copyPilot = new Command (() => MessagingCenter.Send<Pilot> (this, "Copy Pilot"));

				return copyPilot;
			}
		}

		public Pilot Copy ()
		{
			return new Pilot {
				Id = Id,
				Name = Name,
				CanonicalName = CanonicalName,
				OldId = OldId,
				Faction = Faction,
				Ship = Ship.Copy (),
				Unique = Unique,
				BasePilotSkill = BasePilotSkill,
				BaseEnergy = BaseEnergy,
				BaseAttack = BaseAttack,
				BaseAgility = BaseAgility,
				BaseHull = BaseHull,
				BaseShields = BaseShields,
				BaseCost = BaseCost,
				Ability = Ability,
				Preview = Preview,
				IsCustom = IsCustom,
				CCL = CCL,
				MultiSectionId = MultiSectionId,
				Keywords = Keywords,
				UpgradeTypes = new ObservableCollection<string> (UpgradeTypes.ToList ()),
				UpgradesEquipped = new ObservableCollection<Upgrade> (UpgradesEquipped.ToList ())
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
			if (obj == null || !(obj is Pilot))
				return false;

			var pilot = obj as Pilot;

			return (Id == pilot.Id &&
				Faction?.Id == pilot.Faction?.Id &&
				Ship?.Id == pilot.Ship?.Id &&
				UpgradesEquippedString == pilot?.UpgradesEquippedString);
		}

		public override int GetHashCode ()
		{
			return (Id + Faction?.Id + Ship?.Id + UpgradesEquippedString).GetHashCode ();
		}

		public override string ToString ()
		{
			return Ship.Name + " - " + Name;
		}

#region Static Methods
		static ObservableCollection<Pilot> pilots;
		public static ObservableCollection<Pilot> Pilots {
			get {
				if (pilots == null)
					GetAllPilots ();

				return pilots;
			}
			set {
				pilots = value;
				pilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		static ObservableCollection<Pilot> customPilots;
		public static ObservableCollection<Pilot> CustomPilots {
			get {
				if (customPilots == null)
					GetAllPilots ();

				return customPilots;
			}
			set {
				customPilots = value;
				customPilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		static ObservableCollection<Pilot> allPilots;
		public static ObservableCollection<Pilot> AllPilots {
			get {
				if (allPilots == null)
					updateAllPilots ();

				return allPilots;
			}
		}

		static void updateAllPilots ()
		{
			var temp = Pilots.ToList ();
			temp.AddRange (customPilots);
			allPilots = new ObservableCollection<Pilot> (temp);
		}

		public static void GetAllPilots ()
		{
			if (!DependencyService.Get<ISaveAndLoad> ().FileExists (Pilot.PilotsFilename))
				return;

			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var pilotsCollectionElement = collectionXml.Element ("Pilots");

			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Pilot.PilotsFilename)));
			pilots = new ObservableCollection<Pilot> (from pilot in pilotsXml.Elements ()
								  select new Pilot {
									  Id = pilot.Attribute ("id").Value,
									  Name = pilot.Element ("Name").Value,
									  CanonicalName = pilot.Element ("CanonicalName")?.Value,
									  OldId = pilot.Element ("OldId")?.Value,
									  Faction = Faction.Factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
									  Ship = Ship.Ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value)?.Copy (),
									  Unique = (bool) pilot.Element ("Unique"),
									  BasePilotSkill = (int) pilot.Element ("PilotSkill"),
									  BaseEnergy = pilot.Element ("Energy") != null ? (int) pilot.Element ("Energy") : 0,
									  BaseAttack = (int) pilot.Element ("Attack"),
									  BaseAgility = (int) pilot.Element ("Agility"),
									  BaseHull = (int) pilot.Element ("Hull"),
									  BaseShields = (int) pilot.Element ("Shields"),
									  BaseCost = (int) pilot.Element ("Cost"),
									  Ability = pilot.Element ("Ability")?.Value,
									  Keywords = pilot.Element ("Keywords")?.Value ?? "",
									  UpgradeTypes = new ObservableCollection<string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
									  UpgradesEquipped = new ObservableCollection<Upgrade> (new List<Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
									  IsCustom = pilot.Element ("Custom") != null ? (bool) pilot.Element ("Custom") : false,
									  CCL = pilot.Element ("CCL") != null ? (bool) pilot.Element ("CCL") : false,
									  Preview = pilot.Element ("Preview") != null ? (bool) pilot.Element ("Preview") : false,
									  owned = pilotsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == pilot.Attribute ("id").Value) != null ?
						       (int) pilotsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == pilot.Attribute ("id").Value) : 0
								  });

			XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));
			customPilots = new ObservableCollection<Pilot> (from pilot in customPilotsXml.Elements ()
									select new Pilot {
										Id = pilot.Attribute ("id").Value,
										Name = pilot.Element ("Name").Value,
										CanonicalName = pilot.Element ("CanonicalName")?.Value,
										OldId = pilot.Element ("OldId")?.Value,
										Faction = Faction.AllFactions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
										Ship = Ship.AllShips.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value)?.Copy (),
										Unique = (bool) pilot.Element ("Unique"),
										BasePilotSkill = (int) pilot.Element ("PilotSkill"),
										BaseEnergy = pilot.Element ("Energy") != null ? (int) pilot.Element ("Energy") : 0,
										BaseAttack = (int) pilot.Element ("Attack"),
										BaseAgility = (int) pilot.Element ("Agility"),
										BaseHull = (int) pilot.Element ("Hull"),
										BaseShields = (int) pilot.Element ("Shields"),
										BaseCost = (int) pilot.Element ("Cost"),
										Ability = pilot.Element ("Ability")?.Value,
										Keywords = pilot.Element ("Keywords")?.Value ?? "",
										UpgradeTypes = new ObservableCollection<string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
										UpgradesEquipped = new ObservableCollection<Upgrade> (new List<Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
										IsCustom = (bool) pilot.Element ("Custom"),
										CCL = false,
										Preview = pilot.Element ("Preview") != null ? (bool) pilot.Element ("Preview") : false,
										owned = 0
									});

			updateAllPilots ();
		}
		#endregion
	}
}