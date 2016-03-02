using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;

namespace HowlOut
{
	public partial class UserProfile : ContentView
	{
		Profile userProfile;
		Group userGroup;
		Event eventObject;

		DataManager dataManager = new DataManager();

		List <Button> friendButtons = new List <Button>();
		List <Button> groupButtons = new List <Button>();

		List <Profile> friendsToInvite = new List <Profile>();
		List <Group> groupsToInvite = new List <Group>();


		public CreateEvent createEventView;

		public UserProfile (Profile prof, Group group, Event eve, bool inviteMode, bool observer)
		{
			InitializeComponent ();

			userProfile = prof;
			userGroup = group;
			eventObject = eve;

			if (userProfile != null) {
				createList (userProfile.Friends , null, profileGrid);
				createList (null, userProfile.Groups, groupGrid);
			} else if(userGroup != null) {
				createList (userGroup.Members , null, profileGrid);
				friendsButton.Text = "Members";
			} else if(eventObject != null) {
				createList (eventObject.Attendees , null, profileGrid);
				friendsButton.Text = "Attendees";
			}

			if (inviteMode) {
				infoView.IsVisible = false;
			} else {
				inviteLayout.IsVisible = false;

				if (userProfile != null) {
					infoView.Content = new InspectProfile (userProfile);
					WallList.ItemsSource = userProfile.Comments;
				} else if(userGroup != null) {
					infoView.Content = new InspectGroup (userGroup);
					WallList.ItemsSource = userGroup.Comments;
				} else if(eventObject != null) {
					infoView.Content = new InspectEvent (eventObject, observer);
					WallList.ItemsSource = eventObject.Comments;
				}
			}

			profileGrid.IsVisible = true;
			groupGrid.IsVisible = false;
			wall.IsVisible = false;

			friendsButton.Clicked  += (sender, e) => {
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
				wall.IsVisible = false;
			};
			groupsButton.Clicked  += (sender, e) => {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
				wall.IsVisible = false;
			};
			wallButton.Clicked += (sender, e) =>  {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = false;
				wall.IsVisible = true;
			};

			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					if(inviteMode){
						friendsToInvite.RemoveAll(userProfile.Friends[int.Parse(button.Text)]);
						friendsToInvite.Add(userProfile.Friends[int.Parse(button.Text)]);
					}
					else {
						App.coreView.setContentView( new InspectProfile (userProfile.Friends[int.Parse(button.Text)]), "InspectProfile");
					}
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					if(inviteMode){
						groupsToInvite.RemoveAll(userProfile.Groups[int.Parse(button.Text)]);
						groupsToInvite.Add(userProfile.Groups[int.Parse(button.Text)]);
					}
					else {
						App.coreView.setContentView( new InspectGroup (userProfile.Groups[int.Parse(button.Text)].Members ), "InspectGroup");
					}
				};
			}

			inviteButton.Clicked += (sender, e) => {
				if(userGroup != null) {
					sendInviteToGroup();
				} else if(eventObject != null) {
					sendInviteToEvent();
				}
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

		private async void sendInviteToEvent()
		{
			if(friendsToInvite.Count != 0)dataManager.sendProfileInviteToEvent (eventObject, friendsToInvite);

			for (int i = 0; i < groupsToInvite.Count; i++) {
				dataManager.sendProfileInviteToEvent (eventObject, groupsToInvite[i].Members );
			}
		}

		private async void sendInviteToGroup()
		{
			if(friendsToInvite.Count != 0)dataManager.sendInviteToGroup (sendInviteToGroup, friendsToInvite);
		}

		private async void PostNewComment(Event eve)
		{
			if(commentEntry.Text != null || commentEntry.Text != "")
			{
				Event newEvent = await dataManager.AddCommentToEvent(eve.EventId, new Comment {
					Content = commentEntry.Text, SenderID = App.StoredUserFacebookId, DateAndTime = DateTime.Now.ToLocalTime(),
				});
				if (newEvent != null) {
					commentEntry.Text = "";
					App.coreView.setContentView (new InspectEvent (newEvent, 2), "InspectEvent");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}
	}
}

