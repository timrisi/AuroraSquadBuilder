﻿using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class Cards : ObservableObject
	{
		public Cards ()
		{
			getAllFactions ();
			getAllShips ();
			getAllPilots ();
			getAllUpgrades ();
		}

		static Cards sharedInstance;
		public static Cards SharedInstance {
			get {
				if (sharedInstance == null)
					sharedInstance = new Cards ();

				return sharedInstance;
			}
		}

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get { return factions; }
			set { 
				SetProperty (ref factions, value); 
				factions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		ObservableCollection <Faction> customFactions;
		public ObservableCollection <Faction> CustomFactions {
			get { return customFactions; }
			set { 
				SetProperty (ref customFactions, value); 
				customFactions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		ObservableCollection <Faction> allFactions;
		public ObservableCollection <Faction> AllFactions {
			get {
				if (allFactions == null)
					updateAllFactions ();

				return allFactions;
			}
		}

		void updateAllFactions ()
		{
			var temp = factions.ToList ();
			temp.AddRange (customFactions);
			allFactions = new ObservableCollection <Faction> (temp);
		}

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { return ships; }
			set { 
				SetProperty (ref ships, value);
				ships.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		ObservableCollection <Ship> customShips;
		public ObservableCollection <Ship> CustomShips {
			get { return customShips; }
			set { 
				SetProperty (ref customShips, value);
				customShips.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		ObservableCollection <Ship> allShips;
		public ObservableCollection <Ship> AllShips {
			get {
				if (allShips == null)
					updateAllShips ();

				return allShips;
			}
		}

		void updateAllShips ()
		{
			var temp = ships.ToList ();
			temp.AddRange (customShips);
			allShips = new ObservableCollection <Ship> (temp);
		}

		ObservableCollection <Pilot> pilots;
		public ObservableCollection <Pilot> Pilots {
			get { return pilots; }
			set { 
				SetProperty (ref pilots, value); 
				pilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		ObservableCollection <Pilot> customPilots;
		public ObservableCollection <Pilot> CustomPilots {
			get { return customPilots; }
			set {
				SetProperty (ref customPilots, value);
				customPilots.CollectionChanged += (sender, e) => updateAllPilots ();
				updateAllPilots ();
			}
		}

		ObservableCollection <Pilot> allPilots;
		public ObservableCollection <Pilot> AllPilots {
			get {
				if (allPilots == null)
					updateAllPilots ();

				return allPilots;
			}
		}

		void updateAllPilots ()
		{
			var temp = pilots.ToList ();
			temp.AddRange (customPilots);
			allPilots = new ObservableCollection <Pilot> (temp);
		}

		ObservableCollection <Upgrade> upgrades;
		public ObservableCollection <Upgrade> Upgrades {
			get { return upgrades; }
			set { 
				SetProperty (ref upgrades, value);
				upgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		ObservableCollection <Upgrade> customUpgrades;
		public ObservableCollection <Upgrade> CustomUpgrades {
			get { return customUpgrades; }
			set { 
				SetProperty (ref customUpgrades, value);
				customUpgrades.CollectionChanged += (sender, e) => updateAllUpgrades ();
				updateAllUpgrades ();
			}
		}

		ObservableCollection <Upgrade> allUpgrades;
		public ObservableCollection <Upgrade> AllUpgrades {
			get {
				if (allUpgrades == null)
					updateAllUpgrades ();

				return allUpgrades;
			}
		}

		void updateAllUpgrades ()
		{
			var temp = upgrades.ToList ();
			temp.AddRange (customUpgrades);
			allUpgrades = new ObservableCollection <Upgrade> (temp);
		}

		void getAllFactions ()
		{
			XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
			factions = new ObservableCollection <Faction> ((from faction in factionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				})
			);

			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			customFactions = new ObservableCollection <Faction> ((from faction in customFactionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				})
			);

			updateAllFactions ();
		}

		void getAllShips ()
		{
			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships.xml")));
			ships = new ObservableCollection < Ship> ((
				from ship in shipsElement.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
				})
			);

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			customShips = new ObservableCollection <Ship> ((
				from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool)ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool)ship.Element ("Huge") : false,
					Actions = new ObservableCollection <string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),

				})
			);

			updateAllShips ();
		}

		void getAllPilots ()
		{
			XElement pilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots.xml")));
			pilots = new ObservableCollection <Pilot> (from pilot in pilotsXml.Elements ()
			                                           select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				Faction = factions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = ships.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = false,
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
			});

			XElement customPilotsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Pilots_Custom.xml")));
			customPilots = new ObservableCollection <Pilot> (from pilot in customPilotsXml.Elements ()
			                                                 select new Pilot {
				Id = pilot.Attribute ("id").Value,
				Name = pilot.Element ("Name").Value,
				Faction = allFactions.FirstOrDefault (f => f.Id == pilot.Attribute ("faction").Value),
				Ship = allShips.FirstOrDefault (f => f.Id == pilot.Attribute ("ship").Value),
				Unique = (bool)pilot.Element ("Unique"),
				BasePilotSkill = (int)pilot.Element ("PilotSkill"),
				BaseAttack = (int)pilot.Element ("Attack"),
				BaseAgility = (int)pilot.Element ("Agility"),
				BaseHull = (int)pilot.Element ("Hull"),
				BaseShields = (int)pilot.Element ("Shields"),
				BaseCost = (int)pilot.Element ("Cost"),
				Ability = pilot.Element ("Ability")?.Value,
				UpgradeTypes = new ObservableCollection <string> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).ToList ()),
				UpgradesEquipped = new ObservableCollection <Upgrade> (new List <Upgrade> (pilot.Element ("Upgrades").Elements ("Upgrade").Select (e => e.Value).Count ())),
				IsCustom = (bool)pilot.Element ("Custom"),
				Preview = pilot.Element ("Preview") != null ? (bool)pilot.Element ("Preview") : false,
			});

			updateAllPilots ();
		}

		void getAllUpgrades ()
		{
			XElement upgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades.xml")));
			List <Upgrade> allUpgrades = new List <Upgrade> ();

			foreach (var category in upgradesXml.Elements ()) {
				var categoryUpgrades = (from upgrade in category.Elements ()
				                        select new Upgrade {
					Id = upgrade.Attribute ("id").Value,
					Name = upgrade.Element ("Name")?.Value,
					CategoryId = upgrade.Parent.Attribute ("id").Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
					Text = upgrade.Element ("Text")?.Value,
					Faction = factions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					Ship = ships.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
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
					Preview = upgrade.Element ("Preview") != null ? (bool)upgrade.Element ("Preview") : false,
					TIEOnly = upgrade.Element ("TIEOnly") != null ? (bool)upgrade.Element ("TIEOnly") : false,
					AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
					                                                        select upgr.Value).ToList ()),
					Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
					                                           select upgr.Value).ToList ())
				});

				allUpgrades.AddRange (categoryUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));
			}

			upgrades = new ObservableCollection <Upgrade> (allUpgrades);

			XElement customUpgradesXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Upgrades_Custom.xml"))); 
			List <Upgrade> allCustomUpgrades = new List <Upgrade> ();

			foreach (var customCategory in customUpgradesXml.Elements ()) {

				var categoryCustomUpgrades = (from upgrade in customCategory.Elements ()
				                              select new Upgrade {
					Name = upgrade.Element ("Name")?.Value,
					Category = upgrade.Parent.Attribute ("type")?.Value,
					Cost = (int)upgrade.Element ("Cost"),
//					Cost = upgrade.Element ("Cost") != null ? (int)upgrade.Element ("Cost") : 0,
					Text = upgrade.Element ("Text")?.Value,
					Faction = allFactions.FirstOrDefault (f => f.Id == upgrade.Element ("Faction")?.Value),
					Ship = allShips.FirstOrDefault (s => s.Id == upgrade.Element ("Ship")?.Value),
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
					Preview = upgrade.Element ("Preview") != null ? (bool)upgrade.Element ("Preview") : false,
					TIEOnly = upgrade.Element ("TIEOnly") != null ? (bool)upgrade.Element ("TIEOnly") : false,
					AdditionalUpgrades = new ObservableCollection<string> ((from upgr in upgrade.Element ("AdditionalUpgrades") != null ? upgrade.Element ("AdditionalUpgrades").Elements () : new List <XElement> ()
					                                                         select upgr.Value).ToList ()),
					Slots = new ObservableCollection<string> ((from upgr in upgrade.Element ("ExtraSlots") != null ? upgrade.Element ("ExtraSlots").Elements () : new List <XElement> ()
					                                            select upgr.Value).ToList ())
				}
				                             );

				allCustomUpgrades.AddRange (categoryCustomUpgrades);
			}

			customUpgrades = new ObservableCollection <Upgrade> (allCustomUpgrades.OrderBy (u => u.Name).OrderBy (u => u.Cost));

			updateAllUpgrades ();
		}
	}
}
