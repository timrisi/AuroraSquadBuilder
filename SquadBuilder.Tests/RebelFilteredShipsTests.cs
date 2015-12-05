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
	public class RebelFilteredShipsTests
	{
		IApp app;
		Platform platform;

		public RebelFilteredShipsTests (Platform platform)
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
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 0)));
				app.Tap ("OK");
			} else {
				app.Tap ("Rebel");
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));
		}

		[TestCase ("A-Wing", "Tycho Celchu", TestName = "A-Wing")]
		[TestCase ("B-Wing", "Ten Numb", TestName = "B-Wing")]
		[TestCase ("E-Wing", "Corran Horn", TestName = "E-Wing")]
		[TestCase ("HWK-290", "Jan Ors", TestName = "HWK-290")]
		[TestCase ("K-Wing", "Miranda Doni", TestName = "K-Wing")]
		[TestCase ("T-70 X-wing", "Poe Dameron", TestName = "T-70 X-wing")]
		[TestCase ("X-Wing", "Wedge Antilles", TestName = "X-Wing")]
		[TestCase ("Y-Wing", "Horton Salm", TestName = "Y-Wing")]
		[TestCase ("Z-95 Headhunter", "Airen Cracken", TestName = "Z-95 Headhunter")]
		[TestCase ("VCX-100", "Hera Syndulla", TestName = "VCX-100")]
		[TestCase ("YT-1300", "Han Solo", TestName = "YT-1300")]
		[TestCase ("YT-2400", "Dash Rendar", TestName = "YT-2400")]
		[TestCase ("CR90 Corvette (Aft)", "CR90 Corvette (Aft)", TestName = "CR90 Corvette (Aft)")]
		[TestCase ("CR90 Corvette (Fore)", "CR90 Corvette (Fore)", TestName = "CR90 Corvette (Fore)")]
		[TestCase ("GR-75 Medium Transport", "GR-75 Medium Transport", TestName = "GR-75 Medium Transport")]
		public void ShouldAddRebelShip (string ship, string pilot)
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

