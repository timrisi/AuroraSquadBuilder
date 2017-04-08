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

using Newtonsoft.Json;
using XLabs.Platform.Device;
using System.Globalization;

namespace SquadBuilder {
	public class Squadron : ObservableObject {
		const string XwsVersion = "1.0.0";

		public Squadron ()
		{
			Name = "";
			Pilots = new ObservableCollection<Pilot> ();
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

		ObservableCollection<Pilot> pilots = new ObservableCollection<Pilot> ();
		public ObservableCollection<Pilot> Pilots {
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

		public bool ContainsPreview {
			get {
				return Pilots.Any (p => p.Preview || p.UpgradesEquipped.Any (u => u?.Preview ?? false));
			}
		}

		public bool ContainsCustom {
			get {
				return Pilots.Any (p => p.IsCustom || p.UpgradesEquipped.Any (u => u?.IsCustom ?? false));
			}
		}

		public bool ContainsCCL {
			get {
				return Pilots.Any (p => p.CCL || p.UpgradesEquipped.Any (u => u?.CCL ?? false));
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

		public bool Editing {
			get { return Settings.Editing; }
		}

		[XmlIgnore]
		RelayCommand deleteSquadron;
		[XmlIgnore]
		public RelayCommand DeleteSquadron {
			get {
				if (deleteSquadron == null)
					deleteSquadron = new RelayCommand (() => MessagingCenter.Send<Squadron> (this, "DeleteSquadron"));

				return deleteSquadron;
			}
		}

		[XmlIgnore]
		RelayCommand editDetails;
		[XmlIgnore]
		public RelayCommand EditDetails {
			get {
				if (editDetails == null)
					editDetails = new RelayCommand (() => MessagingCenter.Send<Squadron> (this, "EditDetails"));

				return editDetails;
			}
		}

		[XmlIgnore]
		RelayCommand copySquadron;
		[XmlIgnore]
		public RelayCommand CopySquadron {
			get {
				if (copySquadron == null)
						copySquadron = new RelayCommand (() => MessagingCenter.Send<Squadron> (this, "CopySquadron"));

				return copySquadron;
			}
		}

		[XmlIgnore]
		RelayCommand moveUp;
		[XmlIgnore]
		public RelayCommand MoveUp {
			get {
				if (moveUp == null)
					moveUp = new RelayCommand (() => MessagingCenter.Send<Squadron> (this, "MoveSquadronUp"));

				return moveUp;
			}
		}

		[XmlIgnore]
		RelayCommand moveDown;
		[XmlIgnore]
		public RelayCommand MoveDown {
			get {
				if (moveDown == null)
					moveDown = new RelayCommand (() => MessagingCenter.Send<Squadron> (this, "MoveSquadronDown"));

				return moveDown;
			}
		}

		public Squadron Copy ()
		{
			var squadron = new Squadron {
				Name = Name + " - Copy",
				Faction = Faction,
				MaxPoints = MaxPoints,
				Pilots = new ObservableCollection<Pilot> (),
			};

			foreach (var pilot in pilots)
				squadron.Pilots.Add (pilot.Copy ());

			return squadron;
		}

		public JObject CreateXwsObject ()
		{
			var json = new JObject (
				new JProperty ("name", Name),
				new JProperty ("points", Points),
				new JProperty ("faction", Faction?.Id),
				new JProperty ("version", XwsVersion),
				new JProperty ("description", Description ?? ""));
			
			var pilots = new JArray ();

			foreach (var p in Pilots) {
				try {
					var obj = new JObject (
						new JProperty ("name", p.CanonicalName ?? p.Id),
						new JProperty ("ship", p.Ship.CanonicalName ?? p.Ship.Id),
						new JProperty ("upgrades", new JObject (
							from category in
							(from upgrade in p.UpgradesEquipped.Where (u => u != null && !string.IsNullOrEmpty (u.CategoryId))
							 select upgrade.CategoryId).Distinct ()
							select new JProperty (category,
								new JArray (
									from upgrade in p.UpgradesEquipped.Where (u => u != null && !string.IsNullOrEmpty (u.CategoryId))
									where upgrade.CategoryId == category
									select upgrade.CanonicalName ?? upgrade.Id
								)
							)
							)
						)
					);

					if (p.MultiSectionId >= 0)
						obj.Add (new JProperty ("multisection_id", p.MultiSectionId));

					pilots.Add (obj);
				} catch (Exception e) {
					HockeyApp.MetricsManager.TrackEvent ("Xws error",
                    	new Dictionary<string, string> { { "error message", e.Message } },
						new Dictionary<string, double> { { "time", 1.0 } }
                    );
					MessagingCenter.Send<Squadron, string> (this, "Xws Error", $"Error creating xws object for squadron {Name}"); 
				}
			}

			json.Add (new JProperty ("pilots", pilots));
			json.Add (
				new JProperty ("vendor",
					 new JObject (
					   new JProperty ("aurora", new
							JObject (
								new JProperty ("builder", "Aurora Squad Builder"),
								new JProperty ("builder_url", "https://itunes.apple.com/us/app/aurora-squad-builder/id1020767927?mt=8"),
								new JProperty ("maxpoints", MaxPoints),
								new JProperty ("wins", Wins),
								new JProperty ("losses", Losses),
								new JProperty ("draws", Draws)
							)
						 )
					)
				)
			);

			return json;
		}

		public string CreateXws ()
		{
			return CreateXwsObject ().ToString ();
		}

		public static Squadron FromXws (string xws)
		{
			var json = JObject.Parse (xws);

			try {
				var squadron = new Squadron () {
					Name = json ["name"].ToString (),
					Description = json ["description"].ToString (),
					Faction = Cards.SharedInstance.Factions.FirstOrDefault (f => f.Id == json ["faction"].ToString ()),
					MaxPoints = (int)json ["points"],
					Pilots = new ObservableCollection<Pilot> ()
				};

				int wins = 0, losses = 0, draws = 0;
				int.TryParse (json ["vendor"]? ["aurora"]? ["wins"]?.ToString (), out wins);
				int.TryParse (json ["vendor"]? ["aurora"]? ["losses"]?.ToString (), out losses);
				int.TryParse (json ["vendor"]? ["aurora"]? ["draws"]?.ToString (), out draws);

				squadron.Wins = wins;
				squadron.Losses = losses;
				squadron.Draws = draws;

				int maxPoints = 100;
				var parsed = int.TryParse (json ["vendor"]? ["aurora"]? ["maxpoints"]?.ToString (), out maxPoints);
				if (!parsed)
					maxPoints = 100;
				squadron.MaxPoints = maxPoints;

				if (squadron.Faction.Id == "mixed")
				Console.WriteLine("Foo");

				foreach (var pilotObject in json ["pilots"]) {
					var pilot = (Cards.SharedInstance.Pilots.FirstOrDefault (p => (p.CanonicalName ?? p.Id) == pilotObject ["name"].ToString () && (p.Ship.CanonicalName ?? p.Ship.Id) == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
				                 Cards.SharedInstance.Pilots.FirstOrDefault (p => p.OldId == pilotObject ["name"].ToString () && (p.Ship.CanonicalName ?? p.Ship.Id) == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
				                 Cards.SharedInstance.Pilots.FirstOrDefault (p => (p.CanonicalName ?? p.Id) == pilotObject ["name"].ToString () && p.Ship.OldId == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
				                 Cards.SharedInstance.Pilots.FirstOrDefault (p => p.OldId == pilotObject ["name"].ToString () && p.Ship.OldId == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)))?.Copy ();

					if (pilot == null) {
						//pilot = Cards.SharedInstance.Pilots.FirstOrDefault (p => p.OldId == pilotObject ["name"].ToString ());
						continue;
					}

					while (pilot.UpgradesEquipped.Count < pilot.UpgradeTypes.Count)
						pilot.UpgradesEquipped.Add (null);

					if (pilotObject ["upgrades"] != null) {
						List<Upgrade> skippedUpgrades = new List<Upgrade> ();

						foreach (var upgradeTypeArray in pilotObject ["upgrades"]) {
							var upgradeType = (upgradeTypeArray as JProperty).Name;


							foreach (var value in upgradeTypeArray.Values ()) {
								var upgrade = Cards.SharedInstance.Upgrades.FirstOrDefault (u => u.CategoryId == upgradeType && u.Id == value.ToString ())?.Copy ();

								if (upgrade == null)
									continue;

								var index = pilot.Upgrades.IndexOf (new { Name = upgrade.Category, IsUpgrade = false });

								if (index < 0) {
									skippedUpgrades.Add (upgrade);
									continue;
								}

								pilot.EquipUpgrade (index, upgrade);
							}
						}

						if (pilot.UpgradesEquipped.Any (u => u?.Id == "heavyscykinterceptor")) {
							if (skippedUpgrades.Any (u => u.CategoryId == "missile")) {
								pilot.UpgradeTypes.Add ("Missile");
								pilot.UpgradesEquipped.Add (null);
							} else if (skippedUpgrades.Any (u => u.CategoryId == "torpedo")) {
								pilot.UpgradeTypes.Add ("Torpedo");
								pilot.UpgradesEquipped.Add (null);
							} else {
								pilot.UpgradeTypes.Add ("Cannon");
								pilot.UpgradesEquipped.Add (null);
							}
						}

						if (pilot.UpgradesEquipped.Any (u => u?.Id == "ordnancetubes")) {
							foreach (var upgrade in skippedUpgrades) {
								if (upgrade.CategoryId == "missile") {
									var index = pilot.UpgradeTypes.IndexOf ("Hardpoint");
									pilot.UpgradeTypes [index] = "Missile";
								} else if (upgrade.CategoryId == "torpedo") {
									var index = pilot.UpgradeTypes.IndexOf ("Hardpoint");
									pilot.UpgradeTypes [index] = "Torpedo";
								}
							}
						}

						foreach (var upgrade in skippedUpgrades) {
							var index = pilot.Upgrades.IndexOf (new { Name = upgrade.Category, IsUpgrade = false });

							if (index < 0) {
								continue;
							}

							pilot.EquipUpgrade (index, upgrade);
						}
					}

					squadron.pilots.Add (pilot);
				}

				return squadron;
			} catch (Exception e) {
				var schemaText = DependencyService.Get<ISaveAndLoad> ().LoadText ("schema.json");
				var schema = JSchema.Parse (schemaText);

				IList<string> errors;
				var valid = json.IsValid (schema, out errors);

				if (!valid) {
					MessagingCenter.Send<Squadron, IList<string>> (new Squadron (), "Invalid xws info", errors);
					return null;
				}
				Console.WriteLine (e.Message);
			}

			return null;
		}

		public override bool Equals (object obj)
		{
			if (obj == null || !(obj is Squadron))
				return false;

			var squadron = obj as Squadron;

			return Name == squadron.Name &&
				Faction?.Id == squadron.Faction?.Id &&
				Points == squadron.Points &&
				MaxPoints == squadron.MaxPoints &&
				Description == squadron.Description &&
				Pilots.SequenceEqual (squadron.Pilots);
		}

		public override int GetHashCode ()
		{
			return (Name + Description + Points + MaxPoints + Faction.Id + Pilots).GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ($"Name={Name}, Faction={Faction.Name}");
		}
	}
}


