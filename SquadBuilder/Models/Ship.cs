using System;
using System.Collections.Generic;
using XLabs;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using System.Xml.Serialization;

namespace SquadBuilder
{
	public class Ship
	{
		public String Id { get; set; }
		public string Name { get; set; }
		public bool LargeBase { get; set; }
		public bool Huge { get; set; }
		public List <string> Actions { get; set; }
		public string ActionsString { 
			get {
				return string.Join (", ", Actions ?? new List <string> ());
			}
		}

		[XmlIgnore]
		RelayCommand deleteShip;
		[XmlIgnore]
		public RelayCommand DeleteShip {
			get {
				if (deleteShip == null)
					deleteShip = new RelayCommand (() => {
						XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));

						var shipElement = customShipsXml.Descendants ().FirstOrDefault (e => e.Element ("Name").Value == Name);

						if (shipElement == null)
							return;

						shipElement.Remove ();

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships_Custom.xml", customShipsXml.ToString ());

						MessagingCenter.Send <Ship> (this, "Remove Ship");
					});

				return deleteShip;
			}
		}
	}
}

