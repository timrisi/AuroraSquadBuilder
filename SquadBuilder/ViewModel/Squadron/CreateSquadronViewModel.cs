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
			Factions = new ObservableCollection<Faction> (Cards.SharedInstance.AllFactions);
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

		string description;
		public string Description {
			get { return description; }
			set { SetProperty (ref description, value); }
		}

		public string SaveButtonText { get { return "Save"; } }

		RelayCommand saveSquadron;
		public RelayCommand SaveSquadron {
			get {
				if (saveSquadron == null)
					saveSquadron = new RelayCommand (() => {

						if (Points < 0) {
							MessagingCenter.Send <CreateSquadronViewModel> (this, "Negative Squad Points");
							return;
						}

						var squadron = new Squadron {
							Name = SquadName,
							MaxPoints = Points,
							Faction = Factions [SelectedIndex],
							Description = Description
						};
						MessagingCenter.Send <CreateSquadronViewModel, Squadron> (this, "Squadron Created", squadron);
					});

				return saveSquadron;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();
			Factions = new ObservableCollection<Faction> (Cards.SharedInstance.AllFactions);
		}
	}
}

