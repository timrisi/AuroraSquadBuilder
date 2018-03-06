using System;

using Xamarin.Forms;


namespace SquadBuilder
{
	public partial class ExpansionsView : BaseView
	{
		public ExpansionsView ()
		{
			InitializeComponent ();
			BindingContext = new ExpansionsViewModel ();
		}
	}
}

