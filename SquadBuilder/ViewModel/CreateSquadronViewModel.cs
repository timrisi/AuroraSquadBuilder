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
			XElement element = XElement.Load (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Factions.xml"));
			Factions = new ObservableCollection <string> (from faction in element.Elements ()
			                                              select faction.Value);
		}

		public string PlaceholderText { get { return "Enter Squadron Name"; } }

		string squadName;
		public string SquadName {
			get { return squadName; }
			set { SetProperty (ref squadName, value); }
		}

		ObservableCollection <string> factions = new ObservableCollection<string> ();
		public ObservableCollection <string> Factions {
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

