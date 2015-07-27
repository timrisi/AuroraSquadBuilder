using System;
using XLabs;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;

namespace SquadBuilder
{
	public class Faction
	{
		public Faction ()
		{
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public Color Color { get; set; }

		public RelayCommand DeleteFaction {
			get {
				return new RelayCommand (() => {
					XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));

					var factionElement = customFactionsXml.Descendants ().FirstOrDefault (e => e?.Value == Name);

					if (factionElement == null)
						return;

					factionElement.Remove ();

					DependencyService.Get <ISaveAndLoad> ().SaveText ("Factions_Custom.xml", customFactionsXml.ToString ());

					MessagingCenter.Send <Faction> (this, "Remove Faction");
				});
			}
		}
	}
}

