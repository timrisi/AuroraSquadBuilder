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
			MessagingCenter.Subscribe <Faction> (this, "Remove Faction", faction => {
				Factions.Remove (faction);
				Faction.CustomFactions.Remove (faction);
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
							Faction.CustomFactions.Add (faction);
							Faction.GetAllFactions ();
							Navigation.RemoveAsync <CreateFactionViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateFactionViewModel, Faction> (this, "Faction Created");
						});
							
						Navigation.PushAsync <CreateFactionViewModel> ();
					});

				return createFaction;
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Factions = new ObservableCollection <Faction> (Faction.CustomFactions);
		}
	}
}


