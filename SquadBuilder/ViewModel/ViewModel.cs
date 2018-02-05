using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace SquadBuilder
{
	public abstract class ViewModel : ObservableObject {
		public virtual void OnViewAppearing ()
		{
		}

		public virtual void OnViewDisappearing ()
		{
		}
	}
}
