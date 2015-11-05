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
	public class EditUpgradeViewModel : ViewModel
	{
		public EditUpgradeViewModel ()
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

				XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

				originalXml = customUpgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == Upgrade.Id);
			}
		}

		XElement originalXml;
		public XElement OriginalXml {
			get { return originalXml; }
			set { SetProperty (ref originalXml, value); }
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
					Upgrade.Faction = Factions [factionIndex - 1];
				else
					Upgrade.Faction = null;
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set {
				SetProperty (ref shipIndex, value);
				if (shipIndex > 0)
					Upgrade.Ship = Ships [shipIndex - 1];
				else
					Upgrade.Ship = null;
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

						XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades.xml")));
						XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

						if (upgradesXml.Descendants ().FirstOrDefault (e => e?.Value == Name) != null)
							return;
						
						var element = customUpgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == OriginalXml.Attribute ("id")?.Value);
						element.SetAttributeValue ("id", Upgrade.Id);
						element.SetElementValue ("Name", Name);
						element.SetElementValue ("Cost", Cost);
						element.SetElementValue ("Text", Text);
						element.SetElementValue ("Unique", IsUnique);
						element.SetElementValue ("Limited", IsLimited);
						element.SetElementValue ("Ship", Upgrade.Ship?.Id);
						element.SetElementValue ("Faction", Upgrade.Faction?.Id);
						element.SetElementValue ("SmallOnly", SmallOnly);
						element.SetElementValue ("LargeOnly", LargeOnly);
						element.SetElementValue ("HugeOnly", HugeOnly);
						element.SetElementValue ("PilotSkill", PilotSkill);
						element.SetElementValue ("Attack", Attack);
						element.SetElementValue ("Agility", Agility);
						element.SetElementValue ("Hull", Hull);
						element.SetElementValue ("Shields", Shields);
						element.SetElementValue ("SecondaryWeapon", SecondaryWeapon);
						element.SetElementValue ("Dice", Dice);
						element.SetElementValue ("Range", Range);

							//							new XElement ("ExtraSlots", 
							//								new XElement ("Upgrade", )
							//							),
							//							new XElement ("AdditionalUpgrades", 
							//								new XElement ("Upgrade", )

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customUpgradesXml.ToString ());

						MessagingCenter.Send <EditUpgradeViewModel, Upgrade> (this, "Finished Editing", Upgrade);
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

