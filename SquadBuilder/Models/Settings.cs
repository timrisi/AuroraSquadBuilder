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

		static XElement settingsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Settings.xml")));
		static Settings ()
		{
			allowCustom = (bool)settingsXml.Element ("AllowCustom");
			filterPilotsByShip = (bool)settingsXml.Element ("FilterPilotsByShip");
			updateOnLaunch = (bool)settingsXml.Element ("UpdateOnLaunch");
			hideUnavailable = (bool)settingsXml.Element ("HideUnavailable");
		}

		public static float FactionsVersion { get; set; }
		public static float ShipsVersion { get; set; }
		public static float PilotsVersion { get; set; }
		public static float UpgradesVersion { get; set; }
		public static float SettingsVersion { get; set; }
		public static float ExpansionsVersion { get; set; }

		static bool allowCustom;
		public static bool AllowCustom { 
			get { return allowCustom; }
			set {
				allowCustom = value;
				settingsXml.SetElementValue ("AllowCustom", value);
				DependencyService.Get <ISaveAndLoad> ().SaveText ("Settings.xml", settingsXml.ToString ());

			}
		}

		static bool filterPilotsByShip;
		public static bool FilterPilotsByShip { 
			get { return filterPilotsByShip; }
			set {
				filterPilotsByShip = value;
				settingsXml.SetElementValue ("FilterPilotsByShip", value);
				DependencyService.Get <ISaveAndLoad> ().SaveText ("Settings.xml", settingsXml.ToString ());
			}
		}

		static bool updateOnLaunch;
		public static bool UpdateOnLaunch {
			get { return updateOnLaunch; }
			set {
				updateOnLaunch = value;
				settingsXml.SetElementValue ("UpdateOnLaunch", value);
				DependencyService.Get <ISaveAndLoad> ().SaveText ("Settings.xml", settingsXml.ToString ());
			}
		}

		static bool hideUnavailable;
		public static bool HideUnavailable {
			get { return hideUnavailable; }
			set {
				hideUnavailable = value;
				settingsXml.SetElementValue ("HideUnavailable", value);
				DependencyService.Get <ISaveAndLoad> ().SaveText ("Settings.xml", settingsXml.ToString ());
			}
		}

		public static void CheckForUpdates ()
		{
			Task.Run (() => {
				Device.BeginInvokeOnMainThread (() => Application.Current.MainPage.IsBusy = true);


				try {
					var versionsXml = XElement.Load (xwingDataUrl + "Versions.xml");

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

					Cards.SharedInstance.SaveCollection ();
				} catch (Exception e) {		
					Insights.Report (e);
				}

				Device.BeginInvokeOnMainThread (() => Application.Current.MainPage.IsBusy = false);
			});
		}

		static void updateXml (string file)
		{
			var element = XElement.Load (xwingDataUrl + file + ".xml");
			DependencyService.Get <ISaveAndLoad> ().SaveText (file + ".xml", element.ToString ());
		}

		public static void UpdateShips ()
		{
			var element = XElement.Load (xwingDataUrl + "Ships.xml");

			var oldShips = Cards.SharedInstance.Ships;

			DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships.xml", element.ToString ());
			Cards.SharedInstance.GetAllShips ();

			foreach (var ship in oldShips) {
				if (Cards.SharedInstance.Ships.Any (s => s.Id == ship.Id))
					Cards.SharedInstance.Ships.First (s => s.Id == ship.Id).Owned = ship.Owned;
			}
		}

		public static void UpdatePilots ()
		{
			var element = XElement.Load (xwingDataUrl + "Pilots.xml");

			var oldPilots = Cards.SharedInstance.Pilots;

			DependencyService.Get <ISaveAndLoad> ().SaveText ("Pilots.xml", element.ToString ());
			Cards.SharedInstance.GetAllPilots ();

			foreach (var pilot in oldPilots) {
				if (Cards.SharedInstance.Pilots.Any (p => p.Id == pilot.Id))
					Cards.SharedInstance.Pilots.First (p => p.Id == pilot.Id).Owned = pilot.Owned;
			}
		}

		public static void UpdateUpgrades ()
		{
			var element = XElement.Load (xwingDataUrl + "Upgrades.xml");

			var oldUpgrades = Cards.SharedInstance.Upgrades;

			DependencyService.Get <ISaveAndLoad> ().SaveText ("Upgrades.xml", element.ToString ());
			Cards.SharedInstance.GetAllUpgrades ();

			foreach (var upgrade in oldUpgrades) {
				if (Cards.SharedInstance.Upgrades.Any (u => u.Id == upgrade.Id))
					Cards.SharedInstance.Upgrades.First (u => u.Id == upgrade.Id).Owned = upgrade.Owned;
			}
		}

		public static void UpdateExpansions ()
		{
			var element = XElement.Load (xwingDataUrl + "Expansions.xml");

			var oldExpansions = Cards.SharedInstance.Expansions;

			DependencyService.Get <ISaveAndLoad> ().SaveText ("Expansions.xml", element.ToString ());
			Cards.SharedInstance.GetAllExpansions ();

			foreach (var expansion in oldExpansions) {
				if (Cards.SharedInstance.Expansions.Any (e => e.Id == expansion.Id))
					Cards.SharedInstance.Expansions.First (e => e.Id == expansion.Id).Owned = expansion.Owned;
			}
		}
	}
}