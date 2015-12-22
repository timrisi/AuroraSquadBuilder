using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SquadBuilder
{
	public class ExpansionGroup : ObservableCollection <Expansion>
	{
		public ExpansionGroup ()
		{
		}

		public string Wave { get; set; }

		public ExpansionGroup (string wave)
		{
			Wave = wave;
		}

		public string Header {
			get { return Wave; }
		}
	}
}