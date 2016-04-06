using System;

namespace SquadBuilder
{
	public interface ISaveAndLoad {
		bool FileExists (string filename);
		void SaveText (string filename, string text);
		string LoadText (string filename);
		void DeleteFile (string filename);
		string GetPath (string filename);
	}
}