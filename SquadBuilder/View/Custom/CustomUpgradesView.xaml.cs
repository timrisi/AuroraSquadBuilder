using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class CustomUpgradesView : BaseView
	{
		public CustomUpgradesView ()
		{
			InitializeComponent ();
			BindingContext = new CustomUpgradesViewModel ();
		}
	}
}

