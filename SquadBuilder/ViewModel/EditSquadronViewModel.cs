using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

namespace SquadBuilder
{
	public class EditSquadronViewModel : ViewModel
	{
		public EditSquadronViewModel ()
		{
			XElement element = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
			var factions = (from faction in element.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				}).ToList ();

			if ((bool)Application.Current.Properties ["AllowCustom"]) {
				XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
				var customFactions = (from faction in customFactionsXml.Elements ()
					select new Faction {
						Id = faction.Attribute ("id").Value,
						Name = faction.Value,
						Color = Color.FromRgb (
							(int)faction.Element ("Color").Attribute ("r"),
							(int)faction.Element ("Color").Attribute ("g"),
							(int)faction.Element ("Color").Attribute ("b")
						)
					});
				factions.AddRange (customFactions);
			}

			Factions = new ObservableCollection<Faction> (factions);
		}
			
		Squadron squadron;
		public Squadron Squadron {
			get { return squadron; }
			set { 
				SetProperty (ref squadron, value);
				SelectedIndex = Factions.IndexOf (Squadron.Faction);
			}
		}

		ObservableCollection <Faction> factions = new ObservableCollection<Faction> ();
		public ObservableCollection <Faction> Factions {
			get {
				return factions;
			}
			set {
				SetProperty (ref factions, value);
			}
		}

		int selectedIndex = 0;
		public int SelectedIndex {
			get { return selectedIndex; }
			set { 
				SetProperty (ref selectedIndex, value); 
				if (SelectedIndex >= 0) {
					var newFaction = Factions [SelectedIndex];

					if (newFaction.Name != "Mixed" && Squadron.Faction.Name != newFaction.Name) {
						var pilots = new List <Pilot> (Squadron.Pilots);
						foreach (var pilot in pilots) {
							if (pilot.Faction.Name != newFaction.Name)
								Squadron.Pilots.Remove (pilot);
						}
					}

					Squadron.Faction = newFaction;
				}
			}
		}
	}
}

