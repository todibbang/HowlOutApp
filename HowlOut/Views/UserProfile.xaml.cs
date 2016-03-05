using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class UserProfile : ContentView
	{
		Profile userProfile;
		Group userGroup;
		Event eventObject;

		DataManager dataManager = new DataManager();
		private EventApiManager eventApiManager;
		private HttpClient httpClient;

		ObservableCollection <Button> friendButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> groupButtons = new ObservableCollection <Button>();

		ObservableCollection <Profile> friendsToInvite = new ObservableCollection <Profile>();
		ObservableCollection <Group> groupsToInvite = new ObservableCollection <Group>();


		public CreateEvent createEventView;

		public UserProfile (Profile prof, Group group, Event eve, bool inviteMode, bool observer)
		{
			InitializeComponent ();

			httpClient = new HttpClient(new NativeMessageHandler());
			eventApiManager = new EventApiManager (httpClient);

			var userPicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
			usersPhoto.Source = ImageSource.FromUri (userPicUri);



			userProfile = prof;
			userGroup = group;
			eventObject = eve;

			List<Comment> givenList = new List<Comment> ();
			if (userProfile != null) {
				createList (userProfile.Friends , null, profileGrid);
				createList (null, userProfile.Groups, groupGrid);
				givenList = userProfile.Comments;
			} else if(userGroup != null) {
				createList (userGroup.Members , null, profileGrid);
				friendsButton.Text = "Members";
				givenList = userGroup.Comments;
			} else if(eventObject != null) {
				createList (eventObject.Attendees , null, profileGrid);
				friendsButton.Text = "Attendees";
				givenList = eventObject.Comments;
			}
			if (givenList != null) {
				List<Comment> displayedList = new List<Comment> ();
				for (int i = givenList.Count - 1; i > -1; i--) {
					displayedList.Add (givenList [i]);
				}
				WallList.ItemsSource = displayedList;
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
					
					System.Diagnostics.Debug.WriteLine ("BUUUUUUUUUUUU");
					infoView.IsVisible = true;
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
					Profile profile = null;
					if (userProfile != null) { profile = userProfile.Friends[int.Parse(button.Text)];} 
					else if(userGroup != null) { profile = userGroup.Members[int.Parse(button.Text)];} 
					else if(eventObject != null) { profile = eventObject.Attendees[int.Parse(button.Text)];}

					if(inviteMode){
						if(!friendsToInvite.Contains(profile)) {
							friendsToInvite.Add(profile);
						}
					}
					else {
						App.coreView.setContentView (new UserProfile (profile, null, null, false, false), "UserProfile");
					}
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					if(inviteMode){
						if(!groupsToInvite.Contains(userProfile.Groups[int.Parse(button.Text)])){
							groupsToInvite.Add(userProfile.Groups[int.Parse(button.Text)]);
						}
					}
					else {
						App.coreView.setContentView (new UserProfile (null, userProfile.Groups[int.Parse(button.Text)], null, false, false), "UserProfile");
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
			int count = 0;
			if (profiles != null) {
				count = profiles.Count;
			} else {
				count = groups.Count;
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
			
			Button button = new Button {
				Text = "" + i,
				TextColor = Color.Transparent,
				BackgroundColor = Color.Transparent,
			};
			return button;
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
			if (friendsToInvite.Count != 0) {
				await dataManager.sendProfileInviteToEvent (eventObject, friendsToInvite);
			}



			for (int i = 0; i < groupsToInvite.Count; i++) {
				ObservableCollection<Profile> members = new ObservableCollection<Profile> ();
				for (int e = 0; e < groupsToInvite [i].Members.Count; e++) {
					members.Add (groupsToInvite [i].Members[e]);
				}
				await dataManager.sendProfileInviteToEvent (eventObject, members );
			}
		}

		private async void sendInviteToGroup()
		{
			if (friendsToInvite.Count != 0) {
				await dataManager.sendInviteToGroup (userGroup, friendsToInvite);
			}
		}

		private async void PostNewComment(Event eve)
		{
			if(commentEntry.Text != null || commentEntry.Text != "")
			{
				Event newEvent = await eventApiManager.AddCommentToEvent(eve.EventId, new Comment {
					Content = commentEntry.Text, SenderID = App.StoredUserFacebookId, DateAndTime = DateTime.Now.ToLocalTime(),
				});
				if (newEvent != null) {
					commentEntry.Text = "";
					App.coreView.setContentView (new UserProfile (null, null, newEvent, false, false), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}
	}
}

