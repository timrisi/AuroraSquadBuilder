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
	public class FilteredShipPilotTests
	{
		IApp app;
		Platform platform;

		public FilteredShipPilotTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);

			app.Tap ("+");

			app.Tap ("FactionPicker");
		}

		[TestCase ("Imperial", 1, "TIE Adv. Prototype", "The Inquisitor", TestName = "TIE Adv Prototype")]
		[TestCase ("Rebel", 0, "A-Wing", "Tycho Celchu", TestName = "A-Wing")]
		[TestCase ("Scum & Villainy", 2, "G-1A Starfighter", "Zuckuss", TestName = "G-1A Starfighter")]
		public void ShouldAddCorrectFactionShipToList (string faction, int factionIndex, string ship, string pilot)
		{
			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", factionIndex)));
				app.Tap ("OK");
			} else {
				app.Tap (faction);
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap (ship);

			if (platform == Platform.Android && app.Query (pilot).Length > 1)
				app.Tap (x => x.Marked (pilot).Index (1));
			else
				app.Tap (pilot);

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query (pilot));		}

		[TestCase ("Imperial", 1, "A-Wing", TestName = "Imperial A-wing")]
		[TestCase ("Imperial", 1, "G-1A Starfighter", TestName = "Imperial G-1A")]
		[TestCase ("Rebel", 0, "TIE Adv. Prototype", TestName = "Rebel TAP")]
		[TestCase ("Rebel", 0, "G-1A Starfighter", TestName = "Rebel G-1A")]
		[TestCase ("Scum & Villainy", 2, "A-Wing", TestName = "Scum A-Wing")]
		[TestCase ("Scum & Villainy", 2, "TIE Adv. Prototype", TestName = "Scum TAP")]
		public void ShouldNotAddWrongFactionShipToList (string faction, int factionIndex, string ship)
		{
			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", factionIndex)));
				app.Tap ("OK");
			} else {
				app.Tap (faction);
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));
			app.WaitForElement ("Ships");

			Assert.IsEmpty (app.Query (ship), "Ship type " + ship + " should not exist for the " + faction + " faction");
		}

		[TestCase ("Imperial", 1, "TIE Adv. Prototype", "The Inquisitor", TestName = "TIE Adv Prototype")]
		[TestCase ("Rebel", 0, "A-Wing", "Tycho Celchu", TestName = "A-Wing")]
		[TestCase ("Scum & Villainy", 2, "G-1A Starfighter", "Zuckuss", TestName = "G-1A Starfighter")]
		public void ShouldAddAllFactionShipsToMixedList (string faction, int factionIndex, string ship, string pilot)
		{
			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", factionIndex)));
				app.Tap ("OK");
			} else {
				app.Tap (faction);
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			if (!app.Query (ship).Any ())
				app.ScrollDownTo (ship, strategy: ScrollStrategy.Gesture, timeout: TimeSpan.FromSeconds (30));

			app.Tap (ship);

			if (!app.Query (pilot).Any ())
				app.ScrollDownTo (pilot, timeout: TimeSpan.FromSeconds (30));

			if (platform == Platform.Android && app.Query (pilot).Length > 1)
				app.Tap (x => x.Marked (pilot).Index (1));
			else
				app.Tap (pilot);

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query (pilot));
		}
	}
}

