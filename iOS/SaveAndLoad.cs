﻿using System.Runtime.CompilerServices;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.iOS.SaveAndLoad))]
namespace SquadBuilder.iOS {
	public class SaveAndLoad : ISaveAndLoad {
		public bool FileExists (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			return File.Exists (filePath);
		}

		public void SaveText (string filename, string text) 
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			File.WriteAllText (filePath, text);
		}

		public string LoadText (string filename) 
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			return File.ReadAllText (filePath);
		}

		public void DeleteFile (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			if (File.Exists (filePath))
				File.Delete (filePath);
		}

		public string GetPath (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return Path.Combine (documentsPath, filename);
		}
	}
}