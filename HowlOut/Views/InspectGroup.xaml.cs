using System;
using System.Collections.Generic;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InspectGroup : ContentView
	{
		List <Button> profileButtons = new List <Button>();
		DataManager dataManager = new DataManager();


		public InspectGroup (List<Profile> profiles)
		{
			

			InitializeComponent ();

			Grid newGrid = new Grid {
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				},
				RowDefinitions = {
					new RowDefinition{ Height = 150 },
					new RowDefinition{ Height = 150 },
					new RowDefinition{ Height = 150 }
				},
				RowSpacing=0,
				ColumnSpacing=0,
					
			};

			int column = 0;
			int row = 0;

			for (int i = 0; i < profiles.Count; i++) {
			//for (int i = 0; i < 24; i++) {

				if (column == 3) {
					column = 0;
					row++;
					profileGrid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
				}

				//creates the grid for each cell

				////////////////////
				/// Cell Grid
				////////////////////
				Grid cellGrid = new Grid {
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					RowDefinitions = {
						new RowDefinition{ Height = 10 },
						new RowDefinition{ Height = 85 },
						new RowDefinition{ Height = 40 },
					}
				};

				//adds the users profile picture
				var profilePicUri = dataManager.GetFacebookProfileImageUri(profiles[i].ProfileId);
				//var profilePicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
				CircleImage profilePicture = new CircleImage {
					HeightRequest = 85,
					WidthRequest = 85,
					Aspect = Aspect.AspectFill,
					HorizontalOptions = LayoutOptions.Center,
					Source = ImageSource.FromUri (profilePicUri)
				};
				cellGrid.Children.Add (profilePicture, 0,1);

				cellGrid.Children.Add(new Label {
						Text = profiles[i].Name + ", " + profiles[i].Age,
						TextColor = Color.Black,
						FontSize = 10,
						HorizontalTextAlignment = TextAlignment.Center,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
				}, 0, 2);

				//adds the whole cell to the grid
				profileGrid.Children.Add (cellGrid, column, row);

				//Adds a cell button 
				profileButtons.Add (new Button {
					Text = "" + i,
					TextColor = Color.Transparent,
					BackgroundColor = Color.Transparent,
				});
				profileGrid.Children.Add (profileButtons [i], column, row);

				column ++;
			}

			profileGrid = newGrid;

			foreach (Button button in profileButtons) {
				button.Clicked += (sender, e) => {
					App.coreView.setContentView( new InspectProfile (profiles[int.Parse(button.Text)]), "InspectProfile");
				};
			}
		}
	}
}

