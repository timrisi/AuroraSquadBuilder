using System;
using XLabs.Forms.Mvvm;
using XLabs;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CreateUpgradeViewModel : ViewModel
	{
		public CreateUpgradeViewModel ()
		{
			Factions = new ObservableCollection<Faction> (Cards.SharedInstance.AllFactions);

			Ships = new ObservableCollection <Ship> (Cards.SharedInstance.AllShips);

			var upgrades = Cards.SharedInstance.AllUpgrades;
			var upgradeTypes = upgrades.Select (u => u.Category).Distinct ();

			UpgradeTypes =  new ObservableCollection<string> (upgradeTypes);
		}

		Upgrade upgrade;
		public Upgrade Upgrade {
			get { 
				if (upgrade == null)
					upgrade = new Upgrade ();

				return upgrade;
			}
			set {
				SetProperty (ref upgrade, value);
			}
		}

		public string Name {
			get { return Upgrade.Name; }
			set {
				Upgrade.Name = value;

				char[] arr = Name.ToCharArray();

				arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
				Upgrade.Id = new string(arr).ToLower ();
				NotifyPropertyChanged ("Name");
			}
		}

		public int Cost {
			get { return Upgrade.Cost; }
			set {
				Upgrade.Cost = value;
			}
		}

		public string Text {
			get { return Upgrade.Text; }
			set {
				Upgrade.Text = value;
			}
		}

		public bool IsLimited {
			get { return Upgrade.Limited; }
			set {
				Upgrade.Limited = value;
				if (value)
					IsUnique = false;
				NotifyPropertyChanged ("IsUnique");
			}
		}

		public bool IsUnique {
			get { return Upgrade.Unique; }
			set {
				Upgrade.Unique = value;
				if (value)
					IsLimited = false;
				NotifyPropertyChanged ("IsLimited");
			}
		}

		public bool SmallOnly {
			get { return Upgrade.SmallOnly; }
			set {
				Upgrade.SmallOnly = value;
				if (value) {
					HugeOnly = false;
					LargeOnly = false;
					NotifyPropertyChanged ("HugeOnly");
					NotifyPropertyChanged ("LargeOnly");
				}
			}
		}

		public bool LargeOnly {
			get { return Upgrade.LargeOnly; }
			set { 
				Upgrade.LargeOnly = value;
				if (value) {
					SmallOnly = false;
					HugeOnly = false;
					NotifyPropertyChanged ("SmallOnly");
					NotifyPropertyChanged ("HugeOnly");
				}
			}
		}

		public bool HugeOnly {
			get { return Upgrade.HugeOnly; }
			set {
				Upgrade.HugeOnly = value;
				if (value) {
					SmallOnly = false;
					LargeOnly = false;
					NotifyPropertyChanged ("SmallOnly");
					NotifyPropertyChanged ("LargeOnly");
				}
			}
		}

		public bool SecondaryWeapon {
			get { return Upgrade.SecondaryWeapon; }
			set {
				Upgrade.SecondaryWeapon = value;
				NotifyPropertyChanged ("SecondaryWeapon");
			}
		}

		public int Dice {
			get { return Upgrade.Dice; }
			set {
				Upgrade.Dice = value;
			}
		}

		public string Range {
			get { return Upgrade.Range; }
			set {
				Upgrade.Range = value;
			}
		}

		public int PilotSkill {
			get { return Upgrade.PilotSkill; }
			set {
				Upgrade.PilotSkill = value;
			}
		}

		int attack;
		public int Attack {
			get { return attack; }
			set {
				SetProperty (ref attack, value);
			}
		}

		public int Agility {
			get { return Upgrade.Agility; }
			set {
				Upgrade.Agility = value;
			}
		}

		public int Hull {
			get { return Upgrade.Hull; }
			set {
				Upgrade.Hull = value;
			}
		}

		public int Shields {
			get { return Upgrade.Shields; }
			set {
				Upgrade.Shields = value;
			}
		}

		int factionIndex;
		public int FactionIndex {
			get { return factionIndex; }
			set {
				SetProperty (ref factionIndex, value);
				if (factionIndex > 0)
					Upgrade.Factions = new List<Faction> { Factions [factionIndex - 1] };
				else
					Upgrade.Factions = null;
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set {
				SetProperty (ref shipIndex, value);
				if (shipIndex > 0) {
					Upgrade.Ship = Ships [shipIndex - 1].Copy ();
					Upgrade.ShipRequirement = Upgrade.Ship.Name;
				} else {
					Upgrade.Ship = null;
					Upgrade.ShipRequirement = null;
				}
				
			}
		}

		int upgradeTypeIndex;
		public int UpgradeTypeIndex {
			get { return upgradeTypeIndex; }
			set {
				SetProperty (ref upgradeTypeIndex, value);
				if (upgradeTypeIndex >= 0)
					Upgrade.Category = UpgradeTypes [upgradeTypeIndex];
			}
		}

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get { return factions; }
			set {
				SetProperty (ref factions, value);
			}
		}

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { return ships; }
			set {
				SetProperty (ref ships, value);
			}
		}

		ObservableCollection <string> upgradeTypes;
		public ObservableCollection <string> UpgradeTypes {
			get { return upgradeTypes; }
			set {
				SetProperty (ref upgradeTypes, value);
			}
		}

		RelayCommand saveUpgrade;
		public RelayCommand SaveUpgrade {
			get {
				if (saveUpgrade == null)
					saveUpgrade = new RelayCommand (() => {
						if (string.IsNullOrWhiteSpace (Name))
							return;

						XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

						if (Cards.SharedInstance.Upgrades.Count (u => u.Name == Name) > 0)
							return;

						if (Cards.SharedInstance.CustomUpgrades.Count (u => u.Name == Name) > 0)
							return;
						
						var element = new XElement ("Upgrade",
							new XAttribute ("id", Upgrade.Id),
							new XElement ("Name", Name),
							new XElement ("Cost", Cost),
							new XElement ("Text", Text),
							new XElement ("Unique", IsUnique),
							new XElement ("Limited", IsLimited),
                            new XElement ("ShipRequirement", Upgrade.ShipRequirement),
							new XElement ("Factions", Upgrade.Factions? [0]?.Id),
							new XElement ("SmallOnly", SmallOnly),
							new XElement ("LargeOnly", LargeOnly),
							new XElement ("HugeOnly", HugeOnly),
							new XElement ("PilotSkill", PilotSkill),
							new XElement ("Attack", Attack),
							new XElement ("Agility", Agility),
							new XElement ("Hull", Hull),
							new XElement ("Shields", Shields),
							new XElement ("SecondaryWeapon", SecondaryWeapon),
							new XElement ("Dice", Dice),
							new XElement ("Range", Range),
                            new XElement ("Custom", true)
						);

						var category = customUpgradesXml.Elements ().FirstOrDefault (e => e.Attribute ("type")?.Value == UpgradeTypes [UpgradeTypeIndex]);
						if (category == null) {
							category = new XElement ("Category", new XAttribute ("type", UpgradeTypes [UpgradeTypeIndex]));
							customUpgradesXml.Add (category);
						}

						category?.Add (element);
	
						DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customUpgradesXml.ToString ());
//
						MessagingCenter.Send <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", Upgrade);
					});
				return saveUpgrade;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Factions = new ObservableCollection<Faction> (Cards.SharedInstance.AllFactions);

			Ships = new ObservableCollection <Ship> (Cards.SharedInstance.AllShips);

			var upgrades = Cards.SharedInstance.AllUpgrades;
			var upgradeTypes = upgrades.Select (u => u.Category).Distinct ();

			UpgradeTypes =  new ObservableCollection<string> (upgradeTypes);
		}
	}
}

