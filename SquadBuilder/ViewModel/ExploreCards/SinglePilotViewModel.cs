using XLabs.Forms.Mvvm;

namespace SquadBuilder {
	public class SinglePilotViewModel : ViewModel {
		Pilot pilot;
		public Pilot Pilot {
			get { return pilot; }
			set { SetProperty (ref pilot, value); }
		}
	}
}