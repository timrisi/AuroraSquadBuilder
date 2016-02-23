using System;
using XLabs.Forms.Mvvm;
using XLabs;
using Xamarin.Forms;
using Dropbox.Api;
using System.Linq;
using Dropbox.Api.Files;
using System.IO;
using System.Text;
using Dropbox.Api.Sharing;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Xamarin.Auth;
using System.Security.Cryptography;
using Xamarin.Forms;
using System.Threading.Tasks;

#if __IOS__
using UIKit;
#elif __ANDROID__
using Android.Content;
using Android.App;
#endif

namespace SquadBuilder
{
	public class SettingsViewModel : ViewModel
	{
		public const string AccountKey = "AccountKey";
		public const string ParentRevisionKey = "ParentRevision";
		public const string ParentModifiedDateKey = "ParentModifiedDate";
		public const string ModifiedDateKey = "ModifiedDate";
		public const string AccessTokenKey = "AccessTokenKey";

		public SettingsViewModel ()
		{
			MessagingCenter.Subscribe <SettingsViewModel> (this, "Logged in", vm => NotifyPropertyChanged ("Account"));
		}

		public bool AllowCustom {
			get { return Settings.AllowCustom; }
			set {
				Settings.AllowCustom = value;
				NotifyPropertyChanged ("AllowCustom");
				MessagingCenter.Send <SettingsViewModel> (this, "AllowCustom changed");
			}
		}
				
		public bool FilterPilotsByShip {
			get { return Settings.FilterPilotsByShip; }
			set {
				Settings.FilterPilotsByShip = value;
				NotifyPropertyChanged ("FilterPilotsByShip");
			}
		}

		public bool UpdateOnLaunch {
			get { return Settings.UpdateOnLaunch; }
			set {
				Settings.UpdateOnLaunch = value;
				NotifyPropertyChanged ("UpdateOnLaunch");
			}
		}

		public bool HideUnavailable {
			get { return Settings.HideUnavailable; }
			set {
				Settings.HideUnavailable = value;
				NotifyPropertyChanged ("HideUnavailable");
			}
		}

		public bool DropboxSync {
			get { return Settings.DropboxSync; }
			set {
				Settings.DropboxSync = value;
				NotifyPropertyChanged ("DropboxSync");

				if (value)
					DropboxLogin ();
				else {
					App.DropboxClient = null;
					Xamarin.Forms.Application.Current.Properties.Remove (SettingsViewModel.AccessTokenKey);
					Xamarin.Forms.Application.Current.Properties.Remove (AccountKey);
					Xamarin.Forms.Application.Current.SavePropertiesAsync ();
					NotifyPropertyChanged ("Account");
					NotifyPropertyChanged ("DropboxSync");
				}
			}
		}

		public string Account {
			get {
				if (Xamarin.Forms.Application.Current.Properties.ContainsKey (AccountKey))
					return Xamarin.Forms.Application.Current.Properties [AccountKey].ToString ();

				return "";
			}
		}

		RelayCommand checkForUpdates;
		public RelayCommand CheckForUpdates {
			get {
				if (checkForUpdates == null)
					checkForUpdates = new RelayCommand (() => {
						Settings.CheckForUpdates ();
					});	

				return checkForUpdates;
			}
		}

