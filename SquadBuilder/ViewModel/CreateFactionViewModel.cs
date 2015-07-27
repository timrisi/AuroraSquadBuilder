using System;
using XLabs;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SquadBuilder
{
	public class CreateFactionViewModel : MainViewModel
	{
		string name;
		public string Name {
			get { return name; }
			set { SetProperty (ref name, value); }
		}
	
		public ObservableCollection <string> Colors {
			get {
				return new ObservableCollection <string> (nameToColor.Keys);
			}
		}

		int selectedIndex = Device.OnPlatform <int> (8, 14, 8);
		public int SelectedIndex {
			get { return selectedIndex; }
			set { 
				SetProperty (ref selectedIndex, value); 
				SelectedColor = nameToColor [Colors [selectedIndex]];
			}
		}

		Color selectedColor = Device.OnPlatform <Color> (Color.Navy, Color.Teal, Color.Navy);
		public Color SelectedColor {
			get { return selectedColor; }
			set { SetProperty (ref selectedColor, value); }
		}

		Dictionary<string, Color> nameToColor = new Dictionary<string, Color>
		{
			{ "Aqua", Color.Aqua }, { "Black", Color.Black },
			{ "Blue", Color.Blue }, { "Fuschia", Color.Fuchsia },
			{ "Gray", Color.Gray }, { "Green", Color.Green },
			{ "Lime", Color.Lime }, { "Maroon", Color.Maroon },
			{ "Navy", Color.Navy }, { "Olive", Color.Olive },
			{ "Pink", Color.Pink }, { "Purple", Color.Purple }, { "Red", Color.Red },
			{ "Silver", Color.Silver }, { "Teal", Color.Teal },
			{ "White", Color.White }, { "Yellow", Color.Yellow }
		};

		RelayCommand saveFaction;
		public RelayCommand SaveFaction {
			get {
				if (saveFaction == null)
					saveFaction = new RelayCommand (() => {
						if (string.IsNullOrWhiteSpace (name))
							return;

						XElement factionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions.xml")));
						XElement customFactionsXml = XElement.Load (new StringReader (DependencyService.Get <ISaveAndLoad> ().LoadText ("Factions_Custom.xml")));

						if (factionsXml.Descendants ().FirstOrDefault (e => e?.Value == name) != null)
							return;

						if (customFactionsXml.Descendants ().FirstOrDefault (e => e?.Value == name) != null)
							return;

						char[] arr = name.ToCharArray();

						arr = Array.FindAll <char> (arr, (c => (char.IsLetterOrDigit(c))));
						var str = new string(arr);

						var element = new XElement ("Faction", name);
						element.Add (new XAttribute ("id", str.ToLower ()));
						element.Add (new XElement ("Color", 
							new XAttribute ("r", (int)(SelectedColor.R * 255)), 
							new XAttribute ("g", (int)(SelectedColor.G * 255)),
							new XAttribute ("b", (int)(SelectedColor.B * 255))
						));

						customFactionsXml.Add (element);

						DependencyService.Get <ISaveAndLoad> ().SaveText ("Factions_Custom.xml", customFactionsXml.ToString ());

						MessagingCenter.Send <CreateFactionViewModel, Faction> (this, "Faction Created", new Faction { Id = str.ToLower (), Name = name, Color = SelectedColor });
					});

				return saveFaction;
			}
		}
	}
}

