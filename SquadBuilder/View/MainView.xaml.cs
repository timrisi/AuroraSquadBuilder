using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class MainView : BaseView
	{
		public MainView (string faction = null)
		{
			InitializeComponent ();
			BindingContext = new MainViewModel (faction);
		}
	}
}