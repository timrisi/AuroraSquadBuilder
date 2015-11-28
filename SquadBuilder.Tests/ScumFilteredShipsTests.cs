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

		[Test]
		public void ShouldAddG1AStarfighter ()
		{
			addPilot ("G-1A Starfighter", "Zuckuss");
		}

//		[Test]
//		public void ShouldAddHWK290 ()
//		{
//			addPilot ("HWK-290", "Dace Bonearm");
//		}
//
//		[Test]
//		public void ShouldAddKihraxz ()
//		{
//			addPilot ("Kihraxz", "Talonbane Cobra");
//		}
//
//		[Test]
//		public void ShouldAddM3AInterceptor ()
//		{
//			addPilot ("M3-A \"Scyk\" Interceptor", "Serissu");
//		}
//
//		[Test]
//		public void ShouldAddStarViper ()
//		{
//			addPilot ("StarViper", "Prince Xizor");
//		}
//
//		[Test]
//		public void ShouldAddYWing ()
//		{
//			addPilot ("Y-Wing", "Kavil");
//		}
//
//		[Test]
//		public void ShouldAddZ95 ()
//		{
//			addPilot ("Z-95 Headhunter", "N'Dru Suhlak");
//		}
//
//		[Test]
//		public void ShouldAddAggressor ()
//		{
//			addPilot ("Aggressor", "IG88-A");
//		}
//
//		[Test]
//		public void ShouldAddFirespray ()
//		{
//			addPilot ("Firespray-31", "Boba Fett");
//		}
//
//		[Test]
//		public void ShouldAddJumpMaster ()
//		{
//			addPilot ("JumpMaster 5000", "Dengar");
//		}
//
//		[Test]
//		public void ShouldAddYV666 ()
//		{
//			addPilot ("YV-666", "Bossk");
//		}

		void addPilot (string ship, string pilot)
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

