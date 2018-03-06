using System;

using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class UpgradesCollectionCategoryListViewModel : ViewModel
	{
		public UpgradesCollectionCategoryListViewModel ()
		{
			GetAllCategories ();
		}

		public string PageName { get { return "Upgrades"; } }

		ObservableCollection <UpgradeCategory> categories = new ObservableCollection <UpgradeCategory> ();
		public ObservableCollection <UpgradeCategory> Categories {
			get {
				return categories;
			} set {
				SetProperty (ref categories, value);
				categories.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Categories");
			}
		}

		UpgradeCategory selectedCategory = null;
		public UpgradeCategory SelectedCategory {
			get { return selectedCategory; }
			set { 
				SetProperty (ref selectedCategory, value); 

				if (value != null) {
					NavigationService.PushAsync (new UpgradesCollectionViewModel { Category = SelectedCategory.Name });
				}
			}
		}

		void GetAllCategories ()
		{
			var categoryNames = new List<string>(Upgrade.Upgrades.Select(u => u.Category).Distinct());
			Categories = new ObservableCollection<UpgradeCategory> (categoryNames.Select (u => new UpgradeCategory { Name = u, Symbol = Upgrade.GetSymbol(u) }).Distinct ());
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllCategories ();

			SelectedCategory = null;
		}
	}
}

