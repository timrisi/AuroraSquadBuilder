using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Threading;

namespace SquadBuilder.Tests
{
	[TestFixture (Platform.Android)]
	[TestFixture (Platform.iOS)]
	public class BugFixTests
	{
		IApp app;
		Platform platform;

		public BugFixTests (Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest ()
		{
			app = AppInitializer.StartApp (platform);
		}

		[Test]
		public void ShouldSelectSameShipAfterNotSelectingPilot ()
		{
			app.Tap ("+");

			app.Tap ("Save");
			app.WaitForElement ("Export to Clipboard", "Squadron creation failed");

			Assert.IsNotEmpty (app.Query ("0/100"));

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));

			app.Tap ("A-Wing");
			app.Tap ("Ships");
			app.Tap ("A-Wing");
			         
			Thread.Sleep (500);
			Assert.IsNotEmpty (app.Query ("Pilots"));
		}

		[Test]
		public void ShouldNotCrashWhenSelectingPilotAfterBackingCompletelyOut ()
		{
			if (platform == Platform.iOS)
				app.Tap ("Menu");
			else
				app.Tap (x => x.Class ("ImageView"));

			app.Tap ("Settings");
			//app.Tap ("FilterSwitch");
			if (platform == Platform.iOS)
				app.Tap (c => c.Marked ("Filter Pilots By Ship").Switch ());
			else
				app.Tap (x => x.Class ("Switch").Index (1));

			if (platform == Platform.iOS)
				app.Tap ("Menu");
			else
				app.Tap (x => x.Class ("ImageView"));

			app.Tap ("Squadrons");

			app.Tap ("+");

			app.Tap ("Save");
			app.WaitForElement ("Export to Clipboard", "Squadron creation failed");

			Assert.IsNotEmpty (app.Query ("0/100"));

			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));
			app.Back ();
			app.Back ();
			app.Tap ("Rebel");
			if (platform == Platform.iOS)
				app.Tap (x => x.Class ("UINavigationButton"));
			else
				app.Tap (c => c.Class ("ActionMenuItemview"));
			Thread.Sleep (500);
			app.Tap ("Tycho Celchu");

			app.WaitForElement ("Export to Clipboard", "Squadron creation failed");
			Assert.IsNotEmpty (app.Query ("Tycho Celchu"));
		}
	}
}