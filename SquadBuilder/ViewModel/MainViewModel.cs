using System;

using System.Collections.ObjectModel;

using Xamarin.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace SquadBuilder {
	public class MainViewModel : ViewModel {
		public MainViewModel (string faction = null)
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
				return Squadron.Squadrons.Where (s => string.IsNullOrEmpty (Faction) || s?.Faction?.Name == Faction);
			}
		}

		IEnumerable<Squadron> squadrons;
		public IEnumerable<Squadron> Squadrons {
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
					Squadron.CurrentSquadron = selectedSquadron;
					NavigationService.PushAsync (new SquadronViewModel { Squadron = selectedSquadron }).ContinueWith (task => selectedSquadron = null);
				}
			}
		}

		public string AddSquadronText { get { return "+"; } }

		Command addSquadron;
		public Command AddSquadron {
			get {
				if (addSquadron == null) {
					addSquadron = new Command (() => {
						CreateSquadron (Faction);
					});
				}

				return addSquadron;
			}
		}

		public void CreateSquadron (string faction)
		{
			MessagingCenter.Subscribe<CreateSquadronViewModel, Squadron> (this, "Squadron Created", async (viewModel, Squadron) => {
				MessagingCenter.Unsubscribe<CreateSquadronViewModel, Squadron> (this, "Squadron Created");

				Squadron.Squadrons.Add (Squadron);
				await NavigationService.PopAsync ();

				await Squadron.SaveSquadrons ();
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;

				Squadron.CurrentSquadron = Squadron;
				await NavigationService.PushAsync (new SquadronViewModel { Squadron = Squadron });
			});
			var vm = new CreateSquadronViewModel ();
			if (!string.IsNullOrEmpty (faction))
				vm.SelectedIndex = vm.Factions.IndexOf (vm.Factions.FirstOrDefault (f => f.Name == faction));

			NavigationService.PushAsync (vm);
		}

		Command importSquadron;
		public Command ImportSquadron {
			get {
				if (importSquadron == null)
					importSquadron = new Command (() => {
						MessagingCenter.Subscribe<ImportViewModel, Squadron> (this, "Squadron Imported", (vm, squadron) => {
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadron Imported");
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadrons Imported");

							Squadron.Squadrons.Add (squadron);

							Squadron.SaveSquadrons ();
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;

							Squadron.CurrentSquadron = squadron;
							NavigationService.PushAsync (new SquadronViewModel { Squadron = squadron });
						});

						MessagingCenter.Subscribe<ImportViewModel, List<Squadron>> (this, "Squadrons Imported", (vm, squadrons) => {
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadron Imported");
							MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadrons Imported");

							foreach (var squadron in squadrons)
								Squadron.Squadrons.Add (squadron);

							Squadron.SaveSquadrons ();
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;
						});

						NavigationService.PushAsync (new ImportViewModel ());
					});

				return importSquadron;
			}
		}

		Command exportAll;
		public Command ExportAll {
			get {
				if (exportAll == null) {
					exportAll = new Command (() => {
						DependencyService.Get<IClipboardService> ().CopyToClipboard (Squadron.CreateXwc ());
						MessagingCenter.Send<MainViewModel> (this, "Squadrons Copied");
					});
				}

				return exportAll;
			}
		}

		Command sortSquadron;
		public Command SortSquadron {
			get {
				if (sortSquadron == null)
					sortSquadron = new Command (() => {
						if (Settings.Editing) {
							Settings.Editing = !Settings.Editing;
							NotifyPropertyChanged ("Squadrons");
							Squadrons = allSquadrons;
							return;

						}

						MessagingCenter.Subscribe<MenuView, string> (this, "Sort type selected", (MenuView view, string result) => {
							switch (result) {
							case "Alphabetical":
								Squadron.Squadrons = new ObservableCollection<Squadron> (Squadron.Squadrons.OrderBy (s => s.PilotsString, new EmptyStringsAreLast ()).OrderBy (s => s.Name, new EmptyStringsAreLast ()));
								NotifyPropertyChanged ("Squadrons");
								Squadrons = allSquadrons;
								Squadron.SaveSquadrons ();
								break;
							case "Wins":
								Squadron.Squadrons = new ObservableCollection<Squadron> (Squadron.Squadrons.OrderByDescending (s => s.Wins));
								NotifyPropertyChanged ("Squadrons");
								Squadrons = allSquadrons;
								Squadron.SaveSquadrons ();
								break;
							case "Loses":
								Squadron.Squadrons = new ObservableCollection<Squadron> (Squadron.Squadrons.OrderByDescending (s => s.Losses));
								NotifyPropertyChanged ("Squadrons");
								Squadrons = allSquadrons;
								Squadron.SaveSquadrons ();
								break;
							case "Total Games":
								Squadron.Squadrons = new ObservableCollection<Squadron> (Squadron.Squadrons.OrderByDescending (s => s.Wins + s.Losses + s.Draws));
								NotifyPropertyChanged ("Squadrons");
								Squadrons = allSquadrons;
								Squadron.SaveSquadrons ();
								break;
							case "Manual":
								Settings.Editing = !Settings.Editing;
								NotifyPropertyChanged ("Squadrons");
								Squadrons = allSquadrons;
								break;
							default:
								return;
							}
						});

						MessagingCenter.Send<MainViewModel> (this, "Sort squadrons");

						//Settings.Editing = !Settings.Editing;
						//NotifyPropertyChanged ("Squadrons");
						//Squadrons = allSquadrons;
					});

				return sortSquadron;
			}
		}

		public class EmptyStringsAreLast : IComparer<string> {
			public int Compare (string x, string y)
			{
				if (String.IsNullOrEmpty (y) && !String.IsNullOrEmpty (x)) {
					return -1;
				} else if (!String.IsNullOrEmpty (y) && String.IsNullOrEmpty (x)) {
					return 1;
				} else {
					return String.Compare (x, y);
				}
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Squadron.CurrentSquadron = null;

			Squadron.SaveSquadrons ().ContinueWith ((task) => NotifyPropertyChanged ("Squadrons"));

			NotifyPropertyChanged ("Squadrons");
			NotifyPropertyChanged ("SelectedSquadron");

			MessagingCenter.Unsubscribe<CreateSquadronViewModel, Squadron> (this, "Squadron Created");
			MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadron Imported");
			MessagingCenter.Unsubscribe<ImportViewModel, Squadron> (this, "Squadrons Imported");

			MessagingCenter.Subscribe <App> (this, "Create Rebel", (obj) => {
				CreateSquadron ("Rebel");
			});

			MessagingCenter.Subscribe<App> (this, "Create Imperial", (obj) => {
				CreateSquadron ("Imperial");
			});

			MessagingCenter.Subscribe<App> (this, "Create Scum", (obj) => {
				CreateSquadron ("Scum & Villainy");
			});
			                                 
			MessagingCenter.Subscribe<Squadron> (this, "DeleteSquadron", squadron => {
				Squadron.Squadrons.Remove (squadron);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Squadron.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "EditDetails", squadron => {
				NavigationService.PushAsync (new EditSquadronViewModel { Squadron = squadron });
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Squadron.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "CopySquadron", squadron => {
				Squadron.Squadrons.Add (squadron.Copy ());
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Squadron.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "MoveSquadronUp", squadron => {
				var index = Squadron.Squadrons.IndexOf (squadron);

				if (index == 0)
					return;

				int newIndex;

				if (Faction != null) {
					newIndex = Squadron.Squadrons.ToList ().FindLastIndex (index - 1, s => s.Faction.Id == squadron.Faction.Id);
					if (newIndex < 0)
						return;
				} else
					newIndex = index - 1;

				Squadron.Squadrons.Move (index, --index);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Squadron.SaveSquadrons ();
			});

			MessagingCenter.Subscribe<Squadron> (this, "MoveSquadronDown", squadron => {
				var index = Squadron.Squadrons.IndexOf (squadron);

				if (index == Squadron.Squadrons.Count - 1)
					return;

				int newIndex;

				if (Faction != null) {
					newIndex = Squadron.Squadrons.ToList ().FindIndex (index + 1, s => s.Faction.Id == squadron.Faction.Id);
					if (newIndex < 0)
						return;
				} else
					newIndex = index + 1;

				Squadron.Squadrons.Move (index, newIndex);
				NotifyPropertyChanged ("Squadrons");
				Squadrons = allSquadrons;
				Squadron.SaveSquadrons ();
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

			MessagingCenter.Unsubscribe<Squadron> (this, "DeleteSquadron");
			MessagingCenter.Unsubscribe<Squadron> (this, "CopySquadron");
			MessagingCenter.Unsubscribe<Squadron> (this, "EditDetails");
			MessagingCenter.Unsubscribe<Squadron> (this, "MoveSquadronUp");
			MessagingCenter.Unsubscribe<Squadron> (this, "MoveSquadronDown");
			MessagingCenter.Unsubscribe<App> (this, "Create Rebel");
			MessagingCenter.Unsubscribe<App> (this, "Create Imperial");
			MessagingCenter.Unsubscribe<App> (this, "Create Scum");
		}

		public void SearchSquadrons (string text)
		{
			text = text.ToLower ();

			var filteredSquadrons = allSquadrons.Where (s => s.Name.ToLower ().Contains (text) || s.Pilots != null && s.Pilots.Any (p => p != null && (p.Name.ToLower ().Contains (text) || (p.Ship != null && p.Ship.Name.ToLower ().Contains (text)) || (bool)(p.UpgradesEquippedString?.ToLower ().Contains (text) ?? false))));

			Squadrons = filteredSquadrons;
		}
	}
}

