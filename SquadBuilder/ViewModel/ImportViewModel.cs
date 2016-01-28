using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class ImportViewModel : ViewModel
	{
		string importText = "";
		public string ImportText {
			get { return importText; }
			set { SetProperty (ref importText, value); }
		}

		RelayCommand saveSquadron;
		public RelayCommand SaveSquadron {
			get { 
				if (saveSquadron == null) {
					saveSquadron = new RelayCommand (() => {
						var squadron = Squadron.FromXws (ImportText);

						if (squadron == null)
							return;
						else {
							MessagingCenter.Send <ImportViewModel, Squadron> (this, "Squadron Imported", squadron);
							Navigation.RemoveAsync <ImportViewModel> (this);
						}
					});
				}

				return saveSquadron;
			}
		}
	}
}

