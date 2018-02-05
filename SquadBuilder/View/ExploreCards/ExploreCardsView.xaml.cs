using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class ExploreCardsView : BaseView
	{
		public ExploreCardsView ()
		{
			InitializeComponent ();
			BindingContext = new ExploreCardsViewModel ();
		}
	}
}

