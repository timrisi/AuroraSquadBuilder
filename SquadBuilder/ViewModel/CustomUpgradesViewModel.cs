using System;
using XLabs.Forms.Mvvm;
using System.IO;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XLabs;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CustomUpgradesViewModel : ViewModel
	{
		public CustomUpgradesViewModel ()
		{
			XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml")));

			XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
			var factions = (from faction in factionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				}).ToList ();

			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			var customFactions = (from faction in customFactionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				});
			factions.AddRange (customFactions);

			List <Upgrade> upgrades = new List<Upgrade> ();

			foreach (var upgradeElement in customUpgradesXml.Elements ()) {
				upgrades.AddRange ((from upgrade in upgradeElement.Elements ()
				                    select new Upgrade {
					Id = upgrade.Attribute ("id")?.Value,
					Name = upgrade.Element ("Name")?.Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction =  (from faction in factionsXml.Elements ()
						select new Faction {
							Id = faction.Attribute ("id").Value,
							Name = faction.Value,
							Color = Color.FromRgb (
								(int)faction.Element ("Color").Attribute ("r"),
								(int)faction.Element ("Color").Attribute ("g"),
								(int)faction.Element ("Color").Attribute ("b")
							)
						}).FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					PilotSkill = upgrade.Element ("PilotSkill") != null ? (int)upgrade.Element ("PilotSkill") : 0,
					Attack = upgrade.Element ("Attack") != null ? (int)upgrade.Element ("Attack") : 0,
					Agility = upgrade.Element ("Agility") != null ? (int)upgrade.Element ("Agility") : 0,
					Hull = upgrade.Element ("Hull") != null ? (int)upgrade.Element ("Hull") : 0,
					Shields = upgrade.Element ("Shields") != null ? (int)upgrade.Element ("Shields") : 0,
					SecondaryWeapon = upgrade.Element ("SecondaryWeapon") != null ? (bool)upgrade.Element ("SecondaryWeapon") : false,
					Dice = upgrade.Element ("Dice") != null ? (int)upgrade.Element ("Dice") : 0,
					Range = upgrade.Element ("Range")?.Value,
					Unique = upgrade.Element ("Unique") != null ? (bool)upgrade.Element ("Unique") : false,
					Limited = upgrade.Element ("Limited") != null ? (bool)upgrade.Element ("Limited") : false,
					SmallOnly = upgrade.Element ("SmallOnly") != null ? (bool)upgrade.Element ("SmallOnly") : false,
					LargeOnly = upgrade.Element ("LargeOnly") != null ? (bool)upgrade.Element ("LargeOnly") : false,
					HugeOnly = upgrade.Element ("HugeOnly") != null ? (bool)upgrade.Element ("HugeOnly") : false,
					AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
					                                                         select upgr.Value).ToList ()),
					Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
					                                            select upgr.Value).ToList ())
				}).OrderBy (u => u.Name).OrderBy (u => u.Cost));
			}

			var <UpgradeGroup> allUpgradeGroups = new ObservableCollection<UpgradeGroup> ();
			foreach (var upgrade in upgrades) {
				var upgradeGroup = allUpgradeGroups.FirstOrDefault (g => g.Category == upgrade.Category);

				if (upgradeGroup == null) {
					upgradeGroup = new UpgradeGroup (upgrade.Category);
					allUpgradeGroups.Add (upgradeGroup);
				}

				upgradeGroup.Add (upgrade);
			}

			Upgrades = allUpgradeGroups;

			MessagingCenter.Subscribe <Upgrade> (this, "Remove Upgrade", upgrade => {
				Upgrades.FirstOrDefault (g => g.Contains (upgrade))?.Remove (upgrade);
			});
		}

		public string PageName { get { return "Upgrades"; } }

		ObservableCollection <UpgradeGroup> upgrades;
		public ObservableCollection <UpgradeGroup> Upgrades {
			get {
				return upgrades;
			}
			set {
				SetProperty (ref upgrades, value);
			}
		}

		RelayCommand createUpgrade;
		public RelayCommand CreateUpgrade {
			get {
				if (createUpgrade == null)
					createUpgrade = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created", (vm, upgrade) => {
							var upgradeGroup = Upgrades.FirstOrDefault (g => g.Category == upgrade.Category);

							if (upgradeGroup == null) {
								upgradeGroup = new UpgradeGroup (upgrade.Category);
								Upgrades.Add (upgradeGroup);
							}

							upgradeGroup.Add (upgrade);

							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <CreateUpgradeViewModel, Upgrade> (this, "Upgrade Created");
						});

						Navigation.PushAsync <CreateUpgradeViewModel> ();
					});

				return createUpgrade;
			}
		}
	}
}

