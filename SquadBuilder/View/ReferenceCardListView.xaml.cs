using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class ReferenceCardListView : BaseView
	{
		public ReferenceCardListView ()
		{
			InitializeComponent ();
			BindingContext = new ReferenceCardListViewModel ();
		}
	}
}

