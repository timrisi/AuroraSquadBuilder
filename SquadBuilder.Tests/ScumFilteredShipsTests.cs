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
	public class ScumFilteredShipsTests
	{
		IApp app;
		Platform platform;

		public ScumFilteredShipsTests (Platform platform)
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
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 2)));
				app.Tap ("OK");
			} else {
				app.Tap ("Scum & Villainy");
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));
		}

		[TestCase ("G-1A Starfighter", "Zuckuss", TestName = "G-1A Starfighter")]
		[TestCase ("HWK-290", "Dace Bonearm", TestName = "HWK-290")]
		[TestCase ("Kihraxz", "Talonbane Cobra", TestName = "Kihraxz")]
		[TestCase ("M3-A \"Scyk\" Interceptor", "Serissu", TestName = "M3-A \"Scyk\" Interceptor")]
		[TestCase ("StarViper", "Prince Xizor", TestName = "StarViper")]
		[TestCase ("Y-Wing", "Kavil", TestName = "Y-Wing")]
		[TestCase ("Z-95 Headhunter", "N'Dru Suhlak", TestName = "Z-95 Headhunter")]
		[TestCase ("Aggressor", "IG88-A", TestName = "Aggressor")]
		[TestCase ("Firespray-31", "Boba Fett", TestName = "Firespray-31")]
		[TestCase ("JumpMaster 5000", "Dengar", TestName = "JumpMaster 5000")]
		[TestCase ("YV-666", "Bossk", TestName = "YV-666")]
		public void ShouldAddScumShip (string ship, string pilot)
		{
			app.Tap ("+");

			if (!app.Query (ship).Any ())
				app.ScrollDownTo (ship, timeout: TimeSpan.FromSeconds (30));

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

