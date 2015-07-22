using System;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using XLabs;
using Xamarin.Forms;
using System.Xml;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class EditSquadronViewModel : ViewModel
	{
		Squadron squadron;
		public Squadron Squadron {
			get { return squadron; }
			set { SetProperty (ref squadron, value); }
		}
	}
}

