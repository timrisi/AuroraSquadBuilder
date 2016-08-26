using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

