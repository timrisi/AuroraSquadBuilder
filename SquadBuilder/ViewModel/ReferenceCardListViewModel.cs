using System;
using System.IO;
using System.Threading.Tasks;
using XLabs.Forms.Mvvm;
using Xamarin.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class ReferenceCardListViewModel : ViewModel
	{
		public ReferenceCardListViewModel ()
		{
			loadReferenceCards ();
		}

		void loadReferenceCards ()
		{
			var referenceCardXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText (App.ReferenceCardsFilename)));

			ReferenceCards = new ObservableCollection<ReferenceCard> (from card in referenceCardXml.Elements ()
																	  select new ReferenceCard {
																		  Name = card.Element ("Name").Value,
																		  Text = card.Element ("Text").Value,
				Detail = card.Element ("Detail") != null ? card.Element ("Detail").Value : null
			});
		}

		public string Title {
			get {
				return "Reference Cards";
			}
		}

		ReferenceCard selectedReferenceCard;
		public ReferenceCard SelectedReferenceCard {
			get { return selectedReferenceCard; }
			set { 
				SetProperty (ref selectedReferenceCard, value);

				if (selectedReferenceCard == null)
					return;
				
				Navigation.PushAsync<ReferenceCardViewModel> ((vm, page) => {
					vm.Card = selectedReferenceCard;
					selectedReferenceCard = null;
				});
			}
		}

		ObservableCollection<ReferenceCard> referenceCards;
		public ObservableCollection<ReferenceCard> ReferenceCards {
			get { return referenceCards; }
			set { SetProperty (ref referenceCards, value); }
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			SelectedReferenceCard = null;
			NotifyPropertyChanged ("ReferenceCards");
		}
	}
}

