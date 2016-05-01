using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Threading;
using Xamarin.UITest.iOS;

namespace SquadBuilder.Tests
{
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class SquadronTests
	{
		IApp app;
		Platform platform;

		public SquadronTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);
		}

		[Test]
		public void ShouldCreateUnnamedRebelSquadron ()
		{
			createSquadron ("Rebel", index: 0);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ("Rebel"));
		}

		[Test]
		public void ShouldCreateUnnamedImperialSquadron ()
		{
			createSquadron ("Imperial", index: 1);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ("Imperial"));
		}

		[Test]
		public void ShouldCreateUnnamedScumSquadron ()
		{
			createSquadron ("Scum & Villainy", index: 2);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ("Scum & Villainy"));
		}

		[Test]
		public void ShouldCreateUnnamedMixedSquadron ()
		{
			createSquadron ("Mixed", index: 3);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ("Mixed"));
		}

		[Test]
		public void ShouldCreateNamedSquadron ()
		{
			string name = "Foo";

			createSquadron (name: name);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query (name));
		}

		[Test]
		public void ShouldCreate150PointSquadron ()
		{
			int points = 150;

			createSquadron (points: points);

			app.Back ();
			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ($"0 / {points}"));
		}

		[Test]
		public void ShouldDeleteSquadron ()
		{
			createSquadron ("Rebel", index: 0);

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("0 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Rebel");

			app.Tap ("Delete");

			Assert.IsEmpty (app.Query ("Rebel"), "Failed to delete squadron");
		}

		[Test]
		public void ShouldEditSquadronName ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("0 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Edit");
			app.Tap ("NameField");
			app.ClearText ();
			app.EnterText ("Bar");

			app.DismissKeyboard ();
			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsNotEmpty (app.Query ("Bar"), "Failed to change squadron name");
		}

		[Test]
		public void ShouldEditSquadronPoints ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("0 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Edit");
			app.Tap ("PointsField");
			app.ClearText ();
			app.EnterText ("150");

			app.DismissKeyboard ();
			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsNotEmpty (app.Query ("0 / 150"), "Failed to change squadron points");
		}

		[Test]
		public void ShouldEditSquadronFaction ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("0 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Edit");
			app.Tap ("FactionPicker");

			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 1)));
				app.Tap ("OK");
			} else {
				app.Tap ("Imperial");
				app.Tap ("Done");
			}

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsNotEmpty (app.Query ("Imperial"), "Failed to change squadron faction");
		}

		[Test]
		public void ShouldRemoveNonFactionPilotsWhenEditingFaction ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");

			if (platform == Platform.Android && app.Query ("Tycho Celchu").Length > 1)
				app.Tap (x => x.Marked ("Tycho Celchu").Index (1));
			else
				app.Tap ("Tycho Celchu");

			app.WaitForElement ("Export to Clipboard");

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsEmpty (app.Query ("0 / 100"), "Failed to add pilot.");

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("26 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Edit");
			app.Tap ("FactionPicker");

			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 1)));
				app.Tap ("OK");
			} else {
				app.Tap ("Imperial");
				app.Tap ("Done");
			}

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsNotEmpty (app.Query ("0 / 100"), "Failed to remove pilots when changing faction");
		}

		[Test]
		public void ShouldNotRemoveNonFactionPilotsWhenEditingFaction ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");

			if (platform == Platform.Android && app.Query ("Tycho Celchu").Length > 1)
				app.Tap (x => x.Marked ("Tycho Celchu").Index (1));
			else
				app.Tap ("Tycho Celchu");

			app.WaitForElement ("Export to Clipboard");

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsEmpty (app.Query ("0 / 100"), "Failed to add pilot.");

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("26 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Edit");
			app.Tap ("FactionPicker");

			if (platform == Platform.Android) {
				Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", 3)));
				app.Tap ("OK");
			} else {
				app.Tap ("Mixed");
				app.Tap ("Done");
			}

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			Assert.IsNotEmpty (app.Query ("26 / 100"), "Removed pilots when changing to mixed faction.");
		}

		[Test]
		public void ShouldCopySquadron ()
		{
			createSquadron ("Rebel", "Foo", index: 0);

			app.Back ();

			app.WaitForElement ("Squadrons");
			Thread.Sleep (500);

			if (platform == Platform.iOS) {
				var faction = app.Query ("Rebel").First ();
				var points = app.Query ("0 / 100").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, faction.Rect.CenterX, faction.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Foo");

			app.Tap ("Copy");

			Thread.Sleep (250);

			Assert.Greater (app.Query ("Foo").Length, 1, "Failed to copy squadron");
		}

		[Test]
		public void ShouldDeletePilot ()
		{
			createSquadron ("Rebel", index: 0);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");

			if (platform == Platform.Android && app.Query ("Tycho Celchu").Length > 1)
				app.Tap (x => x.Marked ("Tycho Celchu").Index (1));
			else
				app.Tap ("Tycho Celchu");

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("Tycho Celchu"));

			if (platform == Platform.iOS) {
				var name = app.Query ("Tycho Celchu").First ();
				var points = app.Query ("26").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, name.Rect.CenterX, name.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Tycho Celchu");

			app.Tap ("Delete");

			Assert.IsEmpty (app.Query ("Rebel"), "Failed to delete pilot");
		}

		[Test]
		public void ShouldCopyPilot ()
		{
			createSquadron ("Rebel", index: 0);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");

			if (platform == Platform.Android && app.Query ("Tycho Celchu").Length > 1)
				app.Tap (x => x.Marked ("Tycho Celchu").Index (1));
			else
				app.Tap ("Tycho Celchu");

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ("Tycho Celchu"));

			if (platform == Platform.iOS) {
				var name = app.Query ("Tycho Celchu").First ();
				var points = app.Query ("26").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, name.Rect.CenterX, name.Rect.CenterY);
			}
			else
				app.TouchAndHold ("Tycho Celchu");

			app.Tap ("Copy");

			Thread.Sleep (250);

			Assert.Greater (app.Query ("Tycho Celchu").Length, 1, "Failed to copy pilot");
		}

		[Test]
		public void ShouldSearchForPilot ()
		{
			createSquadron ("Rebel", index: 0);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");

			app.Tap ("SearchBar");

			app.EnterText ("prototype");

			Assert.IsNotEmpty (app.Query ("Prototype Pilot"));
		}

		[TestCase ("Rebel", 0, "CR90 Corvette (Fore)", "CR90 Corvette (Aft)", "CR90 Corvette (Fore)")]
		[TestCase ("Imperial", 1, "Raider-class Corvette (Fore)", "Raider-class Corvette (Aft)", "Raider-class Corv. (Fore)")]
		public void ShouldAddBothSectionsForTwoCardEpics (string faction, int index, string firstHalf, string secondHalf, string ship)
		{
			createSquadron (faction, index: index);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			if (!app.Query (ship).Any ())
				app.ScrollDownTo (ship);

			app.Tap (ship);
			app.WaitForElement ("Pilots");

			if (platform == Platform.iOS)
				app.Tap (firstHalf);
			else
				app.Tap (c => c.Marked (firstHalf).Index (app.Query (x => x.Marked (firstHalf)).Length - 1));

			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query (firstHalf));
			Assert.IsNotEmpty (app.Query (secondHalf));
		}

		[TestCase ("Rebel", 0, "CR90 Corvette (Fore)", "CR90 Corvette (Aft)", "CR90 Corvette (Fore)")]
		[TestCase ("Imperial", 1, "Raider-class Corvette (Fore)", "Raider-class Corvette (Aft)", "Raider-class Corv. (Fore)")]
		public void ShouldDeleteBothSectionsForTwoCardEpics (string faction, int index, string firstHalf, string secondHalf, string ship)
		{
			createSquadron (faction, index: index);

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			if (!app.Query (ship).Any ())
				app.ScrollDownTo (ship);

			app.Tap (ship);
			app.WaitForElement ("Pilots");

			if (platform == Platform.iOS)
				app.Tap (firstHalf);
			else
				app.Tap (c => c.Marked (firstHalf).Index (app.Query (x => x.Marked (firstHalf)).Length - 1));

			app.WaitForElement ("Export to Clipboard");

			if (platform == Platform.iOS) {
				var name = app.Query (firstHalf).First ();
				var points = app.Query ("50").First ();

				(app as iOSApp).FlickCoordinates (points.Rect.CenterX, points.Rect.CenterY, name.Rect.CenterX, name.Rect.CenterY);
			}
			else
				app.TouchAndHold (firstHalf);

			app.Tap ("Delete");
			Thread.Sleep (500);

			Assert.IsEmpty (app.Query (firstHalf), "Failed to delete pilot");
			Assert.IsEmpty (app.Query (secondHalf), "Failed to delete pilot");
		}

		void createSquadron (string faction = null, string name = null, int points = 100, int index = 0)
		{
			app.Tap ("+");

			if (name != null) {
				app.Tap ("SquadName");
				app.EnterText ("Foo");
			}

			if (points != 100) {
				app.Tap ("Points");
				app.ClearText ();
				app.EnterText (points.ToString ());
			}

			if (faction != null) {
				app.Tap ("FactionPicker");

				if (platform == Platform.Android) {
					Console.WriteLine (app.Query (x => x.Class ("numberPicker").Invoke ("setValue", index)));
					app.Tap ("OK");
				} else {
					app.Tap (faction);
					app.Tap ("Done");
				}
			}

			app.Tap ("Save");
			app.WaitForElement ("Export to Clipboard");

			Assert.IsNotEmpty (app.Query ($"0/{points}"));
				
			if (name != null)
				Assert.IsNotEmpty (app.Query (name));
		}
	}
}

