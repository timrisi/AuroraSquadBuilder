using System;


using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using System.Linq;

namespace SquadBuilder
{
	public class EditShipViewModel : ViewModel
	{
		public bool Create { get; set; }
		Ship ship;
		public Ship Ship {
			get { 
				if (ship == null)
					ship = new Ship ();

				return ship;
			}
			set {
				SetProperty (ref ship, value);

				if (!Create) {
					XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));

					originalXml = customShipsXml.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Ship.Id);
				}
			}
		}

		XElement originalXml;
		public XElement OriginalXml {
			get { return originalXml; }
			set { SetProperty (ref originalXml, value); }
		}

		public string Name {
			get {
				return Ship.Name;
			}
			set {
				Ship.Name = value;

				char[] arr = Ship.Name.ToCharArray();

				arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
				Ship.Id = new string(arr).ToLower ();
				NotifyPropertyChanged ("Name");
			}
		}

		public bool LargeBase {
			get { return Ship.LargeBase; }
			set { 
				Ship.LargeBase = value;
				if (Ship.LargeBase && Ship.Huge)
					Ship.Huge = false;
				NotifyPropertyChanged ("Huge");
			}
		}

		public bool Huge {
			get { return Ship.Huge; }
			set {
				Ship.Huge = value;
				if (Ship.Huge && Ship.LargeBase)
					Ship.LargeBase = false;
				NotifyPropertyChanged ("LargeBase");
			}
		}

		public bool FocusAvailable {
			get { return Ship.Actions.Contains ("Focus"); }
			set {
				if (value && !Ship.Actions.Contains ("Focus"))
					Ship.Actions.Add ("Focus");

				if (!value && Ship.Actions.Contains ("Focus"))
					Ship.Actions.Remove ("Focus");
			}
		}

		public bool EvadeAvailable {
			get { return Ship.Actions.Contains ("Evade"); }
			set {
				if (value && !Ship.Actions.Contains ("Evade"))
					Ship.Actions.Add ("Evade");

				if (!value && Ship.Actions.Contains ("Evade"))
					Ship.Actions.Remove ("Evade");
			}
		}

		public bool TargetLockAvailable {
			get { return Ship.Actions.Contains ("Target Lock"); }
			set {
				if (value && !Ship.Actions.Contains ("Target Lock"))
					Ship.Actions.Add ("Target Lock");

				if (!value && Ship.Actions.Contains ("Target Lock"))
					Ship.Actions.Remove ("Target Lock");
			}
		}

		public bool BarrelRollAvailable {
			get { return Ship.Actions.Contains ("Barrel Roll"); }
			set {
				if (value && !Ship.Actions.Contains ("Barrel Roll"))
					Ship.Actions.Add ("Barrel Roll");

				if (!value && Ship.Actions.Contains ("Barrel Roll"))
					Ship.Actions.Remove ("Barrel Roll");
			}
		}

		public bool BoostAvailable {
			get { return Ship.Actions.Contains ("Boost"); }
			set {
				if (value && !Ship.Actions.Contains ("Boost"))
					Ship.Actions.Add ("Boost");

				if (!value && Ship.Actions.Contains ("Boost"))
					Ship.Actions.Remove ("Boost");
			}
		}

		public bool CloakAvailable {
			get { return Ship.Actions.Contains ("Cloak"); }
			set {
				if (value && !Ship.Actions.Contains ("Cloak"))
					Ship.Actions.Add ("Cloak");

				if (!value && Ship.Actions.Contains ("Cloak"))
					Ship.Actions.Remove ("Cloak");
			}
		}

		public bool SlamAvailable {
			get { return Ship.Actions.Contains ("SLAM"); }
			set {
				if (value && !Ship.Actions.Contains ("SLAM"))
					Ship.Actions.Add ("SLAM");

				if (!value && Ship.Actions.Contains ("SLAM"))
					Ship.Actions.Remove ("SLAM");
			}
		}

		Command saveShip;
		public Command SaveShip {
			get {
				if (saveShip == null)
					saveShip = new Command (() => {
						if (string.IsNullOrWhiteSpace (Name))
							return;

						XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));

						if (Ship.Ships.FirstOrDefault (e => e.Name == Name) != null)
							return;

						if (Create) {
							if (customShipsXml.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == Ship.Id) != null) {
								return;
							} else {
								var element = new XElement ("Ship");
								element.Add (new XAttribute ("id", Ship.Id));
								element.Add (new XElement ("Name", Name));
								element.Add (new XElement ("LargeBase", LargeBase));
								element.Add (new XElement ("Huge", Huge));
								element.Add (new XElement ("Custom", true));
								element.Add (new XElement ("Actions",
									from action in Ship.Actions
									select new XElement ("Action", action)));

								customShipsXml.Add (element);
							
								DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships_Custom.xml", customShipsXml.ToString ());

								MessagingCenter.Send <EditShipViewModel, Ship> (this, "Ship Created", Ship);
							}
						} else {
							var element = customShipsXml.Elements ().FirstOrDefault (e => e.Attribute ("id").Value == OriginalXml.Attribute ("id").Value);
							element.SetAttributeValue ("id", Ship.Id);
							element.SetElementValue ("Name", Ship.Name);
							element.Element ("LargeBase").SetValue (LargeBase);
							element.Element ("Huge").SetValue (Huge);

							element.Element ("Actions").Remove ();
							element.Add (new XElement ("Actions",
								from action in Ship.Actions
								select new XElement ("Action", action)
							));

							DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships_Custom.xml", customShipsXml.ToString ());
							MessagingCenter.Send <EditShipViewModel, Ship> (this, "Finished Editing", Ship);
						}
					});

				return saveShip;
			}
		}
	}
}