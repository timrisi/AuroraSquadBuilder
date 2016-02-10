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
using Newtonsoft.Json.Schema;
#if __IOS__
using Dropbox.CoreApi.iOS;
#endif
using Newtonsoft.Json;
using XLabs.Platform.Device;

namespace SquadBuilder
{
	public class Squadron : ObservableObject
	{
		const string XwsVersion = "0.3.0";

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

		string description;
		public string Description {
			get { return description; }
			set { SetProperty (ref description, value); }
		}

		public bool DescriptionVisible {
			get { return !string.IsNullOrEmpty (description); }
		}

		ObservableCollection <Pilot> pilots = new ObservableCollection <Pilot> ();
		public ObservableCollection <Pilot> Pilots { 
			get { 
				return pilots;
			}
			set {
				SetProperty (ref pilots, value);
				NotifyPropertyChanged ("PointDetails");
				Pilots.CollectionChanged += (sender, e) => 
					NotifyPropertyChanged ("PointsDescription");
			}
		}

		int wins = 0;
		public int Wins {
			get { return wins; }
			set { 
				if (value < 0)
					value = 0;
				
				SetProperty (ref wins, value); 
			}
		}

		int losses = 0;
		public int Losses {
			get { return losses; }
			set { 
				if (value < 0)
					value = 0;
				
				SetProperty (ref losses, value); 
			}
		}

		int draws = 0;
		public int Draws {
			get { return draws; }
			set { 
				if (value < 0)
					value = 0;
				
				SetProperty (ref draws, value); 
			}
		}

		public string PointDetails {
			get { return Points + " / " + MaxPoints; }
		}

		public string PilotsString {
			get { return string.Join (", ", Pilots.Select (p => p.Name)); }
		}

		public string PointsDescription {
			get {
				return Points + "/" + MaxPoints;
			}
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
			var squadron = new Squadron {
				Name = Name,
				Faction = Faction,
				MaxPoints = MaxPoints,
				Pilots = new ObservableCollection<Pilot> (),
			};

			foreach (var pilot in pilots)
				squadron.Pilots.Add (pilot.Copy ());

			return squadron;
		}

		public string CreateXws ()
		{
			var multiSections = new Dictionary <string, string> {
				{ "cr90f", "cr90corvette" },
				{ "cr90a", "cr90corvette" },
				{ "raiderf", "raiderclasscorvette" },
				{ "raidera", "raiderclasscorvette" }
			};

			var json = new JObject (
				new JProperty ("name", Name),
				new JProperty ("points", Points),
				new JProperty ("faction", Faction.Id),
				new JProperty ("version", XwsVersion),
				new JProperty ("pilots",
					new JArray (
						from p in Pilots
						select new JObject (
							new JProperty ("name", p.Id),
							new JProperty ("ship", multiSections.ContainsKey (p.Ship.Id) ? multiSections [p.Ship.Id] : p.Ship.Id),
							new JProperty ("upgrades", new JObject (
								from category in 
									(from upgrade in p.UpgradesEquipped.Where (u => u != null)
									select upgrade.CategoryId).Distinct ()
								select new JProperty (category, 
									new JArray (
										from upgrade in p.UpgradesEquipped.Where (u => u != null)
										where upgrade.CategoryId == category
										select upgrade.Id
									)
									)
								)
							)
						)
					)
				)
			);

			var schemaText = DependencyService.Get <ISaveAndLoad> ().LoadText ("schema.json");
			var schema = JSchema.Parse (schemaText);

			IList<string> errors;
			var valid = json.IsValid (schema, out errors);

			var jsonString = json.ToString ();

			return jsonString;
		}

		public static Squadron FromXws (string xws)
		{
			var json = JObject.Parse (xws);

			var schemaText = DependencyService.Get <ISaveAndLoad> ().LoadText ("schema.json");
			var schema = JSchema.Parse (schemaText);

			try {
				var squadron = new Squadron () {
					Name = json ["name"].ToString (),
					Faction = Cards.SharedInstance.Factions.FirstOrDefault (f => f.Id == json ["faction"].ToString ()),
					MaxPoints = (int)json ["points"],
					Pilots = new ObservableCollection<Pilot> ()
				};

				foreach (var pilotObject in json ["pilots"]) {
					var pilot = Cards.SharedInstance.Pilots.FirstOrDefault (p => p.Id == pilotObject ["name"].ToString () && p.Ship.Id == pilotObject ["ship"].ToString () && p.Faction.Id == squadron.Faction.Id)?.Copy ();

					if (pilot == null)
						continue;

					while (pilot.UpgradesEquipped.Count < pilot.UpgradeTypes.Count)
						pilot.UpgradesEquipped.Add (null);

					foreach (var upgradeTypeArray in pilotObject ["upgrades"]) {
						var upgradeType = (upgradeTypeArray as JProperty).Name;

						foreach (var value in upgradeTypeArray.Values ()) {
							var upgrade = Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.CategoryId == upgradeType && u.Id == value.ToString ())?.Copy ();

							if (upgrade == null)
								continue;

							var index = pilot.Upgrades.IndexOf (new { Name = upgrade.Category, IsUpgrade = false });

							if (index < 0)
								continue;

							pilot.EquipUpgrade (index, upgrade);
						}
					}

					squadron.pilots.Add (pilot);
				}

				return squadron;
			} catch (Exception e) {
				IList<string> errors;
				var valid = json.IsValid (schema, out errors);

				if (!valid) {
					MessagingCenter.Send <Squadron, IList<string>> (null, "Invalid xws info", errors);
					return null;
				}
				Console.WriteLine (e.Message);
			}

			return null;
		}
	}
}


