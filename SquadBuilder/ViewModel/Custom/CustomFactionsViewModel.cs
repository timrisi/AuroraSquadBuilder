using System;

using System.Collections.ObjectModel;

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

		Command createFaction;
		public Command CreateFaction {
			get {
				if (createFaction == null)
					createFaction = new Command (() => {
						MessagingCenter.Subscribe <CreateFactionViewModel, Faction> (this, "Faction Created", (vm, faction) => {
							Factions.Add (faction);
							Faction.CustomFactions.Add (faction);
							Faction.GetAllFactions ();
							NavigationService.PopAsync ().ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted); // <CreateFactionViewModel> (vm);
							MessagingCenter.Unsubscribe <CreateFactionViewModel, Faction> (this, "Faction Created");
						});
							
						NavigationService.PushAsync (new CreateFactionViewModel ()).ContinueWith (t => Console.WriteLine (t.Exception), System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
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


