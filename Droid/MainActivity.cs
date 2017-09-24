using System;

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
	public class MainActivity : XFormsApplicationDroid
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

			global::Xamarin.Forms.Forms.Init (this, bundle);

			Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) => {
				if (!string.IsNullOrEmpty (e.View.StyleId))
					e.NativeView.ContentDescription = e.View.StyleId;
			};

			if (!Resolver.IsSet)
				SetIoc ();

			var collectionXml = new StreamReader (Application.Context.Assets.Open ("Collection.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Collection.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Collection.xml")))
				saveAndLoad.SaveText ("Collection.xml", collectionXml);

			var referenceCardXml = new StreamReader (Application.Context.Assets.Open ("ReferenceCards.xml")).ReadToEnd ();
			var referenceCardsVersion = (float)XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			var text = saveAndLoad.LoadText (Cards.ReferenceCardsFilename);
			if (!saveAndLoad.FileExists (Cards.ReferenceCardsFilename) || string.IsNullOrEmpty (text) ||  (float)XElement.Load (new StringReader (text))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (Cards.ReferenceCardsFilename, referenceCardXml);

			UpdateIds ();

			var settingsXml = new StreamReader (Application.Context.Assets.Open (Cards.SettingsFilename)).ReadToEnd ();
			var settingsVersion = (float)XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.SettingsFilename);
			if (!saveAndLoad.FileExists (Cards.SettingsFilename) || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (text))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Cards.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;
			Settings.ReferenceCardsVersion = referenceCardsVersion;

			var factionsXml = new StreamReader (Application.Context.Assets.Open ("Factions3.xml")).ReadToEnd ();
			Settings.FactionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.FactionsFilename);
			if (!saveAndLoad.FileExists (Cards.FactionsFilename) || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (text))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Cards.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (Application.Context.Assets.Open ("Factions_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Factions_Custom.xml")))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);
			
			var shipsXml = new StreamReader (Application.Context.Assets.Open ("Ships3.xml")).ReadToEnd ();
			Settings.ShipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.ShipsFilename);
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
			if (!saveAndLoad.FileExists (Cards.ShipsFilename) || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (text))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
				Cards.SharedInstance.GetAllShips ();
			}

			var customShipsXml = new StreamReader (Application.Context.Assets.Open ("Ships_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Ships_Custom.xml")))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);
			
			var pilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots3.xml")).ReadToEnd ();
			Settings.PilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.PilotsFilename);
			if (!saveAndLoad.FileExists (Cards.PilotsFilename) || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
				Cards.SharedInstance.GetAllPilots ();
			}
			
			var customPilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Pilots_Custom.xml")))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades3.xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.UpgradesFilename);
			if (!saveAndLoad.FileExists (Cards.UpgradesFilename)  || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
				Cards.SharedInstance.GetAllUpgrades ();
			}
			
			var customUpgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Upgrades_Custom.xml")))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (Application.Context.Assets.Open ("Expansions3.xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float)XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			text = saveAndLoad.LoadText (Cards.ExpansionsFilename);
			if (!saveAndLoad.FileExists (Cards.ExpansionsFilename) || string.IsNullOrEmpty (text) || (float)XElement.Load (new StringReader (text))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
				Cards.SharedInstance.GetAllExpansions ();
			}


			Cards.SharedInstance.GetAllFactions ();
			Cards.SharedInstance.GetAllShips ();
			Cards.SharedInstance.GetAllPilots ();
			Cards.SharedInstance.GetAllUpgrades ();
			
			LoadApplication(new App());
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}

		void UpdateIds ()
		{
			if (!saveAndLoad.FileExists (Cards.SquadronsFilename))
				return; 

			if (saveAndLoad.FileExists (Cards.FactionsFilename)) {
				var factionXml = saveAndLoad.LoadText (Cards.FactionsFilename);
				foreach (var key in deprecatedIds.Keys)
					factionXml = factionXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText (Cards.FactionsFilename, factionXml);
			}

			if (saveAndLoad.FileExists (Cards.ShipsFilename)) {
				var shipXml = saveAndLoad.LoadText (Cards.ShipsFilename);
				foreach (var key in deprecatedIds.Keys)
					shipXml = shipXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText (Cards.ShipsFilename, shipXml);
			}

			if (saveAndLoad.FileExists (Cards.PilotsFilename)) {
				var pilotXml = saveAndLoad.LoadText (Cards.PilotsFilename);
				foreach (var key in deprecatedIds.Keys)
					pilotXml = pilotXml.Replace (key, deprecatedIds [key]);
				pilotXml = pilotXml.Replace("baronoftheimperial", "baronoftheempire");
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotXml);
			}

			if (saveAndLoad.FileExists (Cards.UpgradesFilename)) {
				var upgradeXml = saveAndLoad.LoadText (Cards.UpgradesFilename);
				foreach (var key in deprecatedIds.Keys)
					upgradeXml = upgradeXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradeXml);
			}

			if (saveAndLoad.FileExists (Cards.ExpansionsFilename)) {
				var expansionXml = saveAndLoad.LoadText (Cards.ExpansionsFilename);
				foreach (var key in deprecatedIds.Keys)
					expansionXml = expansionXml.Replace (key, deprecatedIds [key]);
				expansionXml = expansionXml.Replace("baronoftheimperial", "baronoftheempire");
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionXml);
			}

			if (saveAndLoad.FileExists (Cards.SquadronsFilename)) {
				var squadronXml = saveAndLoad.LoadText (Cards.SquadronsFilename);
				foreach (var key in deprecatedIds.Keys)
					squadronXml = squadronXml.Replace (key, deprecatedIds [key]);
				squadronXml = squadronXml.Replace("baronoftheimperial", "baronoftheempire");
				saveAndLoad.SaveText (Cards.SquadronsFilename, squadronXml);
			}

			Cards.SharedInstance.GetAllSquadrons ();
		}
	}
}