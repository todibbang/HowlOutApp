using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;

namespace HowlOut
{
	public partial class UserProfile : ContentView
	{
		Profile userProfile = App.userProfile;
		DataManager dataManager = new DataManager();

		List <Button> friendButtons = new List <Button>();

		public UserProfile ()
		{
			InitializeComponent ();

			Likes.Text = userProfile.Likes + "";
			Loyalty.Text = userProfile.LoyaltyRating + "";
			NameAndAge.Text = userProfile.Name + ", " + userProfile.Age;


			var profilePicUri = dataManager.GetFacebookProfileImageUri(userProfile.ProfileId);
			Image.Source = ImageSource.FromUri(profilePicUri);

			friendsGrid.IsVisible = true;
			groupsGrid.IsVisible = false;

			friendsButton.Clicked  += (sender, e) => {
				friendsGrid.IsVisible = true;
				groupsGrid.IsVisible = false;
			};

			groupsButton.Clicked  += (sender, e) => {
				friendsGrid.IsVisible = false;
				groupsGrid.IsVisible = true;
			};

			friendsList (userProfile.Friends);
		}


		private void friendsList(List<Profile> profiles)
		{
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
					friendsGrid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
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
				friendsGrid.Children.Add (cellGrid, column, row);

				//Adds a cell button 
				friendButtons.Add (new Button {
					Text = "" + i,
					TextColor = Color.Transparent,
					BackgroundColor = Color.Transparent,
				});
				friendsGrid.Children.Add (friendButtons [i], column, row);

				column ++;
			}

			friendsGrid = newGrid;

			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					App.coreView.setContentView( new InspectProfile (profiles[int.Parse(button.Text)]), "InspectProfile");
				};
			}
		}
	}
}

