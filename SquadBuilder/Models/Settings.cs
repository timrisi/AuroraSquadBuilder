using System;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms;
using XLabs.Data;
using System.Threading.Tasks;
using Xamarin;

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

					if ((float)versionsXml.Element ("Ships") > ShipsVersion) {
						updateXml ("Ships");
						Cards.SharedInstance.GetAllShips ();
					}

					if ((float)versionsXml.Element ("Pilots") > PilotsVersion) {
						updateXml ("Pilots");
						Cards.SharedInstance.GetAllPilots ();
					}

					if ((float)versionsXml.Element ("Upgrades") > UpgradesVersion) {
						updateXml ("Upgrades");
						Cards.SharedInstance.GetAllUpgrades ();
					}

					if ((versionsXml.Element ("Expansions") != null && (float)versionsXml.Element ("Expansions") > ExpansionsVersion)) {
						UpdateExpansions ();
						Cards.SharedInstance.GetAllExpansions ();
					}

					if ((float)versionsXml.Element ("Settings") > SettingsVersion)
						updateXml ("Settings");
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

		public static void UpdateExpansions ()
		{
			throw new NotImplementedException ();
		}
	}
}