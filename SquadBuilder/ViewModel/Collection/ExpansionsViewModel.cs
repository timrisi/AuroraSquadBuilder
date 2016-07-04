using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class ExpansionsViewModel : ViewModel
	{
		public ExpansionsViewModel ()
		{
			expansions = getAllExpansions ();
		}

		public string PageName { get { return "Collection"; } }

		ObservableCollection <ExpansionGroup> expansions;
		public ObservableCollection <ExpansionGroup> Expansions {
			get { return expansions; }
			set { SetProperty (ref expansions, value); }
		}

		ObservableCollection <ExpansionGroup> getAllExpansions ()
		{
			var allExpansionGroups = new ObservableCollection <ExpansionGroup> ();

			var allExpansions = Cards.SharedInstance.Expansions.ToList ();

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

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			expansions = getAllExpansions ();
		}
	}
}