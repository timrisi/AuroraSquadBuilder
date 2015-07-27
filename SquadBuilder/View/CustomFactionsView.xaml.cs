using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

