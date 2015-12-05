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

		[TestCase ("A-Wing", "Tycho Celchu", TestName = "A-Wing")]
		[TestCase ("B-Wing", "Ten Numb", TestName = "B-Wing")]
		[TestCase ("E-Wing", "Corran Horn", TestName = "E-Wing")]
		[TestCase ("HWK-290", "Jan Ors", TestName = "Rebel HWK-290")]
		[TestCase ("K-Wing", "Miranda Doni", TestName = "K-Wing")]
		[TestCase ("T-70 X-wing", "Poe Dameron", TestName = "T-70 X-wing")]
		[TestCase ("X-Wing", "Wedge Antilles", TestName = "X-Wing")]
		[TestCase ("Y-Wing", "Horton Salm", TestName = "Rebel Y-Wing")]
		[TestCase ("Z-95 Headhunter", "Airen Cracken", TestName = "Rebel Z-95 Headhunter")]
		[TestCase ("VCX-100", "Hera Syndulla", TestName = "VCX-100")]
		[TestCase ("YT-1300", "Han Solo", TestName = "YT-1300")]
		[TestCase ("YT-2400", "Dash Rendar", TestName = "YT-2400")]
		[TestCase ("CR90 Corvette (Aft)", "CR90 Corvette (Aft)", TestName = "CR90 Corvette (Aft)")]
		[TestCase ("CR90 Corvette (Fore)", "CR90 Corvette (Fore)", TestName = "CR90 Corvette (Fore)")]
		[TestCase ("GR-75 Medium Transport", "GR-75 Medium Transport", TestName = "GR-75 Medium Transport")]
		[TestCase ("TIE Adv. Prototype", "The Inquisitor", TestName = "TIE Adv Prototype")]
		[TestCase ("TIE Advanced", "Darth Vader", TestName = "TIE Advanced")]
		[TestCase ("TIE Bomber", "Major Rhymer", TestName = "TIE Bomber")]
		[TestCase ("TIE Defender", "Rexler Brath", TestName = "TIE Defender")]
		[TestCase ("TIE Fighter", "\"Howlrunner\"", TestName = "TIE Fighter")]
		[TestCase ("TIE Interceptor", "Soontir Fel", TestName = "TIE Interceptor")]
		[TestCase ("TIE Phantom", "\"Whisper\"", TestName = "TIE Phantom")]
		[TestCase ("TIE Punisher", "\"Redline\"", TestName = "TIE Punisher")]
		[TestCase ("TIE/FO Fighter", "\"Omega Leader\"", TestName = "TIE/FO Fighter")]
		[TestCase ("Firespray-31", "Boba Fett", TestName = "Imperial Firespray-31")]
		[TestCase ("Lambda-Class Shuttle", "Captain Kagi", TestName = "Lambda-Class Shuttle")]
		[TestCase ("VT-49 Decimator", "Rear Admiral Chiraneau", TestName = "VT-49 Decimator")]
		[TestCase ("Raider-class Corvette (Aft)", "Raider-class Corvette (Aft)", TestName = "Raider-class Corvette (Aft)")]
		[TestCase ("Raider-class Corvette (Fore)", "Raider-class Corvette (Fore)", TestName = "Raider-class Corvette (Fore)")]
		[TestCase ("G-1A Starfighter", "Zuckuss", TestName = "G-1A Starfighter")]
		[TestCase ("HWK-290", "Dace Bonearm", TestName = "Scum HWK-290")]
		[TestCase ("Kihraxz", "Talonbane Cobra", TestName = "Kihraxz")]
		[TestCase ("M3-A \"Scyk\" Interceptor", "Serissu", TestName = "M3-A \"Scyk\" Interceptor")]
		[TestCase ("StarViper", "Prince Xizor", TestName = "StarViper")]
		[TestCase ("Y-Wing", "Kavil", TestName = "Scum Y-Wing")]
		[TestCase ("Z-95 Headhunter", "N'Dru Suhlak", TestName = "Scum Z-95 Headhunter")]
		[TestCase ("Aggressor", "IG88-A", TestName = "Aggressor")]
		[TestCase ("Firespray-31", "Boba Fett", TestName = "Scum Firespray-31")]
		[TestCase ("JumpMaster 5000", "Dengar", TestName = "JumpMaster 5000")]
		[TestCase ("YV-666", "Bossk", TestName = "YV-666")]
		public void ShouldAddShip (string ship, string pilot)
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

