using System;
using NUnit.Framework;
using Xamarin.UITest;
using System.Threading;
using Xamarin.UITest.Queries;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;

namespace SquadBuilder.Tests
{
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class UpgradesTests
	{
		const string rebel = "Rebel";
		const int rebelIndex = 0;
		const string imperial = "Imperial";
		const int imperialIndex = 1;
		const string scum = "Scum & Villainy";
		const int scumIndex = 2;

		IApp app;
		Platform platform;

		public UpgradesTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);
		}

		[TestCase ("Astromech Droid", "R2 Astromech", rebel, rebelIndex, "X-Wing", "Wedge Antilles", TestName = "Astromech Droid")]
		[TestCase ("Bomb", "Seismic Charges", rebel, rebelIndex, "K-Wing", "Miranda Doni", TestName = "Bomb")]
		[TestCase ("Cannon", "Flechette Cannon", rebel, rebelIndex, "B-Wing", "Ten Numb", TestName = "Cannon")]
		[TestCase ("Cargo", "Expanded Cargo Hold", rebel, rebelIndex, "GR-75 Medium Transport", "GR-75 Medium Transport", TestName = "Cargo")]
		[TestCase ("Crew", "Intelligence Agent", scum, scumIndex, "G-1A Starfighter", "Zuckuss", TestName = "Crew")]
		[TestCase ("Elite Pilot Talent", "Adrenaline Rush", scum, scumIndex, "G-1A Starfighter", "Zuckuss", TestName = "Elite Pilot Talent")]
		[TestCase ("Hardpoint", "Ion Cannon Battery", rebel, 0, "CR9rebelIndex Corvette (Aft)", "CR90 Corvette (Aft)", TestName = "Hardpoint")]
		[TestCase ("Illicit", "Inertial Dampeners", scum, scumIndex, "G-1A Starfighter", "Zuckuss", TestName = "Illicit")]
		[TestCase ("Missile", "Adv. Homing Missiles", scum, scumIndex, "Kihraxz", "Talonbane Cobra", TestName = "Missile")]
		[TestCase ("Modification", "Experimental Interface", scum, scumIndex, "Kihraxz", "Talonbane Cobra", TestName = "Modification")]
		[TestCase ("Salvaged Astromech", "Unhinged Astromech", scum, scumIndex, "Y-Wing", "Kavil", TestName = "Salvaged Astromech")]
		[TestCase ("System Upgrade", "Fire Control System", scum, scumIndex, "G-1A Starfighter", "Zuckuss", TestName = "System Upgrade")]
		[TestCase ("Team", "Engineering Team", rebel, rebelIndex, "CR90 Corvette (Aft)", "CR90 Corvette (Aft)", TestName = "Team")]
		[TestCase ("Tech", "Weapons Guidance", rebel, rebelIndex, "T-70 X-wing", "Poe Dameron", TestName = "Tech")]
		[TestCase ("Torpedo", "Flechette Torpedoes", scum, scumIndex, "StarViper", "Prince Xizor", TestName = "Torpedo")]
		[TestCase ("Turret Weapon", "Ion Cannon Turret", scum, scumIndex, "HWK-290", "Dace Bonearm", TestName = "Turret Weapon")]
		public void ShouldAddUpgrade (string upgradeType, string upgradeName, string faction, int index, string ship, string pilot)
		{
			createSquadron (faction, index);
			addPilot (ship, pilot);

			app.WaitForElement ("Export to Clipboard");

			app.Tap (pilot);

			if (!app.Query (upgradeType).Any ())
				app.ScrollDownTo (upgradeType);
			
			app.Tap (upgradeType);
			app.Tap (upgradeName);
			app.WaitForElement (pilot);
			Thread.Sleep (150);

			Assert.IsNotEmpty (app.Query (upgradeName));

			app.Tap (upgradeName);
			app.Tap ("None");

			app.WaitForElement (pilot);
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query (upgradeName), $"Failed to remove {upgradeName} upgrade");
			Assert.IsNotEmpty (app.Query (upgradeType));
		}

		[Test]
		public void EmperorShouldUseTwoSlots ()
		{
			createSquadron (imperial, imperialIndex);
			addPilot ("Lambda-Class Shuttle", "Captain Kagi");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Captain Kagi");

			app.Tap ("Crew");
			app.ScrollDownTo ("Emperor Palpatine");
			app.Tap ("Emperor Palpatine");
			app.WaitForElement ("Captain Kagi");
			Thread.Sleep (150);

			Assert.AreEqual (app.Query ("Emperor Palpatine").Length, 2);
			Assert.IsEmpty (app.Query ("Crew"));

			app.Tap ("Emperor Palpatine");
			app.Tap ("None");

			app.WaitForElement ("Captain Kagi");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query ("Emperor Palpatine"), "Failed to remove Emperor Palpatine upgarde");
			Assert.AreEqual (app.Query ("Crew").Length, 2);
		}

		[Test]
		public void RoyalGuardShouldAddModificationSlot ()
		{
			createSquadron (imperial, imperialIndex);
			addPilot ("TIE Interceptor", "Soontir Fel");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Soontir Fel");

			app.Tap ("Title");
			app.Tap ("Royal Guard TIE");
			app.WaitForElement ("Soontir Fel");
			Thread.Sleep (150);

			Assert.AreEqual (app.Query ("Modification").Length, 2);

			app.Tap ("Royal Guard TIE");
			app.Tap ("None");

			app.WaitForElement ("Soontir Fel");
			Thread.Sleep (150);

			Assert.AreEqual (app.Query ("Modification").Length, 1, "Failed to remove extra Modification slot");
		}

		[Test]
		public void AwingTestPilotShouldAddEpt ()
		{
			createSquadron (rebel, rebelIndex);
			addPilot ("A-Wing", "Tycho Celchu");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Tycho Celchu");

			app.Tap ("Title");
			app.Tap ("A-Wing Test Pilot");
			app.WaitForElement ("Tycho Celchu");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.IsNotEmpty (app.Query ("Elite Pilot Talent"));

			app.Tap ("A-Wing Test Pilot");
			app.Tap ("None");

			app.WaitForElement ("Tycho Celchu");
			Thread.Sleep (150);

			Assert.AreEqual (app.Query ("Elite Pilot Talent").Length, 1, "Failed to remove extra Elite Pilot Talent slot");
		}

		[Test]
		public void TIEx1ShouldAddSystemSlot ()
		{
			createSquadron (imperial, imperialIndex);
			addPilot ("TIE Advanced", "Darth Vader");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Darth Vader");

			app.Tap ("Title");
			app.Tap ("TIE/x1");
			app.WaitForElement ("Darth Vader");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.AreEqual (app.Query ("System Upgrade").Length, 1);

			app.Tap ("TIE/x1");
			app.Tap ("None");

			app.WaitForElement ("Darth Vader");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query ("System Upgrade"), "Failed to remove System Upgrade slot");
		}

		[Test]
		public void MistHunterShouldAddTractorBeam ()
		{
			createSquadron (scum, scumIndex);
			addPilot ("G-1A Starfighter", "Zuckuss");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Zuckuss");

			app.Tap ("Title");
			app.Tap ("Mist Hunter");
			app.WaitForElement ("Zuckuss");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.AreEqual (app.Query ("Tractor Beam").Length, 1);

			app.Tap ("Mist Hunter");
			app.Tap ("None");

			app.WaitForElement ("Zuckuss");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query ("Tractor Beam"), "Failed to remove Tractor Beam upgrade");
			Assert.IsEmpty (app.Query ("Cannon"), "Failed to remove Cannon slot");
		}

		[TestCase ("Cannon", TestName = "Cannon")]
		[TestCase ("Torpedo", TestName = "Torpedo")]
		[TestCase ("Missile", TestName = "Missile")]
		public void HeavyScykShouldAddUpgrade (string upgradeType)
		{
			createSquadron (scum, scumIndex);
			addPilot ("M3-A \"Scyk\" Interceptor", "Serissu");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Serissu");

			app.Tap ("Title");
			app.Tap ("\"Heavy Scyk\" Interceptor");
			app.Tap (upgradeType);
			app.WaitForElement ("Serissu");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.AreEqual (app.Query (upgradeType).Length, 1);

			app.Tap ("\"Heavy Scyk\" Interceptor");
			app.Tap ("None");

			app.WaitForElement ("Serissu");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query (upgradeType), $"Failed to remove {upgradeType} slot");
		}

		[Test]
		public void ShouldCancelHeavyScyk ()
		{
			createSquadron (scum, scumIndex);
			addPilot ("M3-A \"Scyk\" Interceptor", "Serissu");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Serissu");

			app.Tap ("Title");
			app.Tap ("\"Heavy Scyk\" Interceptor");
			app.Tap ("Cancel");
			Assert.IsNotEmpty (app.Query ("Title"));
			app.Back ();
			app.WaitForElement ("Serissu");
			Thread.Sleep (150);

			Assert.IsNotEmpty (app.Query ("Title"));
			Assert.IsEmpty (app.Query ("Cannon"));
			Assert.IsEmpty (app.Query ("Torpedo"));
			Assert.IsEmpty (app.Query ("Missile"));
		}

		[Test]
		public void ViragoShouldAddSystemIllicit ()
		{
			createSquadron (scum, scumIndex);
			addPilot ("StarViper", "Prince Xizor");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Prince Xizor");

			app.Tap ("Title");
			app.Tap ("Virago");
			app.WaitForElement ("Prince Xizor");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.AreEqual (app.Query ("System Upgrade").Length, 1);
			Assert.AreEqual (app.Query ("Illicit").Length, 1);

			app.Tap ("Virago");
			app.Tap ("None");

			app.WaitForElement ("Prince Xizor");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query ("System Upgrade"), "Failed to remove System Upgrade slot");
			Assert.IsEmpty (app.Query ("Illicit"), "Failed to remove Illicit slot");
		}

		[Test]
		public void BWingE2ShouldAddCrew ()
		{
			createSquadron (rebel, rebelIndex);
			addPilot ("B-Wing", "Ten Numb");

			app.WaitForElement ("Export to Clipboard");

			app.Tap ("Ten Numb");

			app.Tap ("Modification");
			app.Tap ("B-Wing/E2");
			app.WaitForElement ("Ten Numb");
			Thread.Sleep (150);
			app.ScrollDown ();

			Assert.AreEqual (app.Query ("Crew").Length, 1);

			app.Tap ("B-Wing/E2");
			app.Tap ("None");

			app.WaitForElement ("Ten Numb");
			Thread.Sleep (150);

			Assert.IsEmpty (app.Query ("Crew"), "Failed to remove Crew slot");
		}

		void createSquadron (string faction, int index)
		{
			app.Tap ("+");

			app.Tap ("FactionPicker");

			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", index)));
				app.Tap ("OK");
			} else {
				app.Tap (faction);
				app.Tap ("Done");
			}

			app.Tap ("SaveButton");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("0/100"));
		}

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

