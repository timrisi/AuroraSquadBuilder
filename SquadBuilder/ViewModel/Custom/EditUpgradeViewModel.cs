using System;


using System.Collections.ObjectModel;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace SquadBuilder {
	public class EditUpgradeViewModel : ViewModel {
		public bool Create = false;

		public EditUpgradeViewModel ()
		{
			Factions = new ObservableCollection<Faction> (Faction.AllFactions);
			Factions.Insert (0, new Faction { Name = "Any" });

			Ships = new ObservableCollection<Ship> (Ship.AllShips);
			Ships.Insert (0, new Ship { Name = "Any" });

			var upgrades = Upgrade.AllUpgrades;
			var upgradeTypes = upgrades.Select (u => u.Category).Distinct ();

			UpgradeTypes = new ObservableCollection<string> (upgradeTypes);
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

				XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

				originalXml = customUpgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == Upgrade.Id);

				if (Upgrade.Factions != null && Upgrade.Factions.Count () > 0)
					FactionIndex = Factions.IndexOf (upgrade.Factions [0]);
				NotifyPropertyChanged ("FactionIndex");

				if (Upgrade.ShipRequirement != null && Upgrade.ShipRequirement.Count () > 0)
					ShipIndex = Ships.IndexOf (Ships.FirstOrDefault (s => s.Name == Upgrade.ShipRequirement));
				NotifyPropertyChanged ("ShipIndex");

				if (!string.IsNullOrEmpty (Upgrade.Category))
					UpgradeTypeIndex = UpgradeTypes.IndexOf (Upgrade.Category);
				NotifyPropertyChanged ("UpgradeTypeIndex");
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

				char [] arr = Name.ToCharArray ();

				arr = Array.FindAll<char> (arr, (c => (char.IsLetterOrDigit (c))));
				Upgrade.Id = new string (arr).ToLower ();
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
				if (value < 0) {
					var oldVal = factionIndex;
					SetProperty (ref factionIndex, value);
					SetProperty (ref factionIndex, oldVal);
				} else if (value != factionIndex)
					SetProperty (ref factionIndex, value);

				if (value > 0)
					Upgrade.Factions = new List<Faction> { Factions [value] };
				else
					Upgrade.Factions = null;
			}
		}

		int shipIndex;
		public int ShipIndex {
			get { return shipIndex; }
			set {
				if (value < 0) {
					var oldVal = shipIndex;
					SetProperty (ref shipIndex, value);
					SetProperty (ref shipIndex, oldVal);
				} else if (value != shipIndex)
					SetProperty (ref shipIndex, value);

				if (shipIndex > 0)
					Upgrade.ShipRequirement = Ships [shipIndex].Name;
				else
					Upgrade.ShipRequirement = null;
			}
		}

		int upgradeTypeIndex;
		public int UpgradeTypeIndex {
			get { return upgradeTypeIndex; }
			set {
				if (value < 0) {
					var oldVal = upgradeTypeIndex;
					SetProperty (ref upgradeTypeIndex, value);
					SetProperty (ref upgradeTypeIndex, oldVal);
				} else if (value != shipIndex)
					SetProperty (ref upgradeTypeIndex, value);

				if (upgradeTypeIndex > -1)
					Upgrade.Category = UpgradeTypes [upgradeTypeIndex];
			}
		}

		ObservableCollection<Faction> factions;
		public ObservableCollection<Faction> Factions {
			get { return factions; }
			set {
				SetProperty (ref factions, value);
			}
		}

		ObservableCollection<Ship> ships;
		public ObservableCollection<Ship> Ships {
			get { return ships; }
			set {
				SetProperty (ref ships, value);
			}
		}

		ObservableCollection<string> upgradeTypes;
		public ObservableCollection<string> UpgradeTypes {
			get { return upgradeTypes; }
			set {
				SetProperty (ref upgradeTypes, value);
			}
		}

		Command saveUpgrade;
		public Command SaveUpgrade {
			get {
				if (saveUpgrade == null)
					saveUpgrade = new Command (() => {
						if (string.IsNullOrWhiteSpace (Name))
							return;

						XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

						if (Upgrade.Upgrades.Count (u => u.Name == Name) > 0)
							return;

						if (Create && Upgrade.CustomUpgrades.Count (u => u.Name == Name) > 0)
							return;

						XElement element;
						if (Create) {
							element = new XElement ("Upgrade",
								new XAttribute ("id", Upgrade.Id),
								new XElement ("Name", Name),
								new XElement ("Cost", Cost),
								new XElement ("Text", Text),
								new XElement ("Unique", IsUnique),
								new XElement ("Limited", IsLimited),
								new XElement ("ShipRequirement", Upgrade.ShipRequirement),
				                        	new XElement ("Faction", Upgrade.Factions != null && Upgrade.Factions.Count > 0 ? Upgrade.Factions [0]?.Id : null),
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
								category = new XElement ("Category",
									new XAttribute ("type", UpgradeTypes [UpgradeTypeIndex]),
									new XAttribute ("id", TypeToId [UpgradeTypes [UpgradeTypeIndex]]));
								customUpgradesXml.Add (category);
							}
							element.Add (new XElement ("CategoryId", category.Attribute ("id")?.ToString ()));

							category?.Add (element);
						} else {
							element = customUpgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("id")?.Value == OriginalXml.Attribute ("id")?.Value);
							element.SetAttributeValue ("id", Upgrade.Id);
							element.SetElementValue ("Name", Name);
							element.SetElementValue ("Cost", Cost);
							element.SetElementValue ("Text", Text);
							element.SetElementValue ("Unique", IsUnique);
							element.SetElementValue ("Limited", IsLimited);
							element.SetElementValue ("ShipRequirement", Upgrade.ShipRequirement);
							element.SetElementValue ("Faction", Upgrade.Factions != null && Upgrade.Factions.Count > 0 ? Upgrade.Factions [0]?.Id : null);
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

							if (Upgrade.Category != element.Parent.Attribute ("type").Value) {
								element.Remove ();

								var categoryElement = customUpgradesXml.Descendants ().FirstOrDefault (e => e.Attribute ("type").Value == Upgrade.Category);
								categoryElement.Add (element);
							}
						}


						DependencyService.Get<ISaveAndLoad> ().SaveText ("Upgrades_Custom.xml", customUpgradesXml.ToString ());

						if (Create)
							MessagingCenter.Send<EditUpgradeViewModel, Upgrade> (this, "Upgrade Created", Upgrade.Copy ());
						else
							MessagingCenter.Send<EditUpgradeViewModel, Upgrade> (this, "Finished Editing", Upgrade.Copy ());
					});
				return saveUpgrade;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Factions = new ObservableCollection<Faction> (Faction.AllFactions);
			Factions.Insert (0, new Faction { Name = "Any" });

			Ships = new ObservableCollection<Ship> (Ship.AllShips);
			Ships.Insert (0, new Ship { Name = "Any" });

			var upgrades = Upgrade.AllUpgrades;
			var upgradeTypes = upgrades.Select (u => u.Category).Distinct ();

			UpgradeTypes = new ObservableCollection<string> (upgradeTypes);
		}

		public Dictionary<string, string> TypeToId = new Dictionary<string, string> {
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
			{ "Turret", "turret" },
			{ "Tech", "tech" },
		};

	}
}

