using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class InspectGroup : ContentView
	{
		List <Button> profileButtons = new List <Button>();

		public InspectGroup ()
		{
			InitializeComponent ();


			Grid newGrid = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				},
				RowDefinitions = {
					new RowDefinition{ Height = 100 },
					new RowDefinition{ Height = 100 }
				}
			};

			int column = 0;
			int row = 0;

			for (int i = 0; i < 6; i++) {

				if (column == 3) {
					column = 0;
					row++;
					profileGrid.RowDefinitions.Add (new RowDefinition{ Height = 100 });
				}

				profileGrid.Children.Add (new Label {
					BackgroundColor = Color.Aqua,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
				}, column, row);

				profileButtons.Add (new Button {
					Text = column+","+row,
					TextColor = Color.Purple,
					BackgroundColor = Color.Transparent,
				});

				profileGrid.Children.Add (profileButtons [i], column, row);

				System.Diagnostics.Debug.WriteLine("new cell: " + column + "," + row );

				column ++;
			}

			profileGrid = newGrid;

			for(int i = 0; i < profileButtons.Count; i++) profileButtons[i].Clicked += (sender, e) => {
				System.Diagnostics.Debug.WriteLine("Button pressed: " + i );
			};
		}
	}
}

