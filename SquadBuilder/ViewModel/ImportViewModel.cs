using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
						if (string.IsNullOrEmpty (ImportText))
							return;

						try {
							var json = JObject.Parse (ImportText);
							if (json ["container"] != null) {
								var squadrons = new List<Squadron> ();

								foreach (var squadXws in json ["container"])
									squadrons.Add (Squadron.FromXws (squadXws.ToString ()));

								if (squadrons.Count > 0)
									MessagingCenter.Send<ImportViewModel, List<Squadron>> (this, "Squadrons Imported", squadrons);

								Navigation.RemoveAsync<ImportViewModel> (this);
							} else {

								var squadron = Squadron.FromXws (ImportText);

								if (squadron == null)
									return;
								else {
									MessagingCenter.Send<ImportViewModel, Squadron> (this, "Squadron Imported", squadron);
									Navigation.RemoveAsync<ImportViewModel> (this);
								}
							}
						} catch (Exception e) {
							return;
						}
 					});
				}

				return saveSquadron;
			}
		}
	}
}

