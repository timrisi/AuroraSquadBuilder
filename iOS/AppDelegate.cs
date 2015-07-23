using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Ioc;
using System.IO;
using System.Xml.Linq;

namespace SquadBuilder.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		SaveAndLoad saveAndLoad;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			saveAndLoad = new SaveAndLoad ();
			global::Xamarin.Forms.Forms.Init ();

			if (!Resolver.IsSet)
				SetIoc ();

			var factionsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Factions", "xml")).ReadToEnd ();
			var version = (float)XElement.Load (new StringReader (factionsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Factions.xml") || (float)XElement.Load (saveAndLoad.LoadText ("Factions.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Factions.xml", factionsXml);

			var shipsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Ships", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (shipsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Ships.xml") || (float)XElement.Load (saveAndLoad.LoadText ("Factions.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Ships.xml", shipsXml);

			var pilotsXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Pilots", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (pilotsXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Pilots.xml") || (float)XElement.Load (saveAndLoad.LoadText ("Pilots.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);

			var upgradesXml = new StreamReader (NSBundle.MainBundle.PathForResource ("Upgrades", "xml")).ReadToEnd ();
			version = (float)XElement.Load (new StringReader (upgradesXml)).Attribute ("Version");
			if (!saveAndLoad.FileExists ("Upgrades.xml") || (float)XElement.Load (saveAndLoad.LoadText ("Upgrades.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}

