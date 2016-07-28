using System;
using System.Collections.Generic;
using XLabs;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using XLabs.Data;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace SquadBuilder
{
	public class Pilot  : ObservableObject
	{
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
			get { return canonicalName; }
			set {
				SetProperty (ref canonicalName, value);
			}
		}

		public Faction Faction { get; set; }
		public Ship Ship { get; set; }
		public bool Unique { get; set; }
		public int BasePilotSkill { get; set; }
		public int BaseAttack { get; set; }
		public int BaseAgility { get; set; }
		public int BaseHull { get; set; }
		public int BaseShields { get; set; }
		public int BaseCost { get; set; }
		public string Ability { get; set; }
		public bool IsCustom { get; set; }
		public bool CCL { get; set; }
		public bool Preview { get; set; }
		public Guid LinkedPilotCardGuid { get; set; }

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

				var pilotsOwned = Cards.SharedInstance.Pilots.FirstOrDefault (p => p.Id == Id).Owned;

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

		ObservableCollection <string> upgradeTypes = new ObservableCollection <string> ();
		public ObservableCollection <string> UpgradeTypes { 
			get {
				return upgradeTypes;
			}
			set {
				SetProperty (ref upgradeTypes, value);
			}
		}

		ObservableCollection <Upgrade> upgradesEquipped = new ObservableCollection <Upgrade> ();
		public ObservableCollection <Upgrade> UpgradesEquipped { 
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
					expansions = string.Join (", ", Cards.SharedInstance.Expansions.Where (e => e.Pilots.Any (p => p == Id)).Select (e => e.Name));

				return expansions;
			}
		}

		[XmlIgnore]
		public string UpgradesEquippedString {
			get {
				return string.Join (", ", UpgradesEquipped.Where (u => u != null).Select (u => u.Name));
			}
		}

		[XmlIgnore]
		public string UpgradeTypesString { 
			get {
				return string.Join (", ", UpgradeTypes);
			}
		}

		[XmlIgnore]
		public ObservableCollection <object> Upgrades {
			get {
				var u = new ObservableCollection <object> ();

				for (int i = 0; i < UpgradeTypes.Count (); i++) {
					var upgradeType = UpgradeTypes [i];
					Upgrade upgrade;

					if (upgradesEquipped.Count () > i)
						upgrade = UpgradesEquipped [i];
					else
						upgrade = null;

					if (upgrade != null)
						u.Add (upgrade);
					else
						u.Add (new { Name = upgradeType, IsUpgrade = false });
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
						cost += upgrade.Cost / (upgrade.Slots.Count + 1);
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
				return Device.OnPlatform <Color> (Color.Navy, Color.Teal, Color.Navy);
			}
		}

		public Color TextColor {
			get {
				return IsAvailable ? Color.Black : Color.Gray; 
			}
		}

		public bool IsAvailable {
			get {
				if (Unique && Cards.SharedInstance.CurrentSquadron.Pilots.Any (p => p.Id == Id))
					return false;
				
				if (Cards.SharedInstance.Pilots.Sum (p => p.Owned) == 0)
					return true;
				
				return Owned > Cards.SharedInstance.CurrentSquadron.Pilots.Count (p => p.Id == Id);
			}
		}

		public bool ShowManeuversInPilotView {
			get { return Settings.ShowManeuversInPilotView && !string.IsNullOrEmpty (Ship.ManeuverGridImage); }
		}

		public bool ShowManeuversInSquadronList {
			get { return Settings.ShowManeuversInSquadronList && !string.IsNullOrEmpty (Ship.ManeuverGridImage); }
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

				for (int i = 0; i < oldUpgrade.Slots.Count (); i++) {
					var extraIndex = UpgradesEquipped.IndexOf (oldUpgrade);
					if (index >= 0)
						UpgradesEquipped [extraIndex] = null;
				}

				if (oldUpgrade.Id == "ordnancetubes") {
					for (int i = 0; i < UpgradeTypes.Count; i++) {
						var type = UpgradeTypes [i];
						if (type == "Missile" || type == "Torpedo") {
							UpgradeTypes [i] = "Hardpoint";
							UpgradesEquipped [i] = null;
						}
					}

					if (LinkedPilotCardGuid != Guid.Empty) {
						var otherPilot = Cards.SharedInstance.CurrentSquadron.Pilots.First (p => p.Name != Name && p.LinkedPilotCardGuid == LinkedPilotCardGuid);
						for (int i = 0; i < otherPilot.UpgradeTypes.Count; i++) {
							var type = otherPilot.UpgradeTypes [i];
							if (type == "Missile" || type == "Torpedo") {
								otherPilot.UpgradeTypes [i] = "Hardpoint";
								otherPilot.UpgradesEquipped [i] = null;
							}
						}
					}
				}
			}
			if (upgrade != null) {
				foreach (var newUpgrade in upgrade.AdditionalUpgrades) {
					UpgradeTypes.Add (newUpgrade);
					UpgradesEquipped.Add (null);
				}

				foreach (var removedUpgrade in upgrade.RemovedUpgrades) {
					var removedIndex = UpgradeTypes.IndexOf (removedUpgrade);
					if (removedIndex < 0)
						continue;
					UpgradeTypes.RemoveAt (removedIndex);
					UpgradesEquipped.RemoveAt (removedIndex);
				}

				for (int i = 0; i < upgrade.Slots.Count (); i++) {
					var extraIndex = Upgrades.IndexOf (new { Name = upgrade.Slots [i], IsUpgrade = false });
					if (index >= 0)
						UpgradesEquipped [extraIndex] = upgrade;
				}

				if (upgrade.Id == "misthunter")
					UpgradesEquipped [UpgradesEquipped.Count - 1] = Cards.SharedInstance.Upgrades.First (u => u.Name == "Tractor Beam");
			}

//			NotifyPropertyChanged ("Pilot");
		}

		[XmlIgnore]
		RelayCommand deletePilot;
		[XmlIgnore]
		public RelayCommand DeletePilot {
			get {
				if (deletePilot == null)
					deletePilot = new RelayCommand (() => {
						XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));

						var pilotElement = customPilotsXml.Elements ().FirstOrDefault (e => 
							e.Element ("Name")?.Value == Name &&
							(Faction == null || e.Attribute ("faction")?.Value == Faction?.Id) &&
							(Ship == null || e.Attribute ("ship")?.Value == Ship?.Id));

						if (pilotElement == null)
							return;

						pilotElement.Remove ();

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Pilots_Custom.xml", customPilotsXml.ToString ());

						MessagingCenter.Send <Pilot> (this, "DeletePilot");
					});

				return deletePilot;
			}
		}

		[XmlIgnore]
		RelayCommand editPilot;
		[XmlIgnore]
		public RelayCommand EditPilot {
			get {
				if (editPilot == null)
					editPilot = new RelayCommand (() => {
						MessagingCenter.Send <Pilot> (this, "Edit Pilot");
					});

				return editPilot;
			}
		}

		[XmlIgnore]
		RelayCommand removePilot;
		[XmlIgnore]
		public RelayCommand RemovePilot {
			get {
				if (removePilot == null)
					removePilot = new RelayCommand (() => {
						MessagingCenter.Send <Pilot> (this, "Remove Pilot");
					});

				return removePilot;
			}
		}

		[XmlIgnore]
		RelayCommand copyPilot;
		[XmlIgnore]
		public RelayCommand CopyPilot {
			get {
				if (copyPilot == null)
					copyPilot = new RelayCommand (() => MessagingCenter.Send <Pilot> (this, "Copy Pilot"));

				return copyPilot;
			}
		}

		public Pilot Copy ()
		{
			return new Pilot {
				Id = Id,
				Name = Name,
				CanonicalName = CanonicalName,
				Faction = Faction,
				Ship = Ship,
				Unique = Unique,
				BasePilotSkill = BasePilotSkill,
				BaseAttack = BaseAttack,
				BaseAgility = BaseAgility,
				BaseHull = BaseHull,
				BaseShields = BaseShields,
				BaseCost = BaseCost,
				Ability = Ability,
				Preview = Preview,
				IsCustom = IsCustom,
				CCL = CCL,
				UpgradeTypes = new ObservableCollection<string> (UpgradeTypes.ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (UpgradesEquipped.ToList ())
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
			if (obj == null || !(obj is Pilot))
				return false;

			var pilot = obj as Pilot;

			return (Id == pilot.Id &&
				Faction?.Id == pilot.Faction?.Id &&
				Ship?.Id == pilot.Ship?.Id &&
				UpgradesEquippedString == pilot.UpgradesEquippedString);
		}

		public override int GetHashCode ()
		{
			return (Id + Faction?.Id + Ship?.Id + UpgradesEquippedString).GetHashCode ();
		}
	}
}