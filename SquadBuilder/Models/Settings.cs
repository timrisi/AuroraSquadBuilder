using System;
using System.IO;
using System.Xml.Linq;
using Xamarin.Forms;
using XLabs.Data;

namespace SquadBuilder
{
	public static class Settings
	{
		static XElement settingsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Settings.xml")));
		static Settings ()
		{
			allowCustom = (bool)settingsXml.Element ("AllowCustom");
			filterPilotsByShip = (bool)settingsXml.Element ("FilterPilotsByShip");
		}

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
	}
}