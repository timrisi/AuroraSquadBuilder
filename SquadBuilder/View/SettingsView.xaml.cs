using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class SettingsView : BaseView
	{
		public SettingsView ()
		{
			InitializeComponent ();
			BindingContext = new SettingsViewModel ();
		}
	}
}

