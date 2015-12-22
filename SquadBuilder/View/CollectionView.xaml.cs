using System;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace SquadBuilder
{
	public partial class CollectionView : BaseView
	{
		public CollectionView ()
		{
			InitializeComponent ();
			BindingContext = new CollectionViewModel ();
		}
	}
}

