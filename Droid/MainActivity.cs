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
			var referenceCardsVersion = (float) XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ReferenceCardsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ReferenceCardsFilename)))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (Cards.ReferenceCardsFilename, referenceCardXml);
			Settings.ReferenceCardsVersion = referenceCardsVersion;

			var settingsXml = new StreamReader (Application.Context.Assets.Open (Cards.SettingsFilename)).ReadToEnd ();
			var settingsVersion = (float) XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.SettingsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.SettingsFilename)))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Cards.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;

			var factionsXml = new StreamReader (Application.Context.Assets.Open ("Factions3.xml")).ReadToEnd ();
			Settings.FactionsVersion = (float) XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.FactionsFilename) || (float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.FactionsFilename)))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Cards.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (Application.Context.Assets.Open ("Factions_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);
			
			var shipsXml = new StreamReader (Application.Context.Assets.Open ("Ships3.xml")).ReadToEnd ();
			Settings.ShipsVersion = (float) XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ShipsFilename))
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ShipsFilename)))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
				Cards.SharedInstance.GetAllShips ();
			}

			var customShipsXml = new StreamReader (Application.Context.Assets.Open ("Ships_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Ships_Custom.xml")))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);
			
			var pilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots3.xml")).ReadToEnd ();
			Settings.PilotsVersion = (float) XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.PilotsFilename))
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
				Cards.SharedInstance.GetAllPilots ();
			}
			
			var customPilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Pilots_Custom.xml")))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);
			
			var upgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades3.xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float) XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.UpgradesFilename))
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
			else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
				Cards.SharedInstance.GetAllUpgrades ();
			}
			
			var customUpgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml") || string.IsNullOrEmpty (saveAndLoad.LoadText ("Upgrades_Custom.xml")))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (Application.Context.Assets.Open ("Expansions3.xml")).ReadToEnd ();
			Settings.ExpansionsVersion = (float) XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ExpansionsFilename)) {
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
			} else if ((float) XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ExpansionsFilename)))?.Attribute ("Version") < Settings.ExpansionsVersion) {
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
				Cards.SharedInstance.GetAllExpansions ();
			}


			Cards.SharedInstance.GetAllFactions ();
			Cards.SharedInstance.GetAllShips ();
			Cards.SharedInstance.GetAllPilots ();
			Cards.SharedInstance.GetAllUpgrades ();

			Cards.SharedInstance.GetAllSquadrons ();

			LoadApplication(new App());
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}