using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class CustomShipsView : BaseView
	{
		public CustomShipsView ()
		{
			InitializeComponent ();
			BindingContext = new CustomShipsViewModel ();
		}
	}
}

