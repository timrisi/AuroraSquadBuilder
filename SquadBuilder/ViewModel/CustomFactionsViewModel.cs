using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using System.Linq;

namespace SquadBuilder
{
	public class CustomFactionsViewModel : ViewModel
	{
		public CustomFactionsViewModel ()
		{
			XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));
			Factions = new ObservableCollection <Faction> (from faction in customFactionsXml.Elements ()
				select new Faction {
					Id = faction.Attribute ("id").Value,
					Name = faction.Value,
					Color = Color.FromRgb (
						(int)faction.Element ("Color").Attribute ("r"),
						(int)faction.Element ("Color").Attribute ("g"),
						(int)faction.Element ("Color").Attribute ("b")
					)
				});

			MessagingCenter.Subscribe <Faction> (this, "Remove Faction", faction => {
				Factions.Remove (faction);
			});
		}

		public string PageName { get { return "Factions"; } }

		ObservableCollection <Faction> factions;
		public ObservableCollection <Faction> Factions {
			get {
				return factions;
			}
			set {
				SetProperty (ref factions, value);
			}
		}

		RelayCommand createFaction;
		public RelayCommand CreateFaction {
			get {
				if (createFaction == null)
					createFaction = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateFactionViewModel, Faction> (this, "Faction Created", (vm, faction) => {
							Factions.Add (faction);
							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <CreateFactionViewModel, Faction> (this, "Faction Created");
						});
							
						Navigation.PushAsync <CreateFactionViewModel> ();
					});

				return createFaction;
			}
		}
	}
}


