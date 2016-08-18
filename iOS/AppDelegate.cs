using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Ioc;
using System.IO;
using System.Xml.Linq;
using Xamarin;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
//using Dropbox.CoreApi.iOS;
using Dropbox.Api;

namespace SquadBuilder.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		string appKey = "qms26ynz79cou3i";
		string appSecret = "sa9emnj6m74ofbm";

		Dictionary <string, string> deprecatedIds = new Dictionary <string, string> {
			{ "rebels",	"rebel" },
			{ "empire",	"imperial" },
			{ "tacticaljammers", "tacticaljammer" },
			{ "accuraccorrector", "accuracycorrector" },
			{ "commsrelay", "commrelay" },
			{ "milleniumfalcon", "millenniumfalcon" },
			{ "torkhilmux", "torkilmux" }
		};

		SaveAndLoad saveAndLoad;

		const string HasMigrated = "HasMigrated";

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Insights.Initialize("1396f6a6fc0e812ab8a8d84a01810917fd3940a6");


//			var session = new Session (appKey, appSecret, Session.RootAppFolder);
//			// The session that you have just created, will live through all the app
//			Session.SharedSession = session;
//			if (Session.SharedSession.IsLinked)
//				Console.WriteLine ("Already Linked");

#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif
			 
			saveAndLoad = new SaveAndLoad ();
			global::Xamarin.Forms.Forms.Init ();

			Forms.ViewInitialized += (sender, e) => {
				if (e.View.StyleId != null)
					e.NativeView.AccessibilityIdentifier = e.View.StyleId;
			};

			if (!Resolver.IsSet)
				SetIoc ();

			var collectionXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Collection", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Collection.xml"))
				saveAndLoad.SaveText ("Collection.xml", collectionXml);

			var referenceCardXml = new StreamReader (NSBundle.MainBundle.PathForResource ("ReferenceCards", "xml")).ReadToEnd ();
			var referenceCardsVersion = (float)XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ReferenceCardsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ReferenceCardsFilename)))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (Cards.ReferenceCardsFilename, referenceCardXml);
			
			UpdateIds ();

			var schemaJson = new StreamReader (NSBundle.MainBundle.PathForResource ("schema", "json")).ReadToEnd ();
			saveAndLoad.SaveText ("schema.json", schemaJson);

			var settingsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Settings", "xml")).ReadToEnd ();
			var settingsVersion = (float)XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.SettingsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.SettingsFilename)))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Cards.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;

			Settings.ReferenceCardsVersion = referenceCardsVersion;

			var factionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions3", "xml")).ReadToEnd ();
			Settings.FactionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.FactionsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.FactionsFilename)))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Cards.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);

			var shipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships3", "xml")).ReadToEnd ();
			Settings.ShipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ShipsFilename))
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ShipsFilename)))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
				Cards.SharedInstance.GetAllShips ();
			}

			var customShipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);

			var pilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots3", "xml")).ReadToEnd ();
			Settings.PilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.PilotsFilename))
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
				Cards.SharedInstance.GetAllPilots ();
			}

			var customPilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades3", "xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.UpgradesFilename))
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
				Cards.SharedInstance.GetAllUpgrades ();
			}

			var customUpgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Expansions3", "xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float)XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ExpansionsFilename)) {
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
			}
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ExpansionsFilename)))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
				Cards.SharedInstance.GetAllExpansions ();
			}
				
			Cards.SharedInstance.GetAllFactions ();
			Cards.SharedInstance.GetAllShips ();
			Cards.SharedInstance.GetAllPilots ();
			Cards.SharedInstance.GetAllUpgrades ();

			Cards.SharedInstance.GetAllSquadrons ();

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}

		void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (url.Scheme == "aurorasquadbuilder") {
			}

			return true;
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

			if (saveAndLoad.FileExists ("Factions_Custom.xml")) {
				var factionXml = saveAndLoad.LoadText ("Factions_Custom.xml");
				foreach (var key in deprecatedIds.Keys)
					factionXml = factionXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText ("Factions_Custom.xml", factionXml);
			}

			if (saveAndLoad.FileExists (Cards.ShipsFilename)) {
				var shipXml = saveAndLoad.LoadText (Cards.ShipsFilename);
				foreach (var key in deprecatedIds.Keys)
					shipXml = shipXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText (Cards.ShipsFilename, shipXml);
			}

			if (saveAndLoad.FileExists ("Ships_Custom.xml")) {
				var shipXml = saveAndLoad.LoadText ("Ships_Custom.xml");
				foreach (var key in deprecatedIds.Keys)
					shipXml = shipXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText ("Ships_Custom.xml", shipXml);
			}

			if (saveAndLoad.FileExists (Cards.PilotsFilename)) {
				var pilotXml = saveAndLoad.LoadText (Cards.PilotsFilename);
				foreach (var key in deprecatedIds.Keys)
					pilotXml = pilotXml.Replace (key, deprecatedIds [key]);
				pilotXml = pilotXml.Replace("baronoftheimperial", "baronoftheempire");
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotXml);
			}

			if (saveAndLoad.FileExists ("Pilots_Custom.xml")) {
				var pilotXml = saveAndLoad.LoadText ("Pilots_Custom.xml");
				foreach (var key in deprecatedIds.Keys)
					pilotXml = pilotXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText ("Pilots_Custom.xml", pilotXml);
			}

			if (saveAndLoad.FileExists (Cards.UpgradesFilename)) {
				var upgradeXml = saveAndLoad.LoadText (Cards.UpgradesFilename);
				foreach (var key in deprecatedIds.Keys)
					upgradeXml = upgradeXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradeXml);
			}

			if (saveAndLoad.FileExists ("Upgrades_Custom.xml")) {
				var upgradeXml = saveAndLoad.LoadText ("Upgrades_Custom.xml");
				foreach (var key in deprecatedIds.Keys)
					upgradeXml = upgradeXml.Replace (key, deprecatedIds [key]);
				saveAndLoad.SaveText ("Upgrades_Custom.xml", upgradeXml);
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


		}
	}
}