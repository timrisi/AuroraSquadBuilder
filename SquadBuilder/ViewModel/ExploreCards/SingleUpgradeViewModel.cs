using System;


namespace SquadBuilder {
	public class SingleUpgradeViewModel : ViewModel{
		Upgrade upgrade;
		public Upgrade Upgrade {
			get { return upgrade; }
			set { SetProperty (ref upgrade, value); }
		}
	}
}
