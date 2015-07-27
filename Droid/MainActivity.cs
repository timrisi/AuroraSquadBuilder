using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Forms;
using XLabs.Ioc;
using System.IO;
using System.Xml.Linq;
using Xamarin;

namespace SquadBuilder.Droid
{
	[Activity (Label = "Squad Builder", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid
	{
		SaveAndLoad saveAndLoad;

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);

			Insights.Initialize("1396f6a6fc0e812ab8a8d84a01810917fd3940a6", BaseContext);

			saveAndLoad = new SaveAndLoad ();

			global::Xamarin.Forms.Forms.Init(this, bundle);

			if(!Resolver.IsSet) SetIoc();

			var factionsXml = await new StreamReader (Application.Context.Assets.Open ("Factions.xml")).ReadToEndAsync ();
			var version = (float)XElement.Load (new StringReader (factionsXml )).Attribute ("Version");
					
			if (!saveAndLoad.FileExists ("Factions.xml") || (float)XElement.Load (Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Factions.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Factions.xml", factionsXml);

			var shipsXml = await new StreamReader (Application.Context.Assets.Open ("Ships.xml")).ReadToEndAsync ();
			version = (float)XElement.Load (new StringReader (shipsXml )).Attribute ("Version");

			if (!saveAndLoad.FileExists ("Ships.xml") || (float)XElement.Load (Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Ships.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Ships.xml", shipsXml);

			var pilotsXml = await new StreamReader (Application.Context.Assets.Open ("Pilots.xml")).ReadToEndAsync ();
			version = (float)XElement.Load (new StringReader (pilotsXml )).Attribute ("Version");

			if (!saveAndLoad.FileExists ("Pilots.xml") || (float)XElement.Load (Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Pilots.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Pilots.xml", pilotsXml);

			var upgradesXml = await new StreamReader (Application.Context.Assets.Open ("Upgrades.xml")).ReadToEndAsync ();
			version = (float)XElement.Load (new StringReader (upgradesXml )).Attribute ("Version");

			if (!saveAndLoad.FileExists ("Upgrades.xml") || (float)XElement.Load (Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "Upgrades.xml"))?.Attribute ("Version") < version)
				saveAndLoad.SaveText ("Upgrades.xml", upgradesXml);
			
			LoadApplication(new App());
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();
			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}

