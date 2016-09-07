using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class ExploreUpgradesCategoryListViewModel : ViewModel
	{
		ObservableCollection<string> allCategories;

		public ExploreUpgradesCategoryListViewModel ()
		{
			GetAllCategories ();
		}

		public string PageName { get { return "Upgrades"; } }

		ObservableCollection<string> categories = new ObservableCollection<string> ();
		public ObservableCollection<string> Categories {
			get {
				return categories;
			}
			set {
				SetProperty (ref categories, value);
				categories.CollectionChanged += (sender, e) => NotifyPropertyChanged ("Categories");
			}
		}

		string selectedCategory = null;
		public string SelectedCategory {
			get { return selectedCategory; }
			set {
				SetProperty (ref selectedCategory, value);

				if (value != null) {
					Navigation.PushAsync<ExploreUpgradesViewModel> ((vm, p) => {
						vm.UpgradeType = SelectedCategory;
					}, true);
				}
			}
		}

		void GetAllCategories ()
		{
			allCategories = new ObservableCollection<string> (Cards.SharedInstance.Upgrades.Select (u => u.Category).Distinct ());
			Categories = new ObservableCollection<string> (allCategories);
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllCategories ();

			SelectedCategory = null;
		}
	}
}

