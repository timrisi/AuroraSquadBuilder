using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class UpgradesCollectionCategoryListViewModel : ViewModel
	{
		ObservableCollection <string> allCategories;

		public UpgradesCollectionCategoryListViewModel ()
		{
			GetAllCategories ();
		}

		public string PageName { get { return "Select Upgrade Type"; } }

		ObservableCollection <string> categories = new ObservableCollection <string> ();
		public ObservableCollection <string> Categories {
			get {
				return categories;
			} set {
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
					Navigation.PushAsync <UpgradesCollectionViewModel> ((vm, p) => {
						vm.Category = SelectedCategory;
					});
				}
			}
		}

		void GetAllCategories ()
		{
			allCategories = new ObservableCollection <string> (Cards.SharedInstance.Upgrades.Select (u => u.Category).Distinct ());
			Categories = new ObservableCollection <string> (allCategories);
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			GetAllCategories ();

			SelectedCategory = null;
		}
	}
}

