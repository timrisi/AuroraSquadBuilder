using System;

namespace SquadBuilder
{
	public class ReferenceCardViewModel : ViewModel
	{
		public string Title {
			get {
				return Card?.Name;
			}
		}

		ReferenceCard card;
		public ReferenceCard Card {
			get { return card; }
			set { SetProperty (ref card, value); }
		}
	}
}

