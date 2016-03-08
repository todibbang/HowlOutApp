using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class ListsAndButtons : ContentView
	{
		DataManager dataManager = new DataManager();

		public ListsAndButtons ()
		{
			InitializeComponent ();
		}



		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, ObservableCollection<Button> buttons, Profile userProfile, Button requestButton)
		{
			int count = 0;
			if (profiles != null) {
				count = profiles.Count;
			} else {
				count = groups.Count;
			}

			bool addRequestButton = false;

			if (userProfile != null) {
				if(userProfile.ProfileId == App.userProfile.ProfileId){
					if (profiles != null) {
						if (userProfile.RecievedFriendRequests.Count != 0) {
							count++;
							addRequestButton = true;
						}
					} else if (groups != null) {
						if (userProfile.GroupsInviteTo.Count != 0) {
							count++;
							addRequestButton = true;
						}
					}
				}
			}

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

			int subjectNr = 0;
			int column = 0;
			int row = 0;

			for (int i = 0; i < count; i++) {
				if (column == 3) {
					column = 0;
					row++;
					grid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
				}

				Grid cell = new Grid ();
				Button button = new Button ();

				if (addRequestButton) {
					if (profiles != null) {
						cell = requestGrid(userProfile.RecievedFriendRequests.Count);
						subjectNr--;
					} else if (groups != null) {
						cell = requestGrid (userProfile.GroupsInviteTo.Count);
						subjectNr--;
					}

				} else {
					if (profiles != null) {
						cell = friendCellCreator (profiles [subjectNr]);
						button = buttonCreator (subjectNr);
						buttons.Add (button);

					} else if (groups != null) {
						cell = groupCellCreator (groups [subjectNr]);
						button = buttonCreator (subjectNr);
						buttons.Add (button);
					}
				}
				//adds the whole cell to the grid
				grid.Children.Add (cell, column, row);

				if (addRequestButton) {
					grid.Children.Add (requestButton, column, row);
					addRequestButton = false;
				} else {
					grid.Children.Add (button, column, row);
				}

				column ++;
				subjectNr++;
			}
			grid = newGrid;
		}

		private Button buttonCreator(int i)
		{

			Button button = new Button {
				Text = "" + i,
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,
			};

			if (i == 9999) {
				button.BorderWidth = 6;
				button.BorderColor = Color.Green;
			}
			return button;
		}

		private Grid friendCellCreator(Profile profile)
		{
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
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
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
				Text = profile.Name + ", " + profile.Age,
				TextColor = Color.Black,
				FontSize = 10,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			}, 0, 2);

			return cellGrid;
		}

		private Grid groupCellCreator(Group group)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 85 },
					new RowDefinition{ Height = 40 },
				}
			};

			Button groupButton = new Button {
				HeightRequest = 85,
				WidthRequest = 85,
				BorderRadius = 42,
				BorderWidth = 6,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				Text = group.Members.Count + "",
			};
			cellGrid.Children.Add (groupButton, 0,1);

			cellGrid.Children.Add(new Label {
				Text = group.Name,
				TextColor = Color.Black,
				FontSize = 10,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			}, 0, 2);

			return cellGrid;
		}

		private Grid requestGrid(int i)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 85 },
					new RowDefinition{ Height = 40 },
				}
			};

			Button groupButton = new Button {
				HeightRequest = 85,
				WidthRequest = 85,
				BorderRadius = 42,
				BorderWidth = 6,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				Text = i + "",
				BorderColor = Color.Red
			};

			cellGrid.Children.Add (groupButton, 0,1);

			return cellGrid;
		}
	}
}