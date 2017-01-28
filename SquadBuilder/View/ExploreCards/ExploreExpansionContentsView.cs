using System;
using XLabs.Forms.Mvvm;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace SquadBuilder
{
	public class ExploreExpansionContentsView : BaseView
	{
		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var context = BindingContext as ExploreExpansionContentsViewModel;

			var table = new TableView {
				Root = new TableRoot (),
				HasUnevenRows = true,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Intent = TableIntent.Data
			};

#region Ships
			var ships = new TableSection ("Ships");
			foreach (var ship in context.Ships) {
				var cell = new ViewCell ();

				var internalLayout = new StackLayout {
					Orientation = StackOrientation.Horizontal
				};

				internalLayout.Children.Add (new Label {
					Text = ship.Name,
					TextColor = ship.TextColor
				});
				internalLayout.Children.Add (new Label {
					Text = "Preview",
					IsVisible = ship.IsPreview,
					TextColor = Color.Blue,
					FontAttributes = FontAttributes.Italic
				});
				internalLayout.Children.Add (new Label {
					Text = "Custom",
					IsVisible = ship.IsCustom,
					TextColor = Color.Fuchsia,
					FontAttributes = FontAttributes.Italic
				});
				internalLayout.Children.Add (new Label {
					Text = "CCL",
					IsVisible = ship.CCL,
					TextColor = Color.Fuchsia,
					FontAttributes = FontAttributes.Italic
				});

				var stackLayout = new StackLayout {
					Padding = new Thickness (15, 5),
					Spacing = 0.0,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand
				};

				stackLayout.Children.Add (internalLayout);
				stackLayout.Children.Add (new Label {
					Text = ship.ActionsString,
					FontSize = Device.GetNamedSize (NamedSize.Micro, typeof (Label)),
					TextColor = Color.Navy
				});

				cell.View = stackLayout;

				ships.Add (cell);
			}

			table.Root.Add (ships);
#endregion

#region Pilots
			var pilotsSection = new TableSection ("Pilots");
			var pilots = new List<Pilot> ();
			foreach (var pilot in (BindingContext as ExploreExpansionContentsViewModel).Pilots)
				pilots.Add (pilot);

			pilots.OrderByDescending (p => p.PilotSkill).OrderBy (p => p.Ship);

			foreach (var pilot in pilots) {
				var cell = new TextCell {
					Text = pilot.Name,
					Detail = pilot.Ship?.Name
				};
				//var cell = new ViewCell ();

				//var stack1 = new StackLayout {
				//	Orientation = StackOrientation.Horizontal,
				//	HorizontalOptions = LayoutOptions.FillAndExpand
				//};
				//stack1.Children.Add (new Label {
				//	Text = pilot.Name,
				//	HorizontalOptions = LayoutOptions.StartAndExpand,
				//	HorizontalTextAlignment = TextAlignment.Start
				//});
				//stack1.Children.Add (new Label {
				//	Text = pilot.Cost.ToString (),
				//	HorizontalOptions = LayoutOptions.EndAndExpand,
				//	HorizontalTextAlignment = TextAlignment.End
				//});

				//var stack2 = new StackLayout {
				//	Orientation = StackOrientation.Horizontal,
				//	Spacing = 15.0
				//};
				//stack2.Children.Add (new Label {
				//	Text = pilot.PilotSkill.ToString (),
				//	TextColor = Color.FromHex ("#F60"),
				//	FontSize = Device.GetNamedSize (NamedSize.Large, typeof (Label)),
				//	FontAttributes = FontAttributes.Bold
				//});
				//stack2.Children.Add (new Label {
				//	Text = pilot.Attack.ToString (),
				//	TextColor = Color.Red,
				//	FontSize = Device.GetNamedSize (NamedSize.Large, typeof (Label)),
				//	FontAttributes = FontAttributes.Bold
				//});
				//stack2.Children.Add (new Label {
				//	Text = pilot.Agility.ToString (),
				//	TextColor = Color.Green,
				//	FontSize = Device.GetNamedSize (NamedSize.Large, typeof (Label)),
				//	FontAttributes = FontAttributes.Bold
				//});
				//stack2.Children.Add (new Label {
				//	Text = pilot.Hull.ToString (),
				//	TextColor = Color.FromHex ("#FC0"),
				//	FontSize = Device.GetNamedSize (NamedSize.Large, typeof (Label)),
				//	FontAttributes = FontAttributes.Bold
				//});
				//stack2.Children.Add (new Label {
				//	Text = pilot.Shields.ToString (),
				//	TextColor = Color.Blue,
				//	FontSize = Device.GetNamedSize (NamedSize.Large, typeof (Label)),
				//	FontAttributes = FontAttributes.Bold
				//});

				//var stack3 = new StackLayout {
				//	Orientation = StackOrientation.Horizontal,
				//	Spacing = 10.0
				//};
				//stack3.Children.Add (new Label {
				//	Text = "Unique",
				//	FontAttributes = FontAttributes.Italic,
				//	IsVisible = pilot.Unique
				//});
				//stack3.Children.Add (new Label {
				//	Text = "Preview",
				//	IsVisible = pilot.Preview,
				//	TextColor = Color.Blue, 
				//	FontAttributes = FontAttributes.Italic
				//});
				//stack3.Children.Add (new Label {
				//	Text = "Custom",
				//	IsVisible = pilot.IsCustom,
				//	TextColor = Color.Fuchsia,
				//	FontAttributes = FontAttributes.Italic
				//});
				//stack3.Children.Add (new Label {
				//	Text = "CCL",
				//	IsVisible = pilot.CCL,
				//	TextColor = Color.Fuchsia,
				//	FontAttributes = FontAttributes.Italic
				//});

				//var stack4 = new StackLayout {
				//	Orientation = StackOrientation.Horizontal,
				//	Spacing = 10.0
				//};
				//stack4.Children.Add (new Label {
				//	Text = "Expansions",
				//	FontAttributes = FontAttributes.Bold,
				//	MinimumWidthRequest = 100
				//});
				//stack4.Children.Add (new Label {
				//	Text = pilot.Expansions
				//});

				//var stackLayout = new StackLayout {
				//	VerticalOptions = LayoutOptions.FillAndExpand,
				//	Padding = new Thickness (15, 10, 15, 10)
				//};
				//stackLayout.Children.Add (stack1);
				//stackLayout.Children.Add (stack2);
				//stackLayout.Children.Add (stack3);
				//stackLayout.Children.Add (new Label {
				//	Text = pilot.Ability,
				//	FontSize = Device.GetNamedSize (NamedSize.Small, typeof (Label)),
				//	TextColor = pilot.AbilityColor
				//});
				//stackLayout.Children.Add (new Label {
				//	Text = pilot.UpgradeTypesString,
				//	FontSize = Device.GetNamedSize (NamedSize.Small, typeof (Label))
				//});
				//stackLayout.Children.Add (stack4);

				//cell.View = stackLayout;
				cell.Tapped += (sender, e) => {
					context.SelectedPilot = pilot;
				};
				pilotsSection.Add (cell);
			}

			table.Root.Add (pilotsSection);
#endregion

			var upgradesSection = new TableSection ("Upgrades");
			foreach (var upgrade in (BindingContext as ExploreExpansionContentsViewModel).Upgrades) {
				var cell = new TextCell { Text = upgrade.Name, Detail = upgrade.Category };
				cell.Tapped += (sender, e) => {
					context.SelectedUpgrade = upgrade;
				};
				upgradesSection.Add (cell);
			}

			table.Root.Add (upgradesSection);

			Content = table;
		}
	}
}

