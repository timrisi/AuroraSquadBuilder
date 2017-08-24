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
				stackLayout.Children.Add (new HtmlLabel {
					Text = ship.ActionsString,
					FontSize = Device.GetNamedSize (NamedSize.Small, typeof (Label))
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

