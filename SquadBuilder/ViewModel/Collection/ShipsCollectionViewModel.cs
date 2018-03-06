using System;

using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using Xamarin.Forms;

namespace SquadBuilder
{
	public class ShipsCollectionViewModel : ViewModel
	{
		public ShipsCollectionViewModel ()
		{
			Ships = Ship.Ships;
		}

		public string PageName { get { return "Ships"; } }

		ObservableCollection <Ship> ships;
		public ObservableCollection <Ship> Ships {
			get { return ships; }
			set { SetProperty (ref ships, value); }
		}

		public override void OnViewAppearing ()
		{
			base.OnViewAppearing ();

			Ships = Ship.Ships;
		}
	}
}