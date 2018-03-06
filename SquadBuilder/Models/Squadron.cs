using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Linq;

using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema;

using Newtonsoft.Json;

using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SquadBuilder {
	public class Squadron : ObservableObject {
		const string XwsVersion = "1.0.0";
		public const string SquadronsFilename = "squadrons.xml";
		public const string XwcFilename = "squadrons.xwc";

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
		Command deleteSquadron;
		[XmlIgnore]
		public Command DeleteSquadron {
			get {
				if (deleteSquadron == null)
					deleteSquadron = new Command (() => MessagingCenter.Send<Squadron> (this, "DeleteSquadron"));

				return deleteSquadron;
			}
		}

		[XmlIgnore]
		Command editDetails;
		[XmlIgnore]
		public Command EditDetails {
			get {
				if (editDetails == null)
					editDetails = new Command (() => MessagingCenter.Send<Squadron> (this, "EditDetails"));

				return editDetails;
			}
		}

		[XmlIgnore]
		Command copySquadron;
		[XmlIgnore]
		public Command CopySquadron {
			get {
				if (copySquadron == null)
					copySquadron = new Command (() => MessagingCenter.Send<Squadron> (this, "CopySquadron"));

				return copySquadron;
			}
		}

		[XmlIgnore]
		Command moveUp;
		[XmlIgnore]
		public Command MoveUp {
			get {
				if (moveUp == null)
					moveUp = new Command (() => MessagingCenter.Send<Squadron> (this, "MoveSquadronUp"));

				return moveUp;
			}
		}

		[XmlIgnore]
		Command moveDown;
		[XmlIgnore]
		public Command MoveDown {
			get {
				if (moveDown == null)
					moveDown = new Command (() => MessagingCenter.Send<Squadron> (this, "MoveSquadronDown"));

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
#if IOS
					HockeyApp.MetricsManager.TrackEvent ("Xws error",
			                    	new Dictionary<string, string> { { "error message", e.Message } },
									new Dictionary<string, double> { { "time", 1.0 } }
			                    );
#endif
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
					Faction = Faction.AllFactions.FirstOrDefault (f => f.Id == json ["faction"].ToString ()),
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

				foreach (var pilotObject in json ["pilots"]) {
					var pilot = (Pilot.AllPilots.FirstOrDefault (p => (p.CanonicalName ?? p.Id) == pilotObject ["name"].ToString () && (p.Ship.CanonicalName ?? p.Ship.Id) == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
						 Pilot.AllPilots.FirstOrDefault (p => p.OldId == pilotObject ["name"].ToString () && (p.Ship.CanonicalName ?? p.Ship.Id) == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
						 Pilot.AllPilots.FirstOrDefault (p => (p.CanonicalName ?? p.Id) == pilotObject ["name"].ToString () && p.Ship.OldId == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)) ??
						Pilot.AllPilots.FirstOrDefault (p => p.OldId == pilotObject ["name"].ToString () && p.Ship.OldId == pilotObject ["ship"].ToString () && (squadron.Faction.Id == "mixed" || p.Faction.Id == squadron.Faction.Id)))?.Copy ();

					if (pilot == null)
						continue;

					while (pilot.UpgradesEquipped.Count < pilot.UpgradeTypes.Count)
						pilot.UpgradesEquipped.Add (null);

					if (pilotObject ["multisection_id"] != null) {
						pilot.MultiSectionId = (int)pilotObject ["multisection_id"];
					}

					if (pilotObject ["upgrades"] != null) {
						List<Upgrade> skippedUpgrades = new List<Upgrade> ();

						foreach (var upgradeTypeArray in pilotObject ["upgrades"]) {
							var upgradeType = (upgradeTypeArray as JProperty).Name;

							foreach (var value in upgradeTypeArray.Values ()) {
								var upgrade = (Upgrade.AllUpgrades.FirstOrDefault (u => u.CategoryId == upgradeType && u.CanonicalName == value.ToString ()) ??
									       Upgrade.AllUpgrades.FirstOrDefault (u => u.CategoryId == upgradeType && u.OldId == value.ToString ()))?.Copy ();

								if (upgrade == null)
									continue;

								var index = pilot.Upgrades.IndexOf (Upgrade.CreateUpgradeSlot(upgrade.Category));

								if (index < 0) {
									skippedUpgrades.Add (upgrade);
									continue;
								}

								if (upgrade.Slots != null && upgrade.Slots.Count > 0) {
									var pilotUpgrades = new List<object> (pilot.Upgrades);
									pilotUpgrades.Remove (Upgrade.CreateUpgradeSlot(upgrade.Category));

									bool isValid = true;
									foreach (var slot in upgrade.Slots) {
										var slotObject = Upgrade.CreateUpgradeSlot(slot);
										if (pilotUpgrades.Contains (slotObject))
											pilotUpgrades.Remove (slotObject);
										else {
											isValid = false;
											break;
										}
									}

									if (!isValid) {
										skippedUpgrades.Add (upgrade);
										continue;
									}
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
							var index = pilot.Upgrades.IndexOf (Upgrade.CreateUpgradeSlot(upgrade.Category));

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
				MessagingCenter.Send<Squadron> (new Squadron (), "Error loading squad");
			}

			return new Squadron () ;
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

		#region Static Methods
		static Squadron currentSquadron;
		public static Squadron CurrentSquadron {
			get { return currentSquadron; }
			set { currentSquadron = value; }
		}

		static ObservableCollection<Squadron> squadrons;
		public static ObservableCollection<Squadron> Squadrons {
			get {
				if (squadrons == null)
					GetAllSquadrons ();

				return squadrons;
			}
			set { squadrons = value; }
		}

		public static void GetAllSquadrons ()
		{
			var service = DependencyService.Get<ISaveAndLoad> ();

			if (service.FileExists (XwcFilename)) {
				var xwcText = service.LoadText (XwcFilename);
				if (string.IsNullOrEmpty (xwcText)) {
					Squadrons = new ObservableCollection<Squadron> ();
					return;
				}

				var json = JObject.Parse (xwcText);


				if (json ["container"] == null)
					throw new ArgumentException ("Container key is missing");

				var squads = new List<Squadron> ();

				foreach (var squadXws in json ["container"])
					squads.Add (Squadron.FromXws (squadXws.ToString ()));

				Squadrons = new ObservableCollection<Squadron> (squads);
			} else if (service.FileExists (SquadronsFilename)) {
				var serializedXml = service.LoadText (SquadronsFilename);
				serializedXml.Replace ("<Owned>0</Owned>", "");
				serializedXml.Replace ("<owned>0</owned>", "");
				var serializer = new XmlSerializer (typeof (ObservableCollection<Squadron>));

				using (TextReader reader = new StringReader (serializedXml)) {
					var squads = (ObservableCollection<Squadron>) serializer.Deserialize (reader);

					foreach (var squad in squads) {
						squad.Faction = Faction.AllFactions.FirstOrDefault (f => f.Id == squad.Faction?.Id);

						foreach (var pilot in squad.Pilots) {
							pilot.Ship = Ship.AllShips.FirstOrDefault (f => f.Id == pilot.Ship.Id)?.Copy ();
							if (pilot.Ship.ManeuverGridImage == null) {
								pilot.Ship.ManeuverGridImage = "";
							}

							if (squad.Faction?.Id == "scum") {
								if (pilot.Id == "bobafett")
									pilot.Id = "bobafettscum";
								if (pilot.Id == "kathscarlet")
									pilot.Id = "kathscarletscum";
							}
							if (pilot.Id == "Ello Asty")
								pilot.Id = "elloasty";
							if (pilot.Id == "4lom")
								pilot.Id = "fourlom";

							if (Pilot.CustomPilots.Any (p => p.Id == pilot.Id))
								pilot.IsCustom = true;

							pilot.Preview = Pilot.AllPilots.FirstOrDefault (p => p.Id == pilot.Id)?.Preview ?? false;

							foreach (var upgrade in pilot.UpgradesEquipped) {
								if (upgrade == null)
									continue;

								upgrade.Preview = Upgrade.AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id)?.Preview ?? false;

								if (upgrade.Id == "r2d2" && upgrade.Category == "Crew")
									upgrade.Id = "r2d2crew";
								if (upgrade.Id == "4lom")
									upgrade.Id = "fourlom";

								if (upgrade.CategoryId == null)
									upgrade.CategoryId = Upgrade.AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id && u.Category == upgrade.Category)?.CategoryId;
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "emperorpalpatine") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "emperorpalpatine"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "wookieecommandos") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "wookieecommandos"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}

							if (pilot?.UpgradesEquipped?.Count (u => u?.Id == "unguidedrockets") > 1) {
								var index = pilot.UpgradesEquipped.IndexOf (pilot.UpgradesEquipped.First (u => u?.Id == "unguidedrockets"));
								pilot.UpgradesEquipped.RemoveAt (index);
								pilot.UpgradeTypes.RemoveAt (index);
							}
						}
					}

					Squadrons = squads;
				}
			} else {
				Squadrons = new ObservableCollection<Squadron> ();
				return;
			}
		}

		public static async Task SaveSquadrons ()
		{
			var service = DependencyService.Get<ISaveAndLoad> ();

			if (Squadrons.Count == 0) {
				service.DeleteFile (XwcFilename);
				return;
			}

			var xwc = CreateXwc ();

			if (!service.FileExists (XwcFilename) || service.LoadText (XwcFilename) != xwc) {
				service.SaveText (XwcFilename, xwc);

				App.Storage.Put<DateTime> (SettingsViewModel.ModifiedDateKey, DateTime.Now);

				if (App.DropboxClient != null)
					await SettingsViewModel.SaveToDropbox ();
			}
		}

		public static string CreateXwc ()
		{
			try {
				var squads = new JArray ();
				foreach (var s in Squadrons) {
					var squad = s.CreateXwsObject ();
					if (squad == null)
						continue;
					{ }
					squads.Add (squad);
				}

				var json = new JObject (
					new JProperty ("container", squads),
					new JProperty ("vendor",
				new JObject (
							new JProperty ("aurora",
					    new JObject (
									new JProperty ("builder", "Aurora Squad Builder"),
						    new JProperty ("builder_url", "https://itunes.apple.com/us/app/aurora-squad-builder/id1020767927?mt=8")
							    )
				    )
				    )
			)
				);

				return json.ToString ();
			} catch (Exception e) {
				return "";
			}
		}
		#endregion
	}
}


