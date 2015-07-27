﻿using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XLabs;
using System.Linq;
using XLabs.Data;

namespace SquadBuilder
{
	public class Squadron : ObservableObject
	{
		public Squadron ()
		{
			Name = "";
			Pilots = new ObservableCollection <Pilot> ();
			Pilots.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Points");
		}

		string name;
		public string Name {
			get { return name; }
			set { SetProperty (ref name, value); }
		}

		Faction faction;
		public Faction Faction { 
			get { return faction; }
			set { SetProperty (ref faction, value); }
		}

		public int Points { 
			get {
				return Pilots.Sum (p => p.Cost);
			} 
		}

		int maxPoints;
		public int MaxPoints { 
			get { return maxPoints; }
			set { SetProperty (ref maxPoints, value); }
		}

		ObservableCollection <Pilot> pilots = new ObservableCollection <Pilot> ();
		public ObservableCollection <Pilot> Pilots { 
			get { 
				return pilots;
			}
			set {
				SetProperty (ref pilots, value);
			}
		}
			
		RelayCommand deleteSquadron;
		public RelayCommand DeleteSquadron {
			get {
				if (deleteSquadron == null)
					deleteSquadron = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "DeleteSquadron"));

				return deleteSquadron;
			}
		}

		RelayCommand editDetails;
		public RelayCommand EditDetails {
			get {
				if (editDetails == null)
					editDetails = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "EditDetails"));
			
				return editDetails;
			}
		}

		RelayCommand copySquadron;
		public RelayCommand CopySquadron {
			get {
				if (copySquadron == null)
					copySquadron = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "CopySquadron"));

				return copySquadron;
			}
		}

//		public Color FactionColor {
//			get {
//				switch (Faction) {
//				case "Scum & Villainy":
//					return Color.FromHex ("#CC9900");
//				case "Rebel":
//					return Color.Red;
//				case "Imperial":
//					return Color.Green;
//				default:
//					return Device.OnPlatform <Color> (Color.Navy, Color.Teal, Color.Navy);
//				}
//
//			}
//		}

		public Squadron Copy ()
		{
			return new Squadron {
				Name = Name,
				Faction = Faction,
				MaxPoints = MaxPoints,
				Pilots = new ObservableCollection<Pilot> (Pilots),
			};
		}
	}
}


