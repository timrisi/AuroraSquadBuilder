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
	public class CreateSquadronViewModel : ViewModel
	{
		public CreateSquadronViewModel ()
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

		public string PlaceholderText { get { return "Enter Squadron Name"; } }

		string squadName;
		public string SquadName {
			get { return squadName; }
			set { SetProperty (ref squadName, value); }
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

		int points = 100;
		public int Points {
			get { return points; }
			set {
				SetProperty (ref points, value);
			}
		}

		int selectedIndex = 0;
		public int SelectedIndex {
			get { return selectedIndex; }
			set { SetProperty (ref selectedIndex, value); }
		}

		public string SaveButtonText { get { return "Save"; } }

		RelayCommand saveSquadron;
		public RelayCommand SaveSquadron {
			get {
				if (saveSquadron == null)
					saveSquadron = new RelayCommand (() => {
						var squadron = new Squadron {
							Name = SquadName,
							MaxPoints = Points,
							Faction = Factions [SelectedIndex]
						};
						MessagingCenter.Send <CreateSquadronViewModel, Squadron> (this, "Squadron Created", squadron);
					});

				return saveSquadron;
			}
		}
	}
}

