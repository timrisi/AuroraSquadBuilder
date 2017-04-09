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
using Dropbox.Api;
using HockeyApp.iOS;

namespace SquadBuilder.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		SaveAndLoad saveAndLoad;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure ("ce337527cc114b12805fcf7477297f40");
			manager.StartManager ();
			//manager.Authenticator.AuthenticateInstallation ();

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
	}
}