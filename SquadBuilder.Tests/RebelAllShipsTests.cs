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
	public class RebelAllShipsTests
	{
		IApp app;
		Platform platform;

		public RebelAllShipsTests (Platform platform)
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
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 0)));
				app.Tap ("OK");
			} else {
				app.Tap ("Rebel");
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ( "0/100"));
		}

		[Test]
		public void ShouldAddAWing ()
		{
			addPilot ("A-Wing", "Tycho Celchu");
		}

//		[Test]
//		public void ShouldAddBWing ()
//		{
//			addPilot ("B-Wing", "Ten Numb");
//		}
//
//		[Test]
//		public void ShouldAddEWing ()
//		{
//			addPilot ("E-Wing", "Corran Horn");
//		}
//
//		[Test]
//		public void ShouldAddHWK290 ()
//		{
//			addPilot ("HWK-290", "Jan Ors");
//		}
//
//		[Test]
//		public void ShouldAddKWing ()
//		{
//			addPilot ("K-Wing", "Miranda Doni");
//		}
//
//		[Test]
//		public void ShouldAddT70XWing ()
//		{
//			addPilot ("T-70 X-wing", "Poe Dameron");
//		}
//
//		[Test]
//		public void ShouldAddXWing ()
//		{
//			addPilot ("X-Wing", "Wedge Antilles");
//		}
//
//		[Test]
//		public void ShouldAddYWing ()
//		{
//			addPilot ("Y-Wing", "Horton Salm");
//		}
//
//		[Test]
//		public void ShouldAddZ95 ()
//		{
//			addPilot ("Z-95 Headhunter", "Airen Cracken");
//		}
//
//		[Test]
//		public void ShouldAddGhost ()
//		{
//			addPilot ("VCX-100", "Hera Syndulla");
//		}
//
//		[Test]
//		public void ShouldAddYT1300 ()
//		{
//			addPilot ("YT-1300", "Han Solo");
//		}
//
//		[Test]
//		public void ShouldAddYT2400 ()
//		{
//			addPilot ("YT-2400", "Dash Rendar");
//		}
//
//		[Test]
//		public void ShouldAddCR90Aft ()
//		{
//			addPilot ("CR90 Corvette (Aft)", "CR90 Corvette (Aft)");
//		}
//
//		[Test]
//		public void ShouldAddCR90Fore ()
//		{
//			addPilot ("CR90 Corvette (Fore)", "CR90 Corvette (Fore)");
//		}
//
//		[Test]
//		public void ShouldAddGR75 ()
//		{
//			addPilot ("GR-75 Medium Transport", "GR-75 Medium Transport");
//		}

		void addPilot (string ship, string pilot)
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

