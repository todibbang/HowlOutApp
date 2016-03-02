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
		List <Button> groupButtons = new List <Button>();

		List <Profile> friendsToInvite = new List <Profile>();
		List <Group> groupsToInvite = new List <Group>();

		Event eventBeingInvitedTo = new Event();
		public CreateEvent createEventView;

		public UserProfile (bool InviteMode, Event eve)
		{
			InitializeComponent ();

			Likes.Text = userProfile.Likes + "";
			Loyalty.Text = userProfile.LoyaltyRating + "";
			NameAndAge.Text = userProfile.Name + ", " + userProfile.Age;


			var profilePicUri = dataManager.GetFacebookProfileImageUri(userProfile.ProfileId);
			Image.Source = ImageSource.FromUri(profilePicUri);

			if (InviteMode) {
				userProfileInfo.IsVisible = false;
				eventBeingInvitedTo = eve;
			} else {
				inviteLayout.IsVisible = false;
			}

			viewFriendsGrid.IsVisible = true;
			viewGroupsGrid.IsVisible = false;

			friendsButton.Clicked  += (sender, e) => {
				viewFriendsGrid.IsVisible = true;
				viewGroupsGrid.IsVisible = false;
			};

			groupsButton.Clicked  += (sender, e) => {
				viewFriendsGrid.IsVisible = false;
				viewGroupsGrid.IsVisible = true;
			};

			createList (userProfile.Friends , null, viewFriendsGrid);
			createList (null, userProfile.Groups, viewGroupsGrid);

			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					if(InviteMode){
						friendsToInvite.Add(userProfile.Friends[int.Parse(button.Text)]);
					}
					else {
						App.coreView.setContentView( new InspectProfile (userProfile.Friends[int.Parse(button.Text)]), "InspectProfile");
					}
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					if(InviteMode){
						groupsToInvite.Add(userProfile.Groups[int.Parse(button.Text)]);
					}
					else {
						App.coreView.setContentView( new InspectGroup (userProfile.Groups[int.Parse(button.Text)].Members ), "InspectGroup");
					}
				};
			}

			inviteButton.Clicked += (sender, e) => {
				sendEventInvites();
			};
		}


		private void createList(List<Profile> profiles, List<Group> groups, Grid grid)
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
				if (column == 3) {
					column = 0;
					row++;
					grid.RowDefinitions.Add (new RowDefinition{ Height = 150 });
				}

				Grid cell = new Grid ();
				Button button = new Button ();

				if (profiles != null) {
					cell = friendListCreator (profiles [i]);
					button = buttonCreator (i);
					friendButtons.Add (button);

				} else if (groups != null) {
					cell = groupListCreator (groups [i]);
					button = buttonCreator (i);
					groupButtons.Add (button);
				}
				//adds the whole cell to the grid
				grid.Children.Add (cell, column, row);
				grid.Children.Add (button, column, row);

				column ++;
			}

			grid = newGrid;
		}

		private Grid friendListCreator(Profile profile)
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

		private Button buttonCreator(int i)
		{
			new Button {
				Text = "" + i,
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,
			};
			return friendButtons[i];
		}

		private Grid groupListCreator(Group group)
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

		private async void sendEventInvites()
		{
			if(friendsToInvite.Count != 0)dataManager.sendProfileInviteToEvent (eventBeingInvitedTo, friendsToInvite);

			for (int i = 0; i < groupsToInvite.Count; i++) {
				dataManager.sendProfileInviteToEvent (eventBeingInvitedTo, groupsToInvite[i].Members );
			}
		}
	}
}

