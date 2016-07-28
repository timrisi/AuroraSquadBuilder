using System;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms;
using XLabs.Data;
using System.Threading.Tasks;
using Xamarin;
using System.Linq;

namespace SquadBuilder
{
	public static class Settings
	{
		const string xwingDataUrl = "http://www.risiapps.com/xwing_data/";

		static XElement settingsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Cards.SettingsFilename)));
		static Settings ()
		{
			allowCustom = (bool)settingsXml.Element ("AllowCustom");
			filterPilotsByShip = (bool)settingsXml.Element ("FilterPilotsByShip");
			updateOnLaunch = (bool)settingsXml.Element ("UpdateOnLaunch");
			hideUnavailable = (bool)settingsXml.Element ("HideUnavailable");
			dropboxSync = (bool)settingsXml.Element ("DropboxSync");
		}

		public static float FactionsVersion { get; set; }
		public static float ShipsVersion { get; set; }
		public static float PilotsVersion { get; set; }
		public static float UpgradesVersion { get; set; }
		public static float SettingsVersion { get; set; }
		public static float ExpansionsVersion { get; set; }
		public static float ReferenceCardsVersion { get; set; }

		static bool allowCustom;
		public static bool AllowCustom {
			get { return allowCustom; }
			set {
				allowCustom = value;
				settingsXml.SetElementValue ("AllowCustom", value);
				DependencyService.Get<ISaveAndLoad> ().SaveText (Cards.SettingsFilename, settingsXml.ToString ());

			}
		}

		static bool filterPilotsByShip;
		public static bool FilterPilotsByShip {
			get { return filterPilotsByShip; }
			set {
				filterPilotsByShip = value;
				settingsXml.SetElementValue ("FilterPilotsByShip", value);
				DependencyService.Get<ISaveAndLoad> ().SaveText (Cards.SettingsFilename, settingsXml.ToString ());
			}
		}

		static bool updateOnLaunch;
		public static bool UpdateOnLaunch {
			get { return updateOnLaunch; }
			set {
				updateOnLaunch = value;
				settingsXml.SetElementValue ("UpdateOnLaunch", value);
				DependencyService.Get<ISaveAndLoad> ().SaveText (Cards.SettingsFilename, settingsXml.ToString ());
			}
		}

		static bool hideUnavailable;
		public static bool HideUnavailable {
			get { return hideUnavailable; }
			set {
				hideUnavailable = value;
				settingsXml.SetElementValue ("HideUnavailable", value);
				DependencyService.Get<ISaveAndLoad> ().SaveText (Cards.SettingsFilename, settingsXml.ToString ());
			}
		}

		static bool dropboxSync;
		public static bool DropboxSync {
			get { return dropboxSync; }
			set {
				dropboxSync = value;
				settingsXml.SetElementValue ("DropboxSync", value);
				DependencyService.Get<ISaveAndLoad> ().SaveText (Cards.SettingsFilename, settingsXml.ToString ());
			}
		}

		static bool showManeuversInShipList = true;
		public static bool ShowManeuversInShipList {
			get { return showManeuversInShipList; }
			set {
				showManeuversInShipList = value;
				App.Storage.Put<bool> ("ShowManeuversInShipList", value);
			}
		}

		static bool showManeuversInSquadronList = false;
		public static bool ShowManeuversInSquadronList {
			get { return showManeuversInSquadronList; }
			set {
				showManeuversInSquadronList = value;
				App.Storage.Put<bool> ("ShowManeuversInSquadronList", value);
			}
		}

		static bool showManeuversInPilotView = true;
		public static bool ShowManeuversInPilotView {
			get { return showManeuversInPilotView; }
			set {
				showManeuversInPilotView = value;
				App.Storage.Put<bool> ("ShowManeuversInPilotView", value);
			}
		}

		static bool showManeuversInPilotSelection = false;
		public static bool ShowManeuversInPilotSelection {
			get { return showManeuversInPilotSelection; }
			set {
				showManeuversInPilotSelection = value;
				App.Storage.Put<bool> ("ShowManeuversInPilotSelection", value);
			}
		}

		static bool customCardLeague = false;
		public static bool CustomCardLeague {
			get { return customCardLeague; }
			set {
				customCardLeague = value;
				App.Storage.Put<bool> ("CustomCardLeague", value);
			}
		}

		public static void CheckForUpdates ()
		{
			Task.Run (() => {
				Device.BeginInvokeOnMainThread (() => Application.Current.MainPage.IsBusy = true);


				try {
					var versionsXml = XElement.Load (xwingDataUrl + Cards.VersionsFilename);

					if ((float)versionsXml.Element ("Factions") > FactionsVersion) {
						updateXml ("Factions");
						Cards.SharedInstance.GetAllFactions ();
					}

					if ((float)versionsXml.Element ("Ships") > ShipsVersion)
						UpdateShips ();

					if ((float)versionsXml.Element ("Pilots") > PilotsVersion)
						UpdatePilots ();

					if ((float)versionsXml.Element ("Upgrades") > UpgradesVersion)
						UpdateUpgrades ();

					if ((versionsXml.Element ("Expansions") != null && (float)versionsXml.Element ("Expansions") > ExpansionsVersion))
						UpdateExpansions ();

					if ((float)versionsXml.Element ("Settings") > SettingsVersion)
						updateXml ("Settings");

					if ((float)versionsXml.Element ("ReferenceCards") > ReferenceCardsVersion)
						updateXml ("ReferenceCardsVersion");
					
				} catch (Exception e) {
				}

				Device.BeginInvokeOnMainThread (() => Application.Current.MainPage.IsBusy = false);
			});
		}

		static void updateXml (string file)
		{
			if (file == "Factions")
				file = file + "3";
			
			var element = XElement.Load (xwingDataUrl + file + ".xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (file + ".xml", element.ToString ());
		}

		public static void UpdateShips ()
		{
			var element = XElement.Load (xwingDataUrl + "Ships3.xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.ShipsFilename, element.ToString ());
			Cards.SharedInstance.GetAllShips ();
		}

		public static void UpdatePilots ()
		{
			var element = XElement.Load (xwingDataUrl + "Pilots3.xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.PilotsFilename, element.ToString ());
			Cards.SharedInstance.GetAllPilots ();
		}

		public static void UpdateUpgrades ()
		{
			var element = XElement.Load (xwingDataUrl + "Upgrades3.xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.UpgradesFilename, element.ToString ());
			Cards.SharedInstance.GetAllUpgrades ();
		}

		public static void UpdateExpansions ()
		{
			var element = XElement.Load (xwingDataUrl + "Expansions3.xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.ExpansionsFilename, element.ToString ());
			Cards.SharedInstance.GetAllExpansions ();
		}
	}
}