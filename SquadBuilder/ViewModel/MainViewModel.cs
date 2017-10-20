using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace SquadBuilder
{
	public class MainViewModel : ViewModel
	{
		public MainViewModel(string faction = null)
		{
			Faction = faction;
			squadrons = allSquadrons;
		}

		public string PageName { get { return Faction ?? "All"; } }

		string faction;
		public string Faction {
			get { return faction; }
			set { 
				SetProperty (ref faction, value);
			}
		}

		string searchText;
		public string SearchText {
			get { return searchText; }
			set {
				if (value == null)
					value = "";

				SetProperty (ref searchText, value);

				SearchSquadrons (value);
			}
		}

		IEnumerable<Squadron> allSquadrons {
			get {
				return Cards.SharedInstance.Squadrons.Where (s => string.IsNullOrEmpty (Faction) || s?.Faction?.Name == Faction);
			}
		}

		IEnumerable<Squadron> squadrons;
		public IEnumerable <Squadron> Squadrons {
			get { return squadrons; }
			set { SetProperty (ref squadrons, value); }
		}

		Squadron selectedSquadron = null;
		public Squadron SelectedSquadron {
			get { return selectedSquadron; }
			set {
				if (Settings.Editing) {
					selectedSquadron = null;
					return;
				}
				
				SetProperty (ref selectedSquadron, value);
				if (value != null) {
					Cards.SharedInstance.CurrentSquadron = selectedSquadron;
					Navigation.PushAsync <SquadronViewModel> ((vm,p) => {
						vm.Squadron = selectedSquadron;
						selectedSquadron = null;
					});
				}
			}
		}

		public string AddSquadronText { get { return "+"; } }

		RelayCommand addSquadron;
		public RelayCommand AddSquadron {
			get {
				if (addSquadron == null)
					addSquadron = new RelayCommand (() => {
						MessagingCenter.Subscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created", async (vm, Squadron) => {
							MessagingCenter.Unsubscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created");

							Cards.SharedInstance.Squadrons.Add (Squadron);
							await Navigation.PopAsync (false);

							Cards.SharedInstance.SaveSquadrons ();
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;

							Cards.SharedInstance.CurrentSquadron = Squadron;
							Navigation.PushAsync <SquadronViewModel> ((vm2,p) => {
								vm2.Squadron = Squadron;
							});
						});
						Navigation.PushAsync <CreateSquadronViewModel> ((vm, page) => {
							if (!string.IsNullOrEmpty (Faction))
								vm.SelectedIndex = vm.Factions.IndexOf (vm.Factions.FirstOrDefault (f => f.Name == Faction));
						});
					});

				return addSquadron;
			}
		}

		RelayCommand importSquadron;
		public RelayCommand ImportSquadron {
			get {
				if (importSquadron == null)
					importSquadron = new RelayCommand (() => {
						MessagingCenter.Subscribe <ImportViewModel, Squadron> (this, "Squadron Imported", (vm, squadron) => {
							MessagingCenter.Unsubscribe <ImportViewModel, Squadron> (this, "Squadron Imported");
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadrons Imported");
							
							Cards.SharedInstance.Squadrons.Add (squadron);

							Cards.SharedInstance.SaveSquadrons ();
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;

							Cards.SharedInstance.CurrentSquadron = squadron;
							Navigation.PushAsync <SquadronViewModel> ((vm2,p) => {
								vm2.Squadron = squadron;
							});
						});

						MessagingCenter.Subscribe <ImportViewModel, List<Squadron>> (this, "Squadrons Imported", (vm, squadrons) => {
							MessagingCenter.Unsubscribe <ImportViewModel, Squadron> (this, "Squadron Imported");	
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadrons Imported");

							foreach (var squadron in squadrons)
								Cards.SharedInstance.Squadrons.Add (squadron);

							Cards.SharedInstance.SaveSquadrons ();
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;
						});

						Navigation.PushAsync <ImportViewModel> ();
					});

				return importSquadron;
			}
		}

		RelayCommand exportAll;
		public RelayCommand ExportAll {
			get {
				if (exportAll == null) {
					exportAll = new RelayCommand (() => {
						DependencyService.Get <IClipboardService> ().CopyToClipboard (Cards.SharedInstance.CreateXwc ());
						MessagingCenter.Send <MainViewModel> (this, "Squadrons Copied");
					});
				}

				return exportAll;
			}
		}

		RelayCommand sortSquadron ;
		public RelayCommand SortSquadron {
			get {
				if (sortSquadron == null)
					sortSquadron = new RelayCommand (() => {
						Settings.Editing = !Settings.Editing;
						NotifyPropertyChanged ("Squadrons");
						Squadrons = allSquadrons;
					});

				return sortSquadron;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Cards.SharedInstance.CurrentSquadron = null;

			Cards.SharedInstance.SaveSquadrons ().ContinueWith ((task) => NotifyPropertyChanged ("Squadrons"));

			NotifyPropertyChanged ("Squadrons");
			NotifyPropertyChanged ("SelectedSquadron");

			MessagingCenter.Unsubscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created");
			MessagingCenter.Unsubscribe <ImportViewModel, Squadron> (this, "Squadron Imported");
			MessagingCenter.Unsubscribe <ImportViewModel, Squadron> (this, "Squadrons Imported");

			MessagingCenter.Subscribe <Squadron> (this, "DeleteSquadron", squadron => {
				Cards.SharedInstance.Squadrons.Remove (squadron);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Cards.SharedInstance.SaveSquadrons ();
			});

			MessagingCenter.Subscribe <Squadron> (this, "EditDetails", squadron => {
				Navigation.PushAsync<EditSquadronViewModel> ((vm, p) => vm.Squadron = squadron);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Cards.SharedInstance.SaveSquadrons ();
			});

			MessagingCenter.Subscribe <Squadron> (this, "CopySquadron", squadron => {
				Cards.SharedInstance.Squadrons.Add (squadron.Copy ());
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Cards.SharedInstance.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "MoveSquadronUp", squadron => {
				var index = Cards.SharedInstance.Squadrons.IndexOf (squadron);

				if (index == 0)
					return;

				int newIndex;

				if (Faction != null) {
					newIndex = Cards.SharedInstance.Squadrons.ToList ().FindLastIndex (index - 1, s => s.Faction.Id == squadron.Faction.Id);
					if (newIndex < 0)
						return;
				} else
					newIndex = index - 1;

				Cards.SharedInstance.Squadrons.Move (index, --index);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Cards.SharedInstance.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "MoveSquadronDown", squadron => {
				var index = Cards.SharedInstance.Squadrons.IndexOf (squadron);

				if (index == Cards.SharedInstance.Squadrons.Count - 1)
					return;

				int newIndex;

				if (Faction != null) {
					newIndex = Cards.SharedInstance.Squadrons.ToList ().FindIndex (index + 1, s => s.Faction.Id == squadron.Faction.Id);
					if (newIndex < 0)
						return;
				} else
					newIndex = index + 1;

				Cards.SharedInstance.Squadrons.Move (index, newIndex);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Cards.SharedInstance.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<SquadronViewModel> (this, "Squadron updated", vm => {
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
			});

			Squadrons = allSquadrons;
		}

		public override void OnViewDisappearing ()
		{
			base.OnViewDisappearing ();

			MessagingCenter.Unsubscribe <Squadron> (this, "DeleteSquadron");
			MessagingCenter.Unsubscribe <Squadron> (this, "CopySquadron");
			MessagingCenter.Unsubscribe <Squadron> (this, "EditDetails");
			MessagingCenter.Unsubscribe<Squadron> (this, "MoveSquadronUp");
			MessagingCenter.Unsubscribe<Squadron> (this, "MoveSquadronDown");
		}

		public void SearchSquadrons (string text)
		{
			text = text.ToLower ();

			var filteredSquadrons = allSquadrons.Where (s => s.Name.ToLower ().Contains (text) || s.Pilots != null && s.Pilots.Any (p => p != null && (p.Name.ToLower ().Contains (text) || (p.Ship != null && p.Ship.Name.ToLower ().Contains (text)) || p.UpgradesEquippedString.ToLower ().Contains (text))));

			Squadrons = filteredSquadrons;
		}
	}
}

