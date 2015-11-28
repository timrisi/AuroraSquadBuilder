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
	public class MixedFilteredShipsTests
	{
		IApp app;
		Platform platform;

		public MixedFilteredShipsTests (Platform platform)
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
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 3)));
				app.Tap ("OK");
			} else {
				app.Tap ("Mixed");
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ($"0/100"));
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
//
//		[Test]
//		public void ShouldAddRebelHWK290 ()
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
//		public void ShouldAddRebelYWing ()
//		{
//			addPilot ("Y-Wing", "Horton Salm");
//		}
//
//		[Test]
//		public void ShouldAddRebelZ95 ()
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
//
//		[Test]
//		public void ShouldAddTIEAdvPrototype ()
//		{
//			addPilot ("TIE Adv. Prototype", "The Inquisitor");
//		}
//
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
//		public void ShouldAddImperialFirespray()
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
//
//		[Test]
//		public void ShouldAddG1AStarfighter ()
//		{
//			addPilot ("G-1A Starfighter", "Zuckuss");
//		}
//
//		[Test]
//		public void ShouldAddScumHWK290 ()
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
//		public void ShouldAddScumYWing ()
//		{
//			addPilot ("Y-Wing", "Kavil");
//		}
//
//		[Test]
//		public void ShouldAddScumZ95 ()
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
//		public void ShouldAddScumFirespray ()
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

