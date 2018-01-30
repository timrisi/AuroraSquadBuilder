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
//using Microsoft.Azure.Mobile;
//using Microsoft.Azure.Mobile.Analytics;
//using Microsoft.Azure.Mobile.Crashes;

namespace SquadBuilder.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		public static class ShortcutIdentifier {
			public const string CreateRebel = "com.risiapps.squadbuilder.000";
			public const string CreateImperial = "com.risiapps.squadbuilder.001";
			public const string CreateScum = "com.risiapps.squadbuilder.002";
			public const string BrowseCards = "com.risiapps.squadbuilder.003";
		}

		public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

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

			//MobileCenter.Start ("305be59c-cc7b-471d-80b3-5cc4b4790f59", typeof (Analytics), typeof (Crashes));

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

			var settingsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Settings", "xml")).ReadToEnd ();
			var settingsVersion = (float) XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Settings.SettingsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Settings.SettingsFilename)))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Settings.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;

			var referenceCardXml = new StreamReader (NSBundle.MainBundle.PathForResource ("ReferenceCards", "xml")).ReadToEnd ();
			var referenceCardsVersion = (float)XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (App.ReferenceCardsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (App.ReferenceCardsFilename)))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (App.ReferenceCardsFilename, referenceCardXml);
			Settings.ReferenceCardsVersion = referenceCardsVersion;

			var factionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions3", "xml")).ReadToEnd ();
			Settings.FactionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Faction.FactionsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Faction.FactionsFilename)))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Faction.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);

			var shipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships3", "xml")).ReadToEnd ();
			Settings.ShipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Ship.ShipsFilename))
				saveAndLoad.SaveText (Ship.ShipsFilename, shipsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Ship.ShipsFilename)))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Ship.ShipsFilename, shipsXml);
				Ship.GetAllShips ();
			}

			var customShipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);

			var pilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots3", "xml")).ReadToEnd ();
			Settings.PilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Pilot.PilotsFilename))
				saveAndLoad.SaveText (Pilot.PilotsFilename, pilotsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Pilot.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Pilot.PilotsFilename, pilotsXml);
				Pilot.GetAllPilots ();
			}

			var customPilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades3", "xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Upgrade.UpgradesFilename))
				saveAndLoad.SaveText (Upgrade.UpgradesFilename, upgradesXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Upgrade.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Upgrade.UpgradesFilename, upgradesXml);
				Upgrade.GetAllUpgrades ();
			}

			var customUpgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Expansions3", "xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float)XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Expansion.ExpansionsFilename)) {
				saveAndLoad.SaveText (Expansion.ExpansionsFilename, expansionsXml);
			}
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Expansion.ExpansionsFilename)))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				saveAndLoad.SaveText (Expansion.ExpansionsFilename, expansionsXml);
				Expansion.GetAllExpansions ();
			}
				
			Faction.GetAllFactions ();
			Ship.GetAllShips ();
			Pilot.GetAllPilots ();
			Upgrade.GetAllUpgrades ();

			Squadron.GetAllSquadrons ();

			LoadApplication (new App ());

			// Get possible shortcut item
			if (options != null)
				LaunchedShortcutItem = options [UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;

			return base.FinishedLaunching (app, options);
		}

		void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}

		public bool HandleShortcutItem (UIApplicationShortcutItem shortcutItem)
		{
			var handled = false;

			// Anything to process?
			if (shortcutItem == null) return false;

			// Take action based on the shortcut type
			switch (shortcutItem.Type) {
			case ShortcutIdentifier.CreateRebel:
				MessagingCenter.Send (App.Current, "Create Rebel");
				handled = true;
				break;
			case ShortcutIdentifier.CreateImperial:
				MessagingCenter.Send (App.Current, "Create Imperial");
				handled = true;
				break;
			case ShortcutIdentifier.CreateScum:
				MessagingCenter.Send (App.Current, "Create Scum");
				handled = true;
				break;
			case ShortcutIdentifier.BrowseCards:
				MessagingCenter.Send (App.Current, "BrowseCards");
				handled = true;
				break;
			}

			// Return results
			return handled;
		}

		public override void OnActivated (UIApplication application)
		{
			// Handle any shortcut item being selected
			HandleShortcutItem (LaunchedShortcutItem);

			// Clear shortcut after it's been handled
			LaunchedShortcutItem = null;
		}

		public override void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
		{
			// Perform action
			completionHandler (HandleShortcutItem (shortcutItem));
		}

	}
}