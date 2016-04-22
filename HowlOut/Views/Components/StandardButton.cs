using System;
using Xamarin.Forms;

namespace HowlOut
{
	public class StandardButton
	{
		public StandardButton ()
		{
			
		}

		public Grid StandardButtonGrid(StandardButtonType type, string text, int i)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = 1000,
				HeightRequest = 1000,
				ColumnDefinitions = {
					
					new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
					new ColumnDefinition{ Width = 16},
					new ColumnDefinition{ Width = 2},
				},
				RowDefinitions = {
					new RowDefinition{ Height = 2 },
					new RowDefinition{ Height = 16 },
					new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},

				},
				Padding = 0,
				RowSpacing = 0,
				ColumnSpacing = 0,
			};

			Button standardButton = new Button {
				BorderRadius = 0,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Text = text,
				TextColor = Color.Black,
				BorderColor = Color.FromHex("ffececec"),
				FontSize = 14,
				BorderWidth = 0,
			};

			if (type == StandardButtonType.WithCorners) {
				standardButton.BorderWidth = 1;
				standardButton.BorderRadius = 4;
			} else if (type == StandardButtonType.Transparent) {
				standardButton.BackgroundColor = Color.Transparent;
				standardButton.TextColor = Color.Transparent;
			}


			cellGrid.Children.Add (standardButton, 0,3,0,3);

			if (i > 0) {
				Button notificationButton = new Button {
					HeightRequest = 16,
					WidthRequest = 16,
					BorderRadius = 8,
					BackgroundColor = Color.Red,
					HorizontalOptions = LayoutOptions.Center,
				};
				cellGrid.Children.Add (notificationButton, 1, 2, 1, 2);

				Label newLabel = new Label {
					Text = "" + i,
					TextColor = Color.White,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					BackgroundColor = Color.Transparent,
					FontSize = 11,
				};
				cellGrid.Children.Add (newLabel, 1, 2, 1, 2);
			}

			return cellGrid;
		}

		public enum StandardButtonType {
			WithCorners,
			Plain,
			Transparent
		}
	}
}

