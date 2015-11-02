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

					if (newFaction.Name != "Mixed" && Squadron.Faction?.Name != newFaction.Name) {
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

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Factions = new ObservableCollection<Faction> (Cards.SharedInstance.AllFactions);
		}
	}
}

