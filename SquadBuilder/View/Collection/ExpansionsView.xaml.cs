using System;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

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

