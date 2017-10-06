using System.Runtime.CompilerServices;
using System;
using System.IO;
using Android.App;
using System.Text;

[assembly: Xamarin.Forms.Dependency (typeof (SquadBuilder.Droid.SaveAndLoad))]
namespace SquadBuilder.Droid {
	public class SaveAndLoad : ISaveAndLoad {
		public bool FileExists (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filePath = Path.Combine (documentsPath, filename);
			return File.Exists (filePath);
		}

		public void SaveText (string filename, string text) 
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filePath = Path.Combine (documentsPath, filename);
			File.WriteAllText (filePath, text);
		}

		public string LoadText (string filename) 
		{
			if (!FileExists (filename))
				return null;
			
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filePath = Path.Combine (documentsPath, filename);

			string text = File.ReadAllText (filePath);

			return ReplaceSymbols (text);
		}

		public void DeleteFile (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var filePath = Path.Combine (documentsPath, filename);
			if (File.Exists (filePath))
				File.Delete (filePath);
		}

		public string GetPath (string filename)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			return Path.Combine (documentsPath, filename);
		}

		public string ReplaceSymbols (string text)
		{
			StringBuilder sb = new StringBuilder (text);

			//sb.Replace ("{hit}", "&#60;font family='xwing-miniatures'&#62;d&#60;/font&#62;");
			//sb.Replace ("{crit}", "&#60;font family='xwing-miniatures'&#62;c&#60;/font&#62;");
			//sb.Replace ("\n\n", "&#60;br&#62;&#60;br&#62;");
			//sb.Replace ("{focus}", "&#60;font family='xwing-miniatures'&#62;f&#60;/font&#62;");
			//sb.Replace ("{evade}", "&#60;font family='xwing-miniatures'&#62;e&#60;/font&#62;");
			//sb.Replace ("{astromech droid}", "&#60;font family='xwing-miniatures'&#62;A&#60;/font&#62;");
			//sb.Replace ("{bomb}", "&#60;font family='xwing-miniatures'&#62;B&#60;/font&#62;");
			//sb.Replace ("{cannon}", "&#60;font family='xwing-miniatures'&#62;C&#60;/font&#62;");
			//sb.Replace ("{cargo}", "&#60;font family='xwing-miniatures'&#62;G&#60;/font&#62;");
			//sb.Replace ("{crew}", "&#60;font family='xwing-miniatures'&#62;W&#60;/font&#62;");
			//sb.Replace ("{elite pilot talent}", "&#60;font family='xwing-miniatures'&#62;E&#60;/font&#62;");
			//sb.Replace ("{hardpoint}", "&#60;font family='xwing-miniatures'&#62;H&#60;/font&#62;");
			//sb.Replace ("{illicit}", "&#60;font family='xwing-miniatures'&#62;I&#60;/font&#62;");
			//sb.Replace ("{missile}", "&#60;font family='xwing-miniatures'&#62;M&#60;/font&#62;");
			//sb.Replace ("{modification}", "&#60;font family='xwing-miniatures'&#62;m&#60;/font&#62;");
			//sb.Replace ("{salvaged astromech}", "&#60;font family='xwing-miniatures'&#62;V&#60;/font&#62;");
			//sb.Replace ("{system}", "&#60;font family='xwing-miniatures'&#62;S&#60;/font&#62;");
			//sb.Replace ("{team}", "&#60;font family='xwing-miniatures'&#62;T&#60;/font&#62;");
			//sb.Replace ("{title}", "&#60;font family='xwing-miniatures'&#62;t&#60;/font&#62;");
			//sb.Replace ("{torpedo}", "&#60;font family='xwing-miniatures'&#62;P&#60;/font&#62;");
			//sb.Replace ("{turret}", "&#60;font family='xwing-miniatures'&#62;U&#60;/font&#62;");
			//sb.Replace ("{tech}", "&#60;font family='xwing-miniatures'&#62;X&#60;/font&#62;");
			//sb.Replace ("{turn left}", "&#60;font family='xwing-miniatures'&#62;4&#60;/font&#62;");
			//sb.Replace ("{turn right}", "&#60;font family='xwing-miniatures'&#62;6&#60;/font&#62;");
			//sb.Replace ("{bank left}", "&#60;font family='xwing-miniatures'&#62;7&#60;/font&#62;");
			//sb.Replace ("{bank right}", "&#60;font family='xwing-miniatures'&#62;9&#60;/font&#62;");
			//sb.Replace ("{straight}", "&#60;font family='xwing-miniatures'&#62;8&#60;/font&#62;");
			//sb.Replace ("{sloop left}", "&#60;font family='xwing-miniatures'&#62;1&#60;/font&#62;");
			//sb.Replace ("{sloop right}", "&#60;font family='xwing-miniatures'&#62;3&#60;/font&#62;");
			//sb.Replace ("{troll left}", "&#60;font family='xwing-miniatures'&#62;&#58;&#60;/font&#62;");
			//sb.Replace ("{troll right}", "&#60;font family='xwing-miniatures'&#62;&#59;&#60;/font&#62;");
			//sb.Replace ("{kturn}", "&#60;font family='xwing-miniatures'&#62;2&#60;/font&#62;");
			//sb.Replace ("{boost}", "&#60;font family='xwing-miniatures'&#62;b&#60;/font&#62;");
			//sb.Replace ("{barrel roll}", "&#60;font family='xwing-miniatures'&#62;r&#60;/font&#62;");
			//sb.Replace ("{target lock}", "&#60;font family='xwing-miniatures'&#62;l&#60;/font&#62;");
			//sb.Replace ("{cloak}", "&#60;font family='xwing-miniatures'&#62;k&#60;/font&#62;");
			//sb.Replace ("{slam}", "&#60;font family='xwing-miniatures'&#62;s&#60;/font&#62;");
			//sb.Replace ("{jam}", "&#60;font family='xwing-miniatures'&#62;j&#60;/font&#62;");
			//sb.Replace ("{stop}", "&#60;font family='xwing-miniatures'&#62;5&#60;/font&#62;");
			//sb.Replace ("Action:", "&#60;b&#62;Action:&#60;/b&#62;");
			//sb.Replace ("Attack:", "&#60;b&#62;Attack:&#60;/b&#62;");
			//sb.Replace ("Attack (Target Lock):", "&#60;b&#62;Attack (Target Lock):&#60;/b&#62;");
			//sb.Replace ("Attack (Focus):", "&#60;b&#62;Attack (Focus):&#60;/b&#62;");
			//sb.Replace ("detonates", "&#60;b&#62;detonates&#60;/b&#62;");
			//sb.Replace ("Detonation:", "&#60;b&#62;Detonation:&#60;/b&#62;");
			//sb.Replace ("font face=", "font family=");
			return sb.ToString ();
		}
	}
}