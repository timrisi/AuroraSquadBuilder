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
			base.OnCreate(bundle);

			SimpleStorage.SetContext (ApplicationContext);

			Insights.Initialize("1396f6a6fc0e812ab8a8d84a01810917fd3940a6", BaseContext);

			saveAndLoad = new SaveAndLoad ();

			global::Xamarin.Forms.Forms.Init(this, bundle);

			Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) => {
				if (!string.IsNullOrEmpty (e.View.StyleId))
					e.NativeView.ContentDescription = e.View.StyleId;
			};

			if(!Resolver.IsSet) 
				SetIoc();

			var collectionXml = new StreamReader (Application.Context.Assets.Open ("Collection.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Collection.xml"))
				saveAndLoad.SaveText ("Collection.xml", collectionXml);

			var referenceCardXml = new StreamReader (Application.Context.Assets.Open ("ReferenceCards.xml")).ReadToEnd ();
			var referenceCardsVersion = (float)XElement.Load (new StringReader (referenceCardXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ReferenceCardsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ReferenceCardsFilename)))?.Attribute ("Version") < referenceCardsVersion)
				saveAndLoad.SaveText (Cards.ReferenceCardsFilename, referenceCardXml);
			Settings.ReferenceCardsVersion = referenceCardsVersion;

			UpdateIds ();

			var schemaJson = new StreamReader (Application.Context.Assets.Open ("schema.json")).ReadToEnd ();
			saveAndLoad.SaveText ("schema.json", schemaJson);

			var settingsXml = new StreamReader (Application.Context.Assets.Open (Cards.SettingsFilename)).ReadToEnd ();
			var settingsVersion = (float)XElement.Load (new StringReader (settingsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.SettingsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.SettingsFilename)))?.Attribute ("Version") < settingsVersion)
				saveAndLoad.SaveText (Cards.SettingsFilename, settingsXml);
			Settings.SettingsVersion = settingsVersion;

			var factionsXml = new StreamReader (Application.Context.Assets.Open ("Factions2.xml")).ReadToEnd ();
			Settings.FactionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.FactionsFilename) || (float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.FactionsFilename)))?.Attribute ("Version") < Settings.FactionsVersion)
				saveAndLoad.SaveText (Cards.FactionsFilename, factionsXml);

			var customFactionsXml = new StreamReader (Application.Context.Assets.Open ("Factions_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);
			
			var shipsXml = new StreamReader (Application.Context.Assets.Open ("Ships2.xml")).ReadToEnd ();
			Settings.ShipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ShipsFilename))
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ShipsFilename)))?.Attribute ("Version") < Settings.ShipsVersion) {
				saveAndLoad.SaveText (Cards.ShipsFilename, shipsXml);
				Cards.SharedInstance.GetAllShips ();
			}

			var customShipsXml = new StreamReader (Application.Context.Assets.Open ("Ships_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);
			
			var pilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots2.xml")).ReadToEnd ();
			Settings.PilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.PilotsFilename))
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.PilotsFilename)))?.Attribute ("Version") < Settings.PilotsVersion) {
				saveAndLoad.SaveText (Cards.PilotsFilename, pilotsXml);
				Cards.SharedInstance.GetAllPilots ();
			}
			
			var customPilotsXml = new StreamReader (Application.Context.Assets.Open ("Pilots_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades2.xml")).ReadToEnd ();
			Settings.UpgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.UpgradesFilename))
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.UpgradesFilename)))?.Attribute ("Version") < Settings.UpgradesVersion) {
				saveAndLoad.SaveText (Cards.UpgradesFilename, upgradesXml);
				Cards.SharedInstance.GetAllUpgrades ();
			}
			
			var customUpgradesXml = new StreamReader (Application.Context.Assets.Open ("Upgrades_Custom.xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			var expansionsXml = new StreamReader (Application.Context.Assets.Open (Cards.ExpansionsFilename)).ReadToEnd ();
			Settings.ExpansionsVersion = (float)XElement.Load (new StringReader (expansionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists (Cards.ExpansionsFilename))
				saveAndLoad.SaveText (Cards.ExpansionsFilename, expansionsXml);
			else if ((float)XElement.Load (new StringReader (saveAndLoad.LoadText (Cards.ExpansionsFilename)))?.Attribute ("Version") < Settings.ExpansionsVersion) {
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