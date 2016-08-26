using System;
using XLabs.Forms.Mvvm;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace SquadBuilder
{
	public class ExploreExpansionContentsView : BaseView
	{
		public ExploreExpansionContentsView ()
		{
			//BindingContext = new ExploreExpansionContentsViewModel ();

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var context = BindingContext;

			var table = new TableView {
				Root = new TableRoot ()
			};

			var ships = new TableSection ("Ships");
			foreach (var ship in (BindingContext as ExploreExpansionContentsViewModel).Ships)
				ships.Add (new TextCell { Text = ship.Name });

			//var pilots = new TableSection ("Pilots");
			var pilots = new List<Pilot> ();
			foreach (var pilot in (BindingContext as ExploreExpansionContentsViewModel).Pilots)
				pilots.Add (pilot);
			//pilots.Add (new TextCell { Text = pilot.Name });

			pilots.OrderByDescending (p => p.PilotSkill).OrderBy (p => p.Ship);

			table.Root.Add (ships);

			var pilotsSection = new TableSection ("Pilots");
			foreach (var pilot in pilots)
				pilotsSection.Add (new TextCell { Text = pilot.Name, Detail = pilot.Ship.Name });

			table.Root.Add (pilotsSection);

			var upgradesSection = new TableSection ("Upgrades");
			foreach (var upgrade in (BindingContext as ExploreExpansionContentsViewModel).Upgrades)
				upgradesSection.Add (new TextCell { Text = upgrade.Name, Detail = upgrade.Category });

			table.Root.Add (upgradesSection);

			Content = table;
		}
	}
}

