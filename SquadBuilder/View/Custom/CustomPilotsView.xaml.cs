using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class CustomPilotsView : BaseView
	{
		public CustomPilotsView ()
		{
			InitializeComponent ();
			BindingContext = new CustomPilotsViewModel ();
		}
	}
}

