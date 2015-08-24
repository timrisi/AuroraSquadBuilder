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
			var factionsVersion = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Factions.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Factions.xml")))?.Attribute ("Version") < factionsVersion)
				saveAndLoad.SaveText ("Factions.xml", factionsXml);

			var customFactionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Factions_Custom.xml"))
				saveAndLoad.SaveText ("Factions_Custom.xml", customFactionsXml);

			var shipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships", "xml")).ReadToEnd ();
			var shipsVersion = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Ships.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Ships.xml")))?.Attribute ("Version") < shipsVersion)
				saveAndLoad.SaveText ("Ships.xml", shipsXml);

			var customShipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Ships_Custom.xml"))
				saveAndLoad.SaveText ("Ships_Custom.xml", customShipsXml);

			var pilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots", "xml")).ReadToEnd ();
			var pilotsVersion = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Pilots.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Pilots.xml")))?.Attribute ("Version") < pilotsVersion)
				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);

			var customPilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Pilots_Custom.xml"))
				saveAndLoad.SaveText ("Pilots_Custom.xml", customPilotsXml);

			var upgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades", "xml")).ReadToEnd ();
			var upgradesVersion = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Upgrades.xml") || (float)XElement.Load (new StringReader (saveAndLoad.LoadText ("Upgrades.xml")))?.Attribute ("Version") < upgradesVersion)
				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);

			var customUpgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades_Custom", "xml")).ReadToEnd ();
			if (!saveAndLoad.FileExists ("Upgrades_Custom.xml"))
				saveAndLoad.SaveText ("Upgrades_Custom.xml", customUpgradesXml);

			if (NSUserDefaults.StandardUserDefaults [HasMigrated] == null) {
				if (factionsVersion < 2.0 || shipsVersion < 2.0 || upgradesVersion < 2.0 || pilotsVersion < 2.0)
					UpdateIds ();
				else
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
		}
	}
}

