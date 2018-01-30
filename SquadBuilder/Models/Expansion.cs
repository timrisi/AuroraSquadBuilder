using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using XLabs;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace SquadBuilder
{
	public class Expansion : ObservableObject {
		public const string ExpansionsFilename = "Expansions.xml";

		public string Id { get; set; }
		public string Name { get; set; }
		public string Wave { get; set; }
		public List <string> Ships { get; set; }
		public List <string> Pilots { get; set; }
		public List <string> Upgrades { get; set; }

		[XmlIgnore]
		public int owned;
		[XmlIgnore]
		public int Owned { 
			get { return owned; }
			set {
				if (value < 0)
					value = 0;
				
				if (value == owned)
					return;
				
				var previousNumber = owned;
				SetProperty (ref owned, value);

				var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
				var expansionsElement = collectionXml.Element ("Expansions");
				if (expansionsElement.Elements ().Any (e => e.Attribute ("id").Value == Id)) {
					if (owned == 0)
						expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id)?.Remove ();
					else 
						expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id).SetValue (owned);
				} else {
					var element = new XElement ("Expansion", owned);
					element.SetAttributeValue ("id", Id);
					expansionsElement.Add (element);
				}

				DependencyService.Get<ISaveAndLoad> ().SaveText ("Collection.xml", collectionXml.ToString ());

				foreach (var ship in Ships)
					Ship.Ships.FirstOrDefault (s => s.Id == ship).Owned += (owned - previousNumber);
				
				foreach (var pilot in Pilots)
					(Pilot.Pilots.FirstOrDefault (p => p.Id == pilot && Ships.Contains (p.Ship.Id)) ??
					 Pilot.Pilots.FirstOrDefault (p => p.Id == pilot)).Owned += (owned - previousNumber);
			
				foreach (var upgrade in Upgrades)
					Upgrade.Upgrades.FirstOrDefault (u => u.Id == upgrade).Owned += (owned - previousNumber);
			}
		}

		RelayCommand increment;
		public RelayCommand Increment {
			get {
				if (increment == null)
					increment = new RelayCommand (() => Owned++);

				return increment;
			}
		}

		RelayCommand decrement;
		public RelayCommand Decrement {
			get {
				if (decrement == null)
					decrement = new RelayCommand (() => Owned--);

				return decrement;
			}
		}

		#region Static Methods
		static ObservableCollection<Expansion> expansions;
		public static ObservableCollection<Expansion> Expansions {
			get {
				if (expansions == null)
					GetAllExpansions ();

				return expansions;
			}
			set { expansions = value; }
		}

		public static void GetAllExpansions ()
		{
			if (!DependencyService.Get<ISaveAndLoad> ().FileExists (Expansion.ExpansionsFilename))
				return;
			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var expansionsElement = collectionXml.Element ("Expansions");

			XElement expansionsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Expansion.ExpansionsFilename)));
			expansions = new ObservableCollection<Expansion> (from expansion in expansionsXml.Elements ()
									  select new Expansion {
										  Id = expansion.Attribute ("id").Value,
										  Name = expansion.Element ("Name").Value,
										  Wave = expansion.Element ("Wave").Value,
										  Ships = (from ship in expansion.Element ("Ships").Elements ()
											   select ship.Value).ToList (),
										  Pilots = (from pilot in expansion.Element ("Pilots").Elements ()
											    select pilot.Value).ToList (),
										  Upgrades = (from upgrade in expansion.Element ("Upgrades").Elements ()
											      select upgrade.Value).ToList (),
										  owned = expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == expansion.Attribute ("id").Value) != null ?
							   (int) expansionsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == expansion.Attribute ("id").Value) : 0
									  });
		}
		#endregion
	}
}