using System;
using XLabs.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using XLabs;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;

namespace SquadBuilder
{
	public class Expansion : ObservableObject
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Wave { get; set; }
		public List <string> Ships { get; set; }
		public List <string> Pilots { get; set; }
		public List <string> Upgrades { get; set; }

		public int owned;
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
					Cards.SharedInstance.Ships.FirstOrDefault (s => s.Id == ship).Owned += (owned - previousNumber);
				
				foreach (var pilot in Pilots)
					Cards.SharedInstance.Pilots.FirstOrDefault (p => p.Id == pilot).Owned += (owned - previousNumber);
			
				foreach (var upgrade in Upgrades)
					Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.Id == upgrade).Owned += (owned - previousNumber);
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
	}
}