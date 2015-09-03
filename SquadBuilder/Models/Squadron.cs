using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XLabs;
using System.Linq;
using XLabs.Data;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Android.Runtime;
using Org.Json;

namespace SquadBuilder
{
	public class Squadron : ObservableObject
	{
		public Squadron ()
		{
			Name = "";
			Pilots = new ObservableCollection <Pilot> ();
			Pilots.CollectionChanged += (sender, e) => {
				NotifyPropertyChanged ("Points");
				NotifyPropertyChanged ("PointDetails");
			};
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
			set { 
				SetProperty (ref maxPoints, value);
				NotifyPropertyChanged ("PointDetails");
			}
		}

		ObservableCollection <Pilot> pilots = new ObservableCollection <Pilot> ();
		public ObservableCollection <Pilot> Pilots { 
			get { 
				return pilots;
			}
			set {
				SetProperty (ref pilots, value);
				NotifyPropertyChanged ("PointDetails");
			}
		}

		string pointDetails;
		public string PointDetails {
			get { return Points + " / " + MaxPoints; }
		}

		[XmlIgnore]
		RelayCommand deleteSquadron;
		[XmlIgnore]
		public RelayCommand DeleteSquadron {
			get {
				if (deleteSquadron == null)
					deleteSquadron = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "DeleteSquadron"));

				return deleteSquadron;
			}
		}

		[XmlIgnore]
		RelayCommand editDetails;
		[XmlIgnore]
		public RelayCommand EditDetails {
			get {
				if (editDetails == null)
					editDetails = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "EditDetails"));
			
				return editDetails;
			}
		}

		[XmlIgnore]
		RelayCommand copySquadron;
		[XmlIgnore]
		public RelayCommand CopySquadron {
			get {
				if (copySquadron == null)
					copySquadron = new RelayCommand (() => MessagingCenter.Send <Squadron> (this, "CopySquadron"));

				return copySquadron;
			}
		}

		public Squadron Copy ()
		{
			return new Squadron {
				Name = Name,
				Faction = Faction,
				MaxPoints = MaxPoints,
				Pilots = new ObservableCollection<Pilot> (Pilots),
			};
		}

		public string CreateXws ()
		{
			var json = new JObject (
				new JProperty ("faction", Faction.Id),
				new JProperty ("pilots",
					new JArray (
						from p in Pilots
						select new JObject (
							new JProperty ("name", p.Id),
							new JProperty ("ship", p.Ship.Id),
							new JObject ("upgrades",

							)
						)
					)
				),
				new JProperty ("name", Name),
				new JProperty ("points", Points)
			);
		}
	}
}


