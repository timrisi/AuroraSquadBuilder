using System;
using System.Collections.Generic;
using XLabs;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using System.Xml.Serialization;
using XLabs.Data;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class Ship : ObservableObject
	{
		string id;
		public String Id {
			get { return id; }
			set {
				SetProperty (ref id, value);
			}
		}

		string name;
		public string Name { 
			get { return name; }
			set {
				SetProperty (ref name, value);
			}
		}

		bool largeBase;
		public bool LargeBase { 
			get { return largeBase; }
			set { 
				SetProperty (ref largeBase, value);
			}
		}

		bool huge;
		public bool Huge { 
			get { return huge; } 
			set {
				SetProperty (ref huge, value);
			}
		}

		ObservableCollection <string> actions = new ObservableCollection <string> ();
		public ObservableCollection <string> Actions { 
			get { return actions; }
			set { 
				SetProperty (ref actions, value);
				actions.CollectionChanged += (sender, e) => NotifyPropertyChanged ("ActionsString");
			}
		}

		public string ActionsString { 
			get {
				return string.Join (", ", Actions ?? new ObservableCollection <string> ());
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

						var shipElement = customShipsXml.Descendants ().FirstOrDefault (e => e.Element ("Name")?.Value == Name);

						if (shipElement == null)
							return;

						shipElement.Remove ();

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships_Custom.xml", customShipsXml.ToString ());

						MessagingCenter.Send <Ship> (this, "Remove Ship");
					});

				return deleteShip;
			}
		}

		[XmlIgnore]
		RelayCommand editShip;
		[XmlIgnore]
		public RelayCommand EditShip {
			get {
				if (editShip == null)
					editShip = new RelayCommand (() => {
						MessagingCenter.Send <Ship> (this, "Edit Ship");
					});

				return editShip;
			}
		}

		public Ship Copy ()
		{
			return new Ship {
				Id = Id,
				Name = Name,
				LargeBase = LargeBase,
				Huge = Huge,
				Actions = new ObservableCollection <string> (Actions)
			};
		}
	}
}

