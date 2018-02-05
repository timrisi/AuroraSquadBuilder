using System;
using System.Collections.Generic;
using Xamarin.Forms;


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

