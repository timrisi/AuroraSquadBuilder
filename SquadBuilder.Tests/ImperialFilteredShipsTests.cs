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
	public class ImperialFilteredShipsTests
	{
		IApp app;
		Platform platform;

		public ImperialFilteredShipsTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);

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

		[Test]
		public void ShouldAddTIEAdvPrototype ()
		{
			addPilot ("TIE Adv. Prototype", "The Inquisitor");
		}

//		[Test]
//		public void ShouldAddTIEAdvanced ()
//		{
//			addPilot ("TIE Advanced", "Darth Vader");
//		}
//
//		[Test]
//		public void ShouldAddTIEBomber ()
//		{
//			addPilot ("TIE Bomber", "Major Rhymer");
//		}
//
//		[Test]
//		public void ShouldAddTIEDefender ()
//		{
//			addPilot ("TIE Defender", "Rexler Brath");
//		}
//
//		[Test]
//		public void ShouldAddTIEFighter ()
//		{
//			addPilot ("TIE Fighter", "\"Howlrunner\"");
//		}
//
//		[Test]
//		public void ShouldAddTIEInterceptor ()
//		{
//			addPilot ("TIE Interceptor", "Soontir Fel");
//		}
//
//		[Test]
//		public void ShouldAddTIEPhantom ()
//		{
//			addPilot ("TIE Phantom", "\"Whisper\"");
//		}
//
//		[Test]
//		public void ShouldAddTIEPunisher ()
//		{
//			addPilot ("TIE Punisher", "\"Redline\"");
//		}
//
//		[Test]
//		public void ShouldAddTIEFOFighter()
//		{
//			addPilot ("TIE/FO Fighter", "\"Omega Leader\"");
//		}
//
//		[Test]
//		public void ShouldAddFirespray()
//		{
//			addPilot ("Firespray-31", "Boba Fett");
//		}
//
//		[Test]
//		public void ShouldAddLambdaShuttle()
//		{
//			addPilot ("Lambda-Class Shuttle", "Captain Kagi");
//		}
//
//		[Test]
//		public void ShouldAddDecimator()
//		{
//			addPilot ("VT-49 Decimator", "Rear Admiral Chiraneau");
//		}
//
//		[Test]
//		public void ShouldAddRaiderAft()
//		{
//			addPilot ("Raider-class Corvette (Aft)", "Raider-class Corvette (Aft)");
//		}
//
//		[Test]
//		public void ShouldAddRaiderFore()
//		{
//			addPilot ("Raider-class Corvette (Fore)", "Raider-class Corvette (Fore)");
//		}

		void addPilot (string ship, string pilot)
		{
			app.Tap ("+");

			if (!app.Query (ship).Any ()) {
				app.ScrollDownTo (ship, timeout: TimeSpan.FromSeconds (30));
			}

			app.Tap (ship);

			if (platform == Platform.Android && app.Query (pilot).Length > 1)
				app.Tap (x => x.Marked (pilot).Index (1));
			else
				app.Tap (pilot);

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query (pilot));
		}
	}
}

