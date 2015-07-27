﻿using System;
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
		const string squadronsFilename = "squadrons.xml";

		public MainViewModel()
		{
			var service = DependencyService.Get <ISaveAndLoad> ();
			if (service.FileExists (squadronsFilename)) {
				var serializedXml = service.LoadText (squadronsFilename);
				var serializer = new XmlSerializer (typeof(ObservableCollection<Squadron>));
				using (TextReader reader = new StringReader (serializedXml))
					Squadrons = (ObservableCollection <Squadron>)serializer.Deserialize (reader);
			}

			MessagingCenter.Subscribe <App> (this, "Save Squadrons", (app) => {
				SaveSquadrons ();
			});

			MessagingCenter.Subscribe <Squadron> (this, "DeleteSquadron", 
				squadron => {
					Squadrons.Remove (squadron);
				}
			);

			MessagingCenter.Subscribe <Squadron> (this, "EditDetails", 
				squadron => {
					Navigation.PushAsync <EditSquadronViewModel> ((vm, p) => vm.Squadron = squadron);
				}
			);

			MessagingCenter.Subscribe <Squadron> (this, "CopySquadron", 
				squadron => {
					Squadrons.Add (squadron.Copy ());
				}
			);

			MessagingCenter.Subscribe <MenuViewModel> (this, "Create Faction", vm => {
				Navigation.PushAsync <CreateFactionViewModel> ();
			});
		}

		public string PageName { get { return "Squadrons"; } }

		ObservableCollection <Squadron> squadrons = new ObservableCollection <Squadron> ();
		public ObservableCollection <Squadron> Squadrons {
			get { return squadrons; }
			set { SetProperty (ref squadrons, value); }
		}

		Squadron selectedSquadron = null;
		public Squadron SelectedSquadron {
			get { return selectedSquadron; }
			set { 
				SetProperty (ref selectedSquadron, value);
				if (value != null) {
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
						MessagingCenter.Subscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created", (vm, Squadron) => {
							Squadrons.Add (Squadron);
							Navigation.PopAsync ();
							MessagingCenter.Unsubscribe <CreateSquadronViewModel, Squadron> (this, "Squadron Created");
						});
						Navigation.PushAsync <CreateSquadronViewModel> ();
					});

				return addSquadron;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			NotifyPropertyChanged ("Squadrons");
			NotifyPropertyChanged ("SelectedSquadron");
		}

		public void SaveSquadrons ()
		{
			if (Squadrons.Count == 0)
				DependencyService.Get <ISaveAndLoad> ().DeleteFile (squadronsFilename);
			
			var serializer = new XmlSerializer (typeof (ObservableCollection<Squadron>));
			using (var stringWriter = new StringWriter ()) {
				serializer.Serialize (stringWriter, Squadrons);
				string serializedXML = stringWriter.ToString ();

				DependencyService.Get <ISaveAndLoad> ().SaveText (squadronsFilename, serializedXML);
			}
		}
	}
}

