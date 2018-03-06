using System;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Xml.Serialization;
using System.Security.Policy;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class Faction
	{
		public const string FactionsFilename = "Factions.xml";

		public Faction ()
		{
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public Color Color { get; set; }
		public string CanonicalName { get; set; }
		public string OldId { get; set; }

		[XmlIgnore]
		Command deleteFaction;
		[XmlIgnore]
		public Command DeleteFaction {
			get {
				if (deleteFaction == null)
					deleteFaction = new Command (() => {
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

		static ObservableCollection<Faction> factions;
		public static ObservableCollection<Faction> Factions {
			get {
				if (factions == null)
					GetAllFactions ();

				return factions;
			}
			set {
				factions = value;
				factions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		static ObservableCollection<Faction> customFactions;
		public static ObservableCollection<Faction> CustomFactions {
			get {
				if (customFactions == null)
					GetAllFactions ();

				return customFactions;
			}
			set {
				customFactions = value;
				customFactions.CollectionChanged += (sender, e) => updateAllFactions ();
				updateAllFactions ();
			}
		}

		static ObservableCollection<Faction> allFactions;
		public static ObservableCollection<Faction> AllFactions {
			get {
				if (allFactions == null)
					updateAllFactions ();

				return allFactions;
			}
		}

		static void updateAllFactions ()
		{
			var temp = Factions.ToList ();
			temp.AddRange (customFactions);
			allFactions = new ObservableCollection<Faction> (temp);
		}

		public static void GetAllFactions ()
		{
			if (!DependencyService.Get<ISaveAndLoad> ().FileExists (Faction.FactionsFilename))
				return;

			XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Faction.FactionsFilename)));
			factions = new ObservableCollection<Faction> ((from faction in factionsXml.Elements ()
								       select new Faction {
									       Id = faction.Attribute ("id").Value,
									       Name = faction.Value,
									       CanonicalName = faction.Element ("CanonicalName")?.Value,
									       OldId = faction.Element ("OldId")?.Value,
									       Color = Color.FromRgb (
										       (int) faction.Element ("Color").Attribute ("r"),
										       (int) faction.Element ("Color").Attribute ("g"),
										       (int) faction.Element ("Color").Attribute ("b")
									       )
								       })
			);

			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			customFactions = new ObservableCollection<Faction> ((from faction in customFactionsXml.Elements ()
									     select new Faction {
										     Id = faction.Attribute ("id").Value,
										     Name = faction.Value,
										     CanonicalName = faction.Element ("CanonicalName")?.Value,
										     OldId = faction.Element ("OldId")?.Value,
										     Color = Color.FromRgb (
											     (int) faction.Element ("Color").Attribute ("r"),
											     (int) faction.Element ("Color").Attribute ("g"),
											     (int) faction.Element ("Color").Attribute ("b")
										     )
									     })
			);

			updateAllFactions ();
		}
	}
}

