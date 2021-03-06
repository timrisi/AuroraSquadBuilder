﻿using System;

using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;


namespace SquadBuilder
{
	public class ExploreExpansionsViewModel : ViewModel
	{
		public ExploreExpansionsViewModel ()
		{
			expansions = getAllExpansions ();
		}

		public string PageName { get { return "Expansions"; } }

		ObservableCollection<ExpansionGroup> expansions;
		public ObservableCollection<ExpansionGroup> Expansions {
			get { return expansions; }
			set { SetProperty (ref expansions, value); }
		}

		ObservableCollection<ExpansionGroup> getAllExpansions ()
		{
			var allExpansionGroups = new ObservableCollection<ExpansionGroup> ();

			var allExpansions = Expansion.Expansions.ToList ();

			foreach (var expansion in allExpansions) {
				var expansionGroup = allExpansionGroups.FirstOrDefault (g => g.Wave == expansion.Wave);

				if (expansionGroup == null) {
					expansionGroup = new ExpansionGroup (expansion.Wave);
					allExpansionGroups.Add (expansionGroup);
				}

				expansionGroup.Add (expansion);
			}

			return allExpansionGroups;
		}

		Expansion selectedExpansion;
		public Expansion SelectedExpansion {
			get { return selectedExpansion; }
			set {
				SetProperty (ref selectedExpansion, value);

				if (selectedExpansion != null) {
					NavigationService.PushAsync (new ExploreExpansionContentsViewModel { Expansion = selectedExpansion }).ContinueWith (task => selectedExpansion = null);
				}
			}
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			expansions = getAllExpansions ();

			NotifyPropertyChanged ("SelectedExpansion");
		}
	}
}