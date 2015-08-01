using System;
using XLabs.Forms.Mvvm;
using XLabs;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using System.Linq;

namespace SquadBuilder
{
	public class CreateShipViewModel : ViewModel
	{
		string name;
		public string Name {
			get {
				return name;
			}
			set {
				SetProperty (ref name, value);
			}
		}
			
		bool largeBase;
		public bool LargeBase {
			get { return largeBase; }
			set { 
				SetProperty (ref largeBase, value);
				if (largeBase && Huge)
					Huge = false;
			}
		}

		bool huge;
		public bool Huge {
			get { return huge; }
			set {
				SetProperty (ref huge, value);
				if (huge && LargeBase)
					LargeBase = false;
			}
		}

		bool focusAvailable;
		public bool FocusAvailable {
			get { return focusAvailable; }
			set {
				SetProperty (ref focusAvailable, value);
			}
		}

		bool evadeAvailable;
		public bool EvadeAvailable {
			get { return evadeAvailable; }
			set {
				SetProperty (ref evadeAvailable, value);
			}
		}

		bool targetLockAvailable;
		public bool TargetLockAvailable {
			get { return targetLockAvailable; }
			set {
				SetProperty (ref targetLockAvailable, value);
			}
		}

		bool barrelRollAvailable;
		public bool BarrelRollAvailable {
			get { return barrelRollAvailable; }
			set {
				SetProperty (ref barrelRollAvailable, value);
			}
		}

		bool boostAvailable;
		public bool BoostAvailable {
			get { return boostAvailable; }
			set {
				SetProperty (ref boostAvailable, value);
			}
		}

		bool cloakAvailable;
		public bool CloakAvailable {
			get { return cloakAvailable; }
			set {
				SetProperty (ref cloakAvailable, value);
			}
		}

		bool slamAvailable;
		public bool SlamAvailable {
			get { return slamAvailable; }
			set {
				SetProperty (ref slamAvailable, value);
			}
		}

		RelayCommand saveShip;
		public RelayCommand SaveShip {
			get {
				if (saveShip == null)
					saveShip = new RelayCommand (() => {
						if (string.IsNullOrWhiteSpace (name))
							return;
						
						XElement shipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships.xml")));
						XElement customShipsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Ships_Custom.xml")));

						if (shipsXml.Descendants ().FirstOrDefault (e => e?.Value == name) != null)
							return;

						if (customShipsXml.Descendants ().FirstOrDefault (e => e?.Value == name) != null)
							return;
						
						char[] arr = name.ToCharArray();

						arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
						var str = new string(arr);

						var actions = new List <string> ();

						if (FocusAvailable) 
							actions.Add ("Focus");
						if (TargetLockAvailable) 
							actions.Add ("Target Lock");
						if (EvadeAvailable) 
							actions.Add ("Evade");
						if (BarrelRollAvailable)
							actions.Add ("Barrel Roll");
						if (BoostAvailable)
							actions.Add ("Boost");
						if (CloakAvailable)
							actions.Add ("Cloak");
						if (SlamAvailable)
							actions.Add ("SLAM");

						var element = new XElement ("Ship");
						element.Add (new XAttribute ("id", str.ToLower ()));
						element.Add (new XElement ("Name", name));
						if (LargeBase)
							element.Add (new XElement ("LargeBase", true));
						if (Huge)
							element.Add (new XElement ("Huge", true));
						element.Add (new XElement ("Actions",
							from action in actions
							select new XElement ("Action", action)));

						customShipsXml.Add (element);

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Ships_Custom.xml", customShipsXml.ToString ());

						MessagingCenter.Send <CreateShipViewModel, Ship> (this, "Ship Created", 
							new Ship { 
								Id = str.ToLower (), 
								Name = Name, 
								LargeBase = LargeBase, 
								Huge = Huge,
								Actions = actions
							}
						);
					});

				return saveShip;
			}
		}
	}
}