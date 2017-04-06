using System;
using XLabs;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Xml.Serialization;
using System.Security.Policy;

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
		public string CanonicalName { get; set; }
		public string OldId { get; set; }

		[XmlIgnore]
		RelayCommand deleteFaction;
		[XmlIgnore]
		public RelayCommand DeleteFaction {
			get {
				if (deleteFaction == null)
					deleteFaction = new RelayCommand (() => {
						XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));

						var factionElement = customFactionsXml.Descendants ().FirstOrDefault (e => e?.Value == Name);

						if (factionElement == null)
							return;

						factionElement.Remove ();

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Factions_Custom.xml", customFactionsXml.ToString ());

						MessagingCenter.Send <Faction> (this, "Remove Faction");
					});

				return deleteFaction;
			}
		}

		public override bool Equals (object obj)
		{
			if (!(obj is Faction))
				return false;

			var faction = obj as Faction;
			return Id == faction.Id &&
				Name == faction.Name &&
				Color.ToString () == faction.Color.ToString ();
		}

		public override int GetHashCode ()
		{
			return (Id + Name + Color.GetHashCode ()).GetHashCode ();
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

