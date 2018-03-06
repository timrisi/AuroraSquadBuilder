using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Xml.Linq;
using Xamarin;
using System.Linq;
using System.Collections.Generic;
using PerpetualEngine.Storage;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;

namespace SquadBuilder.Droid
{
	[Activity (Label = "Aurora", 
		Icon="@drawable/icon", 
		MainLauncher = true, 
		Theme = "@style/CustomTheme",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		SaveAndLoad saveAndLoad;

		Dictionary <string, string> deprecatedIds = new Dictionary <string, string> {
			{ "rebels",	"rebel" },
			{ "empire",	"imperial" },
			{ "tacticaljammers", "tacticaljammer" },
			{ "accuraccorrector", "accuracycorrector" },
			{ "commsrelay", "commrelay" },
			{ "milleniumfalcon", "millenniumfalcon" },
			{ "torkhilmux", "torkilmux" }
		};

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SimpleStorage.SetContext (ApplicationContext);

			CrashManager.Register (this, "ce337527cc114b12805fcf7477297f40");
			MetricsManager.Register (Application, "ce337527cc114b12805fcf7477297f40");
			SendMail.ApplicationContext = ApplicationContext;

			saveAndLoad = new SaveAndLoad ();

			Xamarin.Forms.Forms.SetFlags ("FastRenderers_Experimental");
			global::Xamarin.Forms.Forms.Init (this, bundle);

			Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) => {
				if (!string.IsNullOrEmpty (e.View.StyleId))
					e.NativeView.ContentDescription = e.View.StyleId;
			};

			var collectionXml = new StreamReader (Application.Context.Assets.Open ("Collection.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Collection.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Collection.xml")))
				saveAndLoad.SaveText ("Collection.xml", collectionXml);
			
			var settingsXml = new StreamReader (Application.Context.Assets.Open (Settings.SettingsFilename)).ReadToEnd ();
			var settingsVersion = (float) XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Settings.SettingsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Settings.SettingsFilename)))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Settings.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;

			var referenceCardXml = new StreamReader (Application.Context.Assets.Open ("ReferenceCards.xml")).ReadToEnd ();
			var referenceCardsVersion = (float) XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (App.ReferenceCardsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (App.ReferenceCardsFilename)))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (App.ReferenceCardsFilename, referenceCardXml);
			Settings.ReferenceCardsVersion = referenceCardsVersion;

			var factionsXml = new StreamReader (Application.Context.Assets.Open ("Factions3.xml")).ReadToEnd ();
			Settings.FactionsVersion = (float) XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Faction.FactionsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Faction.FactionsFilename)))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Faction.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (Application.Context.Assets.Open ("Factions_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);
			
			var shipsXml = new StreamReader (Application.Context.Assets.Open ("Ships3.xml")).ReadToEnd ();
			Settings.ShipsVersion = (float) XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Ship.ShipsFilename))
				saveAndLoad.SaveText (Ship.ShipsFilename, shipsXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Ship.ShipsFilename)))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Ship.ShipsFilename, shipsXml);
				Ship.GetAllShips ();
			}

			var customShipsXml = new StreamReader (Application.Context.Assets.Open ("Ships_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Ships_Custom.xml")))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);
			
			var pilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots3.xml")).ReadToEnd ();
			Settings.PilotsVersion = (float) XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Pilot.PilotsFilename))
				saveAndLoad.SaveText (Pilot.PilotsFilename, pilotsXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Pilot.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Pilot.PilotsFilename, pilotsXml);
				Pilot.GetAllPilots ();
			}
			
			var customPilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Pilots_Custom.xml")))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);
			
			var upgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades3.xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float) XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Upgrade.UpgradesFilename))
				saveAndLoad.SaveText (Upgrade.UpgradesFilename, upgradesXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Upgrade.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Upgrade.UpgradesFilename, upgradesXml);
				Upgrade.GetAllUpgrades ();
			}
			
			var customUpgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Upgrades_Custom.xml")))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (Application.Context.Assets.Open ("Expansions3.xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float) XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Expansion.ExpansionsFilename)) {
				saveAndLoad.SaveText (Expansion.ExpansionsFilename, expansionsXml);
			} else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Expansion.ExpansionsFilename)))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				saveAndLoad.SaveText (Expansion.ExpansionsFilename, expansionsXml);
				Expansion.GetAllExpansions ();
			}
			
			Faction.GetAllFactions ();
			Ship.GetAllShips ();
			Pilot.GetAllPilots ();
			Upgrade.GetAllUpgrades ();
			Squadron.GetAllSquadrons ();

			LoadApplication (new App());
		}
	}
}