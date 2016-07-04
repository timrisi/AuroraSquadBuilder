using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

