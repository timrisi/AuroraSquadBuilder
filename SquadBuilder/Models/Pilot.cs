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
		public bool Preview { get; set; }
		public Guid LinkedPilotCardGuid { get; set; }

		int owned;
		public int Owned { 
			get { return owned; }
			set {
				if (value < 0)
					value = 0;
				
				SetProperty (ref owned, value); 
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
				if (Id == "keyanfarlander")
					Console.WriteLine ("Keyan: Available - " + IsAvailable);
				return IsAvailable ? Color.Black : Color.Gray; 
			}
		}

		public bool IsAvailable {
			get {
				if (Cards.SharedInstance.Pilots.Sum (p => p.Owned) == 0)
					return true;
				
				if (Unique && Cards.SharedInstance.CurrentSquadron.Pilots.Any (p => p.Id == Id))
					return false;
				
				return Owned > Cards.SharedInstance.CurrentSquadron.Pilots.Count (p => p.Id == Id);
			}
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
	}
}