		public void DropboxLogin ()
		{
			if (App.DropboxClient != null)
				return;

			var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri ("qms26ynz79cou3i");
			Console.WriteLine (authorizeUri.ToString ());
			var auth = new OAuth2Authenticator (
				"qms26ynz79cou3i",
				"",
				new Uri ("https://www.dropbox.com/1/oauth2/authorize"),
				new Uri ("https://www.dropbox.com"));

			auth.AllowCancel = true;
			auth.Completed += async (object sender, AuthenticatorCompletedEventArgs e) => {
				if (e.IsAuthenticated) {
					var token = e.Account.Properties ["access_token"];
					Xamarin.Forms.Application.Current.Properties [AccessTokenKey] = token;
					Xamarin.Forms.Application.Current.SavePropertiesAsync ();
					App.DropboxClient = new DropboxClient (token);
					var userAccount = await App.DropboxClient.Users.GetCurrentAccountAsync ();
					Xamarin.Forms.Application.Current.Properties [SettingsViewModel.AccountKey] = userAccount.Name.DisplayName;
					Xamarin.Forms.Application.Current.SavePropertiesAsync ();
					MessagingCenter.Send <SettingsViewModel> (this, "Logged in");
				}

#if __IOS__
				UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController (false, null);
#elif __ANDROID__

#endif

				await SyncDropbox ();
			};

#if __IOS__
			UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController (auth.GetUI (), false, null);
#elif __ANDROID__
			var activity = Forms.Context as Activity;
			activity.StartActivity (auth.GetUI (activity));
#endif
		}

