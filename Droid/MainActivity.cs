﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Forms;
using XLabs.Ioc;
using System.IO;
using System.Xml.Linq;
using Xamarin;
using System.Linq;

namespace SquadBuilder.Droid
{
	[Activity (Label = "Aurora SB", 
		Icon="@drawable/icon", 
		MainLauncher = true, 
		Theme = "@style/CustomTheme",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid
	{
		SaveAndLoad saveAndLoad;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			Insights.Initialize("1396f6a6fc0e812ab8a8d84a01810917fd3940a6", BaseContext);

			saveAndLoad = new SaveAndLoad ();

			global::Xamarin.Forms.Forms.Init(this, bundle);

			Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) => {
				if (!string.IsNullOrEmpty (e.View.StyleId))
					e.NativeView.ContentDescription = e.View.StyleId;
			};

			if(!Resolver.IsSet) SetIoc();
			var settingsXml = new StreamReader (Application.Context.Assets.Open ("Settings.xml")).ReadToEnd ();
			var settingsVersion = (float)XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Settings.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Settings.xml")))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText ("Settings.xml", settingsXml);
			Settings.SettingsVersion = settingsVersion;

			var factionsXml = new StreamReader (Application.Context.Assets.Open ("Factions.xml")).ReadToEnd ();
			Settings.FactionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Factions.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Factions.xml")))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText ("Factions.xml", factionsXml);

			var customFactionsXml = new StreamReader (Application.Context.Assets.Open ("Factions_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);
			
			var shipsXml = new StreamReader (Application.Context.Assets.Open ("Ships.xml")).ReadToEnd ();
			Settings.ShipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Ships.xml"))
				saveAndLoad.SaveText ("Ships.xml", shipsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Ships.xml")))?.Attribute ("Version") < Settings.ShipsVersion) {
				var oldShips = Cards.SharedInstance.Ships;

				saveAndLoad.SaveText ("Ships.xml", shipsXml);
				Cards.SharedInstance.GetAllShips ();

				foreach (var ship in oldShips) {
					if (Cards.SharedInstance.Ships.Any (s => s.Id == ship.Id))
						Cards.SharedInstance.Ships.First (s => s.Id == ship.Id).Owned = ship.Owned;
				}
			}

			var customShipsXml = new StreamReader (Application.Context.Assets.Open ("Ships_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);
			
			var pilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots.xml")).ReadToEnd ();
			Settings.PilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Pilots.xml"))
				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Pilots.xml")))?.Attribute ("Version") < Settings.PilotsVersion) {
				var oldPilots = Cards.SharedInstance.Pilots;

				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);
				Cards.SharedInstance.GetAllPilots ();

				foreach (var pilot in oldPilots) {
					if (Cards.SharedInstance.Pilots.Any (p => p.Id == pilot.Id && p.Ship.Id == pilot.Ship.Id && p.Faction.Id == pilot.Faction.Id))
						Cards.SharedInstance.Pilots.First (p => p.Id == pilot.Id && p.Ship.Id == pilot.Ship.Id && p.Faction.Id == pilot.Faction.Id).Owned = pilot.Owned;
				}
			}
			
			var customPilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades.xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Upgrades.xml"))
				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Upgrades.xml")))?.Attribute ("Version") < Settings.UpgradesVersion) {
				var oldUpgrades = Cards.SharedInstance.Upgrades;

				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);
				Cards.SharedInstance.GetAllUpgrades ();

				foreach (var upgrade in oldUpgrades) {
					if (Cards.SharedInstance.Upgrades.Any (u => u.Id == upgrade.Id && u.CategoryId == upgrade.CategoryId))
						Cards.SharedInstance.Upgrades.First (u => u.Id == upgrade.Id && u.CategoryId == upgrade.CategoryId).Owned = upgrade.Owned;
				}
			}
			
			var customUpgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (Application.Context.Assets.Open ("Expansions.xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float)XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Expansions.xml"))
				saveAndLoad.SaveText ("Expansions.xml", expansionsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Expansions.xml")))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				var oldExpansions = Cards.SharedInstance.Expansions;			

				saveAndLoad.SaveText ("Expansions.xml", expansionsXml);
				Cards.SharedInstance.GetAllExpansions ();

				foreach (var expansion in oldExpansions) {
					if (Cards.SharedInstance.Expansions.Any (e => e.Id == expansion.Id))
						Cards.SharedInstance.Expansions.First (e => e.Id == expansion.Id).owned = expansion.owned;
				}
			}
			
			LoadApplication(new App());
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}