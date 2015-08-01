using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

