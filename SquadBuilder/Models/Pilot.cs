using System;
using System.Collections.Generic;
using XLabs;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using XLabs.Data;

namespace SquadBuilder
{
	public class Pilot  : ObservableObject
	{
		public Pilot ()
		{
		}

		public string Name { get; set; }
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

		ObservableCollection <string> upgradeTypes;
		public ObservableCollection <string> UpgradeTypes { 
			get {
				return upgradeTypes;
			}
			set {
				SetProperty (ref upgradeTypes, value);
			}
		}

		ObservableCollection <Upgrade> upgradesEquipped;
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

					if (UpgradesEquipped == null || UpgradesEquipped.Count (u => u?.Name == "TIE/x1") == 0)
						return;

					var index = UpgradeTypes.IndexOf ("System Upgrade");
					if (index > -1 && UpgradesEquipped [index] != null)
						UpgradesEquipped [index].Cost = (int)Math.Max (0, UpgradesEquipped [index].Cost - 4);
				};
			}
		}

		public string UpgradesEquippedString {
			get {
				return string.Join (", ", UpgradesEquipped.Where (u => u != null).Select (u => u.Name));
			}
		}

		public string UpgradeTypesString { 
			get {
				return string.Join (", ", UpgradeTypes);
			}
		}

		public ObservableCollection <string> Upgrades {
			get {
				var u = new ObservableCollection <string> ();

				for (int i = 0; i < UpgradeTypes.Count (); i++) {
					var upgradeType = UpgradeTypes [i];
					var upgrade = UpgradesEquipped [i];

					if (upgrade != null)
						u.Add (upgrade.Name);
					else
						u.Add (upgradeType);
				}

				return u;
			}
		}

		public string UpgradesString {
			get {
				return string.Join (", ", Upgrades);
			}
		}

		public int PilotSkill { 
			get {
				return BasePilotSkill + UpgradesEquipped.Sum (u => u?.PilotSkill ?? 0);
			}
		
		}
		public int Attack { 
			get {
				return BaseAttack + UpgradesEquipped.Sum (u => u?.Attack ?? 0);
			}
		
		}
		public int Agility {
			get {
				return BaseAgility + UpgradesEquipped.Sum (u => u?.Agility ?? 0);
			}
		}

		public int Hull { 
			get {
				return BaseHull + UpgradesEquipped.Sum (u => u?.Hull ?? 0);
			}
		}

		public int Shields {
			get {
				return BaseShields + UpgradesEquipped.Sum (u => u?.Shields ?? 0);
			}
		}

		public int Cost {
			get {
				float cost = BaseCost;

				foreach (var upgrade in UpgradesEquipped) {
					if (upgrade != null)
						cost += upgrade.Cost / (upgrade.Slots.Count + 1);
				}

				return (int)cost;
			}
		}

		public Color AbilityColor {
			get {
				return Device.OnPlatform <Color> (Color.Navy, Color.Teal, Color.Navy);
			}
		}

		RelayCommand deletePilot;
		public RelayCommand DeletePilot {
			get {
				if (deletePilot == null)
					deletePilot = new RelayCommand (() => MessagingCenter.Send <Pilot> (this, "DeletePilot"));

				return deletePilot;
			}
		}

		RelayCommand copyPilot;
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
				UpgradeTypes = new ObservableCollection<string> (UpgradeTypes.ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (UpgradesEquipped.ToList ())
			};
		}
	}
}

