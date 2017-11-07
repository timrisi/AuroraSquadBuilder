using Android.App;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Message))]
namespace SquadBuilder {
	public class Message : IMessage {
		public void LongAlert (string message)
		{
			Toast.MakeText (Application.Context, message, ToastLength.Long).Show ();
		}

		public void ShortAlert (string message)
		{
			Toast.MakeText (Application.Context, message, ToastLength.Short).Show ();
		}
	}
}