		public static async Task SyncDropbox ()
		{
			var client = App.DropboxClient;

			var list = await client.Files.ListFolderAsync("");

			var file = list.Entries.Where (i => i.IsFile).FirstOrDefault (f => f.Name == Cards.SquadronsFilename);

			if (file == null) {
				await SaveToDropbox ();
				return;
			}

			try {
				var metadata = (await client.Files.GetMetadataAsync ("/" + Cards.SquadronsFilename)).AsFile;

				if (Cards.SharedInstance.Squadrons.Count > 0) {
					if (!Xamarin.Forms.Application.Current.Properties.ContainsKey (ParentRevisionKey))
						throw new Exception ("Dropbox conflict");

					if (Xamarin.Forms.Application.Current.Properties [ParentRevisionKey].ToString () == metadata.Rev)
						return;

					if ((DateTime)Xamarin.Forms.Application.Current.Properties [ModifiedDateKey] > (DateTime)Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey])
						throw new Exception ("Dropbox conflict");
				}

				using (var fileMetadata = await client.Files.DownloadAsync ("/" + Cards.SquadronsFilename)) {
					var squadronsXml = await fileMetadata.GetContentAsStringAsync ();
					DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.SquadronsFilename, squadronsXml);
					Cards.SharedInstance.GetAllSquadrons ();
					Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = fileMetadata.Response.Rev;
					Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = fileMetadata.Response.ServerModified.ToLocalTime ();
					Xamarin.Forms.Application.Current.Properties [ModifiedDateKey] = fileMetadata.Response.ServerModified.ToLocalTime ();
					Xamarin.Forms.Application.Current.SavePropertiesAsync ();
				}
			} catch (Exception e) {
				if (e.Message != "Dropbox conflict")
					throw e;

				await ResolveDropboxConflict ();
			}
		}

		public static async Task SaveToDropbox ()
		{
			if (Cards.SharedInstance.Squadrons.Count == 0)
				return;

			var client = App.DropboxClient;

			var list = await client.Files.ListFolderAsync("");

			var file = list.Entries.Where (i => i.IsFile).FirstOrDefault (f => f.Name == Cards.SquadronsFilename);

			if (file == null) {
				var updated = await client.Files.UploadAsync (
					"/" + Cards.SquadronsFilename,
					WriteMode.Add.Instance,
					body: new MemoryStream (Encoding.UTF8.GetBytes (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.SquadronsFilename))));

				Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = updated.Rev;
				Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = updated.ServerModified.ToLocalTime ();
				Xamarin.Forms.Application.Current.SavePropertiesAsync ();
				return;
			}

			if (!Xamarin.Forms.Application.Current.Properties.ContainsKey (ParentRevisionKey) || Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] == null) {
				await ResolveDropboxConflict ();
				return;
			}

			var metadata = (await client.Files.GetMetadataAsync ("/" + Cards.SquadronsFilename)).AsFile;

			if (Xamarin.Forms.Application.Current.Properties [ParentRevisionKey].ToString () == metadata.Rev) {
				if ((DateTime) Xamarin.Forms.Application.Current.Properties [ModifiedDateKey] <= metadata.ServerModified.ToLocalTime ())
					return;

				var updated = await client.Files.UploadAsync (
					"/" + Cards.SquadronsFilename,
					new WriteMode.Update (Xamarin.Forms.Application.Current.Properties [ParentRevisionKey].ToString ()),
					body: new MemoryStream (Encoding.UTF8.GetBytes (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.SquadronsFilename))));

				Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = updated.Rev;
				Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = updated.ServerModified.ToLocalTime ();
				Xamarin.Forms.Application.Current.SavePropertiesAsync ();
				return;
			}

			if ((DateTime) Xamarin.Forms.Application.Current.Properties [ModifiedDateKey] > (DateTime) Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey]) {
				await ResolveDropboxConflict ();
				return;
			}
		}

		public static async Task ResolveDropboxConflict ()
		{
			var client = App.DropboxClient;

			var response = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet ("Syncing Conflict between local and dropbox squadrons.  Which version would you like to use?  Selecting local will overwrite the version in dropbox.  Selecting dropbox will overwrite the local squadrons.  Selecting Merge will combine the two.", null, null, "Local", "Dropbox", "Merge");
			switch (response) {
			case "Local":
				var updated = await client.Files.UploadAsync (
					"/" + Cards.SquadronsFilename,
					WriteMode.Overwrite.Instance,
					body: new MemoryStream (Encoding.UTF8.GetBytes (DependencyService.Get <ISaveAndLoad> ().LoadText (Cards.SquadronsFilename))));

				Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = updated.Rev;
				Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = updated.ServerModified.ToLocalTime ();
				break;
			case "Dropbox":
				var fileMetadata = await client.Files.DownloadAsync ("/" + Cards.SquadronsFilename);
				var serializedXml = await fileMetadata.GetContentAsStringAsync ();

				DependencyService.Get <ISaveAndLoad> ().SaveText (Cards.SquadronsFilename, serializedXml);
				Xamarin.Forms.Application.Current.Properties [ModifiedDateKey] = fileMetadata.Response.ServerModified.ToLocalTime ();
				Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = fileMetadata.Response.Rev;
				Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = fileMetadata.Response.ServerModified.ToLocalTime ();
				Xamarin.Forms.Application.Current.SavePropertiesAsync ();

				break;
			case "Merge":
				var squadronMetadata = await client.Files.DownloadAsync ("/" + Cards.SquadronsFilename);
				var squadronXml = await squadronMetadata.GetContentAsStringAsync ();

				var serializer = new XmlSerializer (typeof(ObservableCollection <Squadron>));

				using (TextReader reader = new StringReader (squadronXml)) {
					var squads = (ObservableCollection <Squadron>)serializer.Deserialize (reader);

					foreach (var squad in squads) {
						squad.Faction = Cards.SharedInstance.AllFactions.FirstOrDefault (f => f.Id == squad.Faction?.Id);

						foreach (var pilot in squad.Pilots) {
							foreach (var upgrade in pilot.UpgradesEquipped) {
								if (upgrade == null)
									continue;

								if (upgrade.CategoryId == null)
									upgrade.CategoryId = Cards.SharedInstance.AllUpgrades.FirstOrDefault (u => u.Id == upgrade.Id && u.Category == upgrade.Category)?.CategoryId;
							}
						}
					}

					var mergedSquads = Cards.SharedInstance.Squadrons.Union (squads);

					Cards.SharedInstance.Squadrons = new ObservableCollection <Squadron> (mergedSquads);

					Xamarin.Forms.Application.Current.Properties [ParentRevisionKey] = squadronMetadata.Response.Rev;
					Xamarin.Forms.Application.Current.Properties [ParentModifiedDateKey] = squadronMetadata.Response.ServerModified.ToLocalTime ();
					Xamarin.Forms.Application.Current.SavePropertiesAsync ();

					await Cards.SharedInstance.SaveSquadrons ();
				}
				break;
			}
		}
	}
}