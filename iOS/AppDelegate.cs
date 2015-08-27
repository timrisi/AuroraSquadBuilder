using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Ioc;
using System.IO;
using System.Xml.Linq;
using Xamarin;

namespace SquadBuilder.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		SaveAndLoad saveAndLoad;

		const string HasMigrated = "HasMigrated";

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Insights.Initialize("1396f6a6fc0e812ab8a8d84a01810917fd3940a6");

			saveAndLoad = new SaveAndLoad ();
			global::Xamarin.Forms.Forms.Init ();

			if (!Resolver.IsSet)
				SetIoc ();

			var factionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions", "xml")).ReadToEnd ();
			var version = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
//			if (!saveAndLoad.FileExists ("Factions.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Factions.xml")))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Factions.xml", factionsXml);

			var customFactionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);

			var shipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
//			if (!saveAndLoad.FileExists ("Ships.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Ships.xml")))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Ships.xml", shipsXml);

			var customShipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);

			var pilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
//			if (!saveAndLoad.FileExists ("Pilots.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Pilots.xml")))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);

			var customPilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
//			if (!saveAndLoad.FileExists ("Upgrades.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Upgrades.xml")))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);

			var customUpgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			if (NSUserDefaults.StandardUserDefaults [HasMigrated] == null) {
					UpdateIds ();
					NSUserDefaults.StandardUserDefaults.SetBool (true, HasMigrated);
			}

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}

		void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}

		void UpdateIds ()
		{
			var deprecatedFactions = new Dictionary <string, string> {
				{ "rebel", 				"rebels" },
				{ "imperial", 			"empire" }
			};

			var deprecatedShips = new Dictionary <string, string> {
				{ "z95", 				"z95headhunter" },
				{ "scyk", 				"m3ainterceptor" },
				{ "kihraxz", 			"kihraxzfighter" },
				{ "firespray", 			"firespray31" },
				{ "gr75", 				"gr75mediumtransport" },
				{ "advanced", 			"tieadvanced" },
				{ "bomber", 			"tiebomber" },
				{ "defender", 			"tiedefender" },
				{ "fighter",			"tiefighter" },
				{ "interceptor", 		"tieinterceptor" },
				{ "phantom", 			"tiephantom" },
				{ "punisher", 			"tiepunisher" },
				{ "tieadvprototype", 	"tieadvancedprototype" },
				{ "lambda", 			"lambdaclassshuttle" },
				{ "decimator",			"vt49decimator" }
			};
				
			var deprecatedPilots = new Dictionary <string, string> {
				{ "xizor", 				"princexizor" },
				{ "dace", 				"dacebonearm" },
				{ "palob", 				"palobgodalhi" },
				{ "torkhil", 			"torkhilmux" },
				{ "drea", 				"drearenthal" },
				{ "thug", 				"syndicatethug" },
				{ "ndru", 				"ndrusuhlak" },
				{ "kaato", 				"kaatoleeachos" },
				{ "laetin", 			"laetinashera" },
				{ "talonbane", 			"talonbanecobra" },
				{ "graz", 				"grazthehunter" },
				{ "fett",				"bobafett" },
				{ "emon",				"emonazzameen" },
				{ "latts",				"lattsrazzi" },
				{ "tycho",				"tychocelchu" },
				{ "jake", 				"jakefarrell" },
				{ "arvel",				"arvelcrynyd" },
				{ "gemmer",				"gemmersojan" },
				{ "green",				"greensquadronpilot" },
				{ "keyan",				"keyanfarlander" },
				{ "nera",				"neradantels" },
				{ "dagger",				"daggersquadronpilot" },
				{ "blue", 				"bluesquadronpilot" },
				{ "corran",				"corranhorn" },
				{ "etahn",				"etahnabaht" },
				{ "blackmoon",			"blackmoonsquadronpilot" },
				{ "knave",				"knavesquadronpilot" },
				{ "jan",				"janors" },
				{ "kyle",				"kylekatarn" },
				{ "roark",				"roarkgarnet" },
				{ "miranda",			"mirandadoni" },
				{ "esege", 				"esegetuketu" },
				{ "guardian",			"guardiansquadronpilot" },
				{ "warden",				"wardensquadronpilot" },
				{ "wedge",				"wedgeantilles" },
				{ "wes",				"wesjanson" },
				{ "luke",				"lukeskywalker" },
				{ "jek",				"jekporkins" },
				{ "garven",				"garvendreis" },
				{ "biggs",				"biggsdarklighter" },
				{ "hobbie",				"hobbieklivian" },
				{ "red",				"redsquadronpilot" },
				{ "horton",				"hortonsalm" },
				{ "dutch",				"dutchvander" },
				{ "grey",				"graysquadronpilot" },
				{ "gold",				"goldsquadronpilot" },
				{ "airen",				"airencracken" },
				{ "blount",				"lieutenantblount" },
				{ "tala",				"talasquadronpilot" },
				{ "bandit",				"banditsquadronpilot" },
				{ "han",				"hansolo" },
				{ "lando",				"landocalrissian" },
				{ "dash",				"dashrendar" },
				{ "eaden",				"eadenvrill" },
				{ "fringer",			"wildspacefringer" },
				{ "hera",				"herasyndulla" },
				{ "gr75",				"gr75mediumtransport" },
				{ "Backstabber",		"backstabber" },
				{ "Lieutenant Colzet", 	"lieutenantcolzet" },
				{ "blackeightsqpilot",	"blackeightsquadronpilot" },
				{ "chiraneau",			"rearadmiralchiraneau" },
				{ "kenkirk",			"commanderkenkirk" },
				{ "oicunn",				"captainoicunn" }
			};

			var deprecatedUpgrades = new Dictionary <string, string> {
				{ "Gunner", "gunner" },
				{ "eh", "experthandling" },
				{ "st", "swarmtactics" },
				{ "sl", "squadleader" },
				{ "dtf", "drawtheirfire" },
				{ "vi", "veteraninstincts" },
				{ "ptl", "pushthelimit" },
				{ "ar", "adrenalinerush" },
				{ "lw", "lonewolf" },
				{ "sot", "stayontarget" },
				{ "advhomingmissiles", "advancedhomingmissiles" },
				{ "antipursuitlaser", "antipursuitlasers" },
				{ "twinionenginemki", "twinionenginemkii" },
				{ "quantomstorm", "quantumstorm" }
			};

			if (saveAndLoad.FileExists ("squadrons.xml")) {
				var squadronXml = XElement.Load (new StringReader (saveAndLoad.LoadText ("squadrons.xml")));
				var squadronElements = squadronXml.Elements ();
				foreach (var squadron in squadronElements) {
					foreach (var faction in squadron.Descendants ("Faction")) {
						string key = faction.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;
					
						if (deprecatedFactions.ContainsKey (key))
							faction.SetElementValue ("Id", deprecatedFactions [key]);
					}

					foreach (var ship in squadron.Descendants ("Ship")) {
						string key = ship.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;
					
						if (deprecatedShips.ContainsKey (key))
							ship.SetElementValue ("Id", deprecatedShips [key]);
					}

					foreach (var upgrade in squadron.Descendants ("Upgrade")) {
						string key = upgrade.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;
					
						if (deprecatedShips.ContainsKey (key))
							upgrade.SetElementValue ("Id", deprecatedUpgrades [key]);
					}
				}

				saveAndLoad.SaveText ("squadrons.xml", squadronXml.ToString ());
			}

			if (saveAndLoad.FileExists ("Pilots_Custom.xml")) {
				var pilotXml = XElement.Load (new StringReader (saveAndLoad.LoadText ("Pilots_Custom.xml")));
				var pilotElements = pilotXml.Elements ();
				foreach (var pilot in pilotElements) {
					foreach (var faction in pilot.Descendants ("Faction")) {
						string key = faction.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedFactions.ContainsKey (key))
							faction.SetElementValue ("Id", deprecatedFactions [key]);
					}

					foreach (var ship in pilot.Descendants ("Ship")) {
						string key = ship.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedShips.ContainsKey (key))
							ship.SetElementValue ("Id", deprecatedShips [key]);
					}

					foreach (var upgrade in pilot.Descendants ("Upgrade")) {
						string key = upgrade.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedShips.ContainsKey (key))
							upgrade.SetElementValue ("Id", deprecatedUpgrades [key]);
					}
				}

				saveAndLoad.SaveText ("Pilots_Custom.xml", pilotXml.ToString ());
			}

			if (saveAndLoad.FileExists ("Upgrades_Custom.xml")) {
				var upgradeXml = XElement.Load (new StringReader (saveAndLoad.LoadText ("Upgrades_Custom.xml")));
				var upgradeElements = upgradeXml.Elements ();
				foreach (var upgrade in upgradeElements) {
					foreach (var faction in upgrade.Descendants ("Faction")) {
						string key = faction.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedFactions.ContainsKey (key))
							faction.SetElementValue ("Id", deprecatedFactions [key]);
					}

					foreach (var ship in upgrade.Descendants ("Ship")) {
						string key = ship.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedShips.ContainsKey (key))
							ship.SetElementValue ("Id", deprecatedShips [key]);
					}
				}

				saveAndLoad.SaveText ("Upgrades_Custom.xml", upgradeXml.ToString ());
			}

			if (saveAndLoad.FileExists ("Ships_Custom.xml")) {
				var shipXml = XElement.Load (new StringReader (saveAndLoad.LoadText ("Ships_Custom.xml")));
				var shipElements = shipXml.Elements ();
				foreach (var ship in shipElements) {
					foreach (var faction in ship.Descendants ("Faction")) {
						string key = faction.Element ("Id")?.Value;

						if (string.IsNullOrEmpty (key))
							continue;

						if (deprecatedFactions.ContainsKey (key))
							faction.SetElementValue ("Id", deprecatedFactions [key]);
					}
				}

				saveAndLoad.SaveText ("Ships_Custom.xml", shipXml.ToString ());
			}
		}
	}
}

