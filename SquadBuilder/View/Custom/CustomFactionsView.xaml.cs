using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class CustomFactionsView : BaseView
	{
		public CustomFactionsView ()
		{
			InitializeComponent ();
			BindingContext = new CustomFactionsViewModel ();
		}
	}
}

