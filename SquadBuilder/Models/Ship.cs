﻿using System;
using System.Collections.Generic;

using System.Xml.Linq;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using System.Xml.Serialization;

using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class Ship : ObservableObject {
		public const string ShipsFilename = "Ships.xml";

		string id;
		public string Id {
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

		string canonicalName;
		public string CanonicalName {
			get { return canonicalName ?? id; }
			set {
				SetProperty (ref canonicalName, value);
			}
		}

		string oldId;
		public string OldId {
			get { return oldId; }
			set { SetProperty (ref oldId, value); }
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

		bool isPreview;
		public bool IsPreview {
			get { return isPreview; }
			set { SetProperty (ref isPreview, value); }
		}

		bool isCustom;
		public bool IsCustom {
			get { return isCustom; }
			set { SetProperty (ref isCustom, value); }
		}

		bool ccl;
		public bool CCL {
			get { return ccl; }
			set { SetProperty (ref ccl, value); }
		}

		string maneuverGridImage;
		public string ManeuverGridImage {
			get {
				return maneuverGridImage;
			}
			set {
				SetProperty (ref maneuverGridImage, value);
			}
		}

		string symbol;
		public string Symbol {
			get { return symbol; }
			set { SetProperty (ref symbol, value);}
		}

		string attackSymbol;
		public string AttackSymbol {
			get { return attackSymbol; }
			set { SetProperty (ref attackSymbol, value); }
		}

#region Maneuvers
		//public string straightOne;
		//public string StraightOne {
		//	get { return straightOne; }
		//	set {
		//		if (!string.IsNullOrEmpty(value))
		//			straightOne = $"<font color='{value}' face='xwing-miniatures'>8</font>";
		//		else
		//			straightOne = "";
		//	}
		//}

		//public string straightTwo;
		//public string StraightTwo
		//{
		//	get { return straightTwo; }
		//	set
		//	{
		//		if (!string.IsNullOrEmpty(value))
		//			straightTwo = $"<font color='{value}' face='xwing-miniatures'>8</font>";
		//		else
		//			straightTwo = "";
		//	}
		//}

		//public string straightThree;
		//public string StraightThree
		//{
		//	get { return straightThree; }
		//	set
		//	{
		//		if (!string.IsNullOrEmpty(value))
		//			straightThree = $"<font color='{value}' face='xwing-miniatures'>8</font>";
		//		else
		//			straightThree = "";
		//	}
		//}

		//public string straightFour;
		//public string StraightFour
		//{
		//	get { return straightOne; }
		//	set
		//	{
		//		if (!string.IsNullOrEmpty(value))
		//			straightFour = $"<font color='{value}' face='xwing-miniatures'>8</font>";
		//		else
		//			straightFour = "";
		//	}
		//}

		//public string straightFive;
		//public string StraightFive
		//{
		//	get { return straightFive; }
		//	set
		//	{
		//		if (!string.IsNullOrEmpty(value))
		//			straightFive = $"<font color='{value}' face='xwing-miniatures'>8</font>";
		//		else
		//			straightFive = "";
		//	}
		//}
#endregion

		[XmlIgnore]
		public int owned;
		[XmlIgnore]
		public int Owned {
			get { return owned; }
			set {
				if (value < 0)
					value = 0;
				
				SetProperty (ref owned, value); 

				var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
				var shipsElement = collectionXml.Element ("Ships");

				var shipsOwned = Ship.Ships.FirstOrDefault (s => s.Id == Id).Owned;

				if (shipsElement.Elements ().Any (e => e.Attribute ("id").Value == Id)) {
					if (shipsOwned == 0)
						shipsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id)?.Remove ();
					else
						shipsElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Id).SetValue (shipsOwned);
				} else {
					var element = new XElement ("Ship", shipsOwned);
					element.SetAttributeValue ("id", Id);
					shipsElement.Add (element);
				}

				DependencyService.Get<ISaveAndLoad> ().SaveText ("Collection.xml", collectionXml.ToString ());
			}
		}

		ObservableCollection <string> actions = new ObservableCollection <string> ();
		public ObservableCollection <string> Actions { 
			get { return actions; }
			set { 
				SetProperty (ref actions, value);
				updateActions ();
				NotifyPropertyChanged ("ActionsString");
				actions.CollectionChanged += (sender, e) => {
					updateActions ();
					NotifyPropertyChanged ("ActionsString");
				};
				Actions.CollectionChanged += (sender, e) => {
					updateActions ();
					NotifyPropertyChanged ("ActionsString");
				};
			}
		}

		Dictionary<string, string> ActionsDictionary = new Dictionary<string, string> {
			{ "Focus", "f" },
			{ "Target Lock", "l" },
			{ "Evade", "e" },
			{ "Barrel Roll", "r" },
			{ "Boost", "b" },
			{ "SLAM", "s" },
			{ "Reinforce", "i" },
			{ "Coordinate", "o" },
			{ "Jam", "j" },
			{ "Cloak", "k" },
			{ "Recover", "v" },
			{ "Rotate Arc", "R" },
			{ "Reload", "" }
		};

		void updateActions ()
		{
			var actionsString = ActionsDictionary [Actions [0]];
			for (int i = 1; i < Actions.Count; i++)
				actionsString += " " + ActionsDictionary [Actions [i]];

			this.actionsString = actionsString;
		}

		string actionsString;
		public string ActionsString { 
			get {
				return actionsString;
			} set {
				SetProperty (ref actionsString, value);
			}
		}

		public Color TextColor {
			get { return IsAvailable ? Color.Black : Color.Gray; }
		}

		public bool IsAvailable {
			get { 
				if (Ship.Ships.Sum (s => s.Owned) == 0)
					return true;
				
				return Owned > (Squadron.CurrentSquadron != null ? Squadron.CurrentSquadron.Pilots.Count (p => p.Ship.Id == Id) : 0); 
			}
		}

		public bool ShowManeuvers {
			get { return Settings.ShowManeuversInShipList && !string.IsNullOrEmpty (ManeuverGridImage); }
		}

		[XmlIgnore]
		Command deleteShip;
		[XmlIgnore]
		public Command DeleteShip {
			get {
				if (deleteShip == null)
					deleteShip = new Command (() => {
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
		Command editShip;
		[XmlIgnore]
		public Command EditShip {
			get {
				if (editShip == null)
					editShip = new Command (() => {
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
				CanonicalName = CanonicalName,
				LargeBase = LargeBase,
				Huge = Huge,
				Actions = new ObservableCollection<string> (Actions),
				ManeuverGridImage = ManeuverGridImage,
				IsCustom = IsCustom,
				CCL = CCL,
				OldId = OldId,
				IsPreview = IsPreview,
				Symbol = Symbol,
				AttackSymbol = AttackSymbol,

				//straightOne = StraightOne,
				//straightTwo = StraightTwo,
				//straightThree = StraightThree,
				//straightFour = StraightFour,
				//straightFive = StraightFive,
			};
		}

		Command increment;
		public Command Increment {
			get {
				if (increment == null)
					increment = new Command (() => Owned++);

				return increment;
			}
		}

		Command decrement;
		public Command Decrement {
			get {
				if (decrement == null)
					decrement = new Command (() => Owned--);

				return decrement;
			}
		}

		public override bool Equals (object obj)
		{
			if (!(obj is Ship))
				return false;

			return Id == (obj as Ship).Id;
		}

		public override int GetHashCode ()
		{
			return Id.GetHashCode ();
		}

		public override string ToString ()
		{
			return Name;
		}

#region Static Methods
		static ObservableCollection<Ship> ships;
		public static ObservableCollection<Ship> Ships {
			get {
				if (ships == null)
					GetAllShips ();

				return ships;
			}
			set {
				ships = value;
				ships.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		static ObservableCollection<Ship> customShips;
		public static ObservableCollection<Ship> CustomShips {
			get {
				if (customShips == null)
					GetAllShips ();

				return customShips;
			}
			set {
				customShips = value;
				customShips.CollectionChanged += (sender, e) => updateAllShips ();
				updateAllShips ();
			}
		}

		static ObservableCollection<Ship> allShips;
		public static ObservableCollection<Ship> AllShips {
			get {
				if (allShips == null)
					updateAllShips ();

				return allShips;
			}
		}

		static void updateAllShips ()
		{
			var temp = Ships.ToList ();
			temp.AddRange (customShips);
			allShips = new ObservableCollection<Ship> (temp);
		}

		public static void GetAllShips ()
		{
			if (!DependencyService.Get<ISaveAndLoad> ().FileExists (Ship.ShipsFilename))
				return;

			var collectionXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Collection.xml")));
			var shipsCollectionElement = collectionXml.Element ("Ships");

			XElement shipsElement = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText (Ship.ShipsFilename)));
			ships = new ObservableCollection<Ship> ((
				from ship in shipsElement.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					OldId = ship.Element ("OldId")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool) ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool) ship.Element ("Huge") : false,
					Actions = new ObservableCollection<string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					IsCustom = ship.Element ("Custom") != null ? (bool) ship.Element ("Custom") : false,
					CCL = ship.Element ("CCL") != null ? (bool) ship.Element ("CCL") : false,
					IsPreview = ship.Element ("Preview") != null ? (bool) ship.Element ("Preview") : false,
					ManeuverGridImage = ship.Element ("ManeuverGridImage")?.Value ?? "",
					Symbol = ship.Element ("Symbol")?.Value ?? "",
					AttackSymbol = ship.Element ("AttackSymbol")?.Value,
					//StraightOne = ship.Element("StraightOne")?.Value ?? "",
					//StraightTwo = ship.Element("StraightOne")?.Value ?? "",
					//StraightThree = ship.Element("StraightOne")?.Value ?? "",
					//StraightFour = ship.Element("StraightOne")?.Value ?? "",
					//StraightFive = ship.Element("StraightOne")?.Value ?? "",
					owned = shipsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == ship.Attribute ("id").Value) != null ?
							(int) shipsCollectionElement.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == ship.Attribute ("id").Value) : 0
				}).OrderBy (s => s.Name).OrderBy (s => s.LargeBase).OrderBy (s => s.Huge)
			);

			XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get<ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));
			customShips = new ObservableCollection<Ship> ((
				from ship in customShipsXml.Elements ()
				select new Ship {
					Id = ship.Attribute ("id").Value,
					Name = ship.Element ("Name").Value,
					CanonicalName = ship.Element ("CanonicalName")?.Value,
					OldId = ship.Element ("OldId")?.Value,
					LargeBase = ship.Element ("LargeBase") != null ? (bool) ship.Element ("LargeBase") : false,
					Huge = ship.Element ("Huge") != null ? (bool) ship.Element ("Huge") : false,
					Actions = new ObservableCollection<string> (
						from action in ship.Element ("Actions").Elements ()
						select action.Value),
					IsPreview = ship.Element ("Preview") != null ? (bool) ship.Element ("Preview") : false,
					IsCustom = ship.Element ("Custom") != null ? (bool) ship.Element ("Custom") : false,
					CCL = false,
					ManeuverGridImage = "",
					owned = 0
				})
			);

			updateAllShips ();
		}
		#endregion
	}
}

