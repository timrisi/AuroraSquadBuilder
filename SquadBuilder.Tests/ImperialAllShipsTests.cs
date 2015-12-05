using System;
using NUnit.Framework;
using Xamarin.UITest;
using System.Threading;
using Xamarin.UITest.Queries;
using System.Linq;

namespace SquadBuilder.Tests
{
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class ImperialAllShipsTests
	{
		IApp app;
		Platform platform;

		public ImperialAllShipsTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);

			if (platform == Platform.iOS)
				app.Tap ("Menu");
			else
				app.Tap (x => x.Class ("ImageView"));
			
			app.Tap ("Settings");
			app.Tap ("FilterSwitch");

			if (platform == Platform.iOS)
				app.Tap ("Menu");
			else
				app.Tap (x => x.Class ("ImageView"));
					
			app.Tap ("Squadrons");

			app.Tap ("+");

			app.Tap ("FactionPicker");

			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 1)));
				app.Tap ("OK");
			} else {
				app.Tap ("Imperial");
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));
		}

		[TestCase ("TIE Adv. Prototype", "The Inquisitor", TestName = "TIE Adv Prototype")]
		[TestCase ("TIE Advanced", "Darth Vader", TestName = "TIE Advanced")]
		[TestCase ("TIE Bomber", "Major Rhymer", TestName = "TIE Bomber")]
		[TestCase ("TIE Defender", "Rexler Brath", TestName = "TIE Defender")]
		[TestCase ("TIE Fighter", "\"Howlrunner\"", TestName = "TIE Fighter")]
		[TestCase ("TIE Interceptor", "Soontir Fel", TestName = "TIE Interceptor")]
		[TestCase ("TIE Phantom", "\"Whisper\"", TestName = "TIE Phantom")]
		[TestCase ("TIE Punisher", "\"Redline\"", TestName = "TIE Punisher")]
		[TestCase ("TIE/FO Fighter", "\"Omega Leader\"", TestName = "TIE/FO Fighter")]
		[TestCase ("Firespray-31", "Boba Fett", TestName = "Firespray-31")]
		[TestCase ("Lambda-Class Shuttle", "Captain Kagi", TestName = "Lambda-Class Shuttle")]
		[TestCase ("VT-49 Decimator", "Rear Admiral Chiraneau", TestName = "VT-49 Decimator")]
		[TestCase ("Raider-class Corvette (Aft)", "Raider-class Corvette (Aft)", TestName = "Raider-class Corvette (Aft)")]
		[TestCase ("Raider-class Corvette (Fore)", "Raider-class Corvette (Fore)", TestName = "Raider-class Corvette (Fore)")]
		public void ShouldAddImperialShip (string ship, string pilot)
		{
			app.Tap ("+");

			if (!app.Query (ship).Any ())
				app.ScrollDownTo (ship, timeout: TimeSpan.FromSeconds (120));

			if (!app.Query (pilot).Any ())
				app.ScrollDownTo (pilot, timeout: TimeSpan.FromSeconds (30));

			var count = app.Query (pilot).Length;
			app.Tap (x => x.Marked (pilot).Index (count - 1));

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query (pilot));
		}
	}
}

