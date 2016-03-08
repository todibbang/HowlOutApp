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



		public void createList(Grid grid, List<Profile> profiles, List<Group> groups, ObservableCollection<Button> buttons, ObservableCollection<Button> acceptButtons, ObservableCollection<Button> declineButtons, Profile userProfile, Button requestButton)
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
				Button acceptButton = new Button ();
				Button declineButton = new Button ();

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
						cell = friendCellCreator (profiles [subjectNr], buttons, acceptButtons, declineButtons);

					} else if (groups != null) {
						cell = groupCellCreator (groups [subjectNr]);
					}
				}
				//adds the whole cell to the grid
				grid.Children.Add (cell, column, row);

				if (addRequestButton) {
					grid.Children.Add (requestButton, column, row);
					addRequestButton = false;
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
			return button;
		}

		private Grid friendCellCreator(Profile profile, ObservableCollection<Button> buttons, ObservableCollection<Button> acceptButtons, ObservableCollection<Button> declineButtons)
		{
			Grid cellGrid = new Grid {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				RowDefinitions = {
					new RowDefinition{ Height = 10 },
					new RowDefinition{ Height = 85 },
					new RowDefinition{ Height = 40 },
					new RowDefinition{ Height = 50 },
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

			Button newProfileButton = new Button ();
			buttons.Add (newProfileButton);
			cellGrid.Children.Add (newProfileButton, 0, 2);

			StackLayout stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal,};

			Button newAcceptButton = new Button {Text = "Accept"};
			buttons.Add (newAcceptButton);
			stackLayout.Children.Add (newAcceptButton);

			Button newDeclineButton = new Button {Text = "Decline"};
			buttons.Add (newDeclineButton);
			stackLayout.Children.Add (newDeclineButton);

			cellGrid.Children.Add (stackLayout, 0, 3);

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