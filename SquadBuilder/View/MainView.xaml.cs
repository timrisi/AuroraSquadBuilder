using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class MainView : BaseView
	{
		public MainView ()
		{
			InitializeComponent ();
			BindingContext = new MainViewModel ();
		}
	}
}

