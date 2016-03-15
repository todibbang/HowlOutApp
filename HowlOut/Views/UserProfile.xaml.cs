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
		ListsAndButtons listMaker = new ListsAndButtons();

		private EventApiManager eventApiManager= new EventApiManager (new HttpClient(new NativeMessageHandler()));

		ObservableCollection <Button> friendButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> groupButtons = new ObservableCollection <Button>();
		ObservableCollection <Profile> friendsToInvite = new ObservableCollection <Profile>();
		ObservableCollection <Group> groupsToInvite = new ObservableCollection <Group>();
		public CreateEvent createEventView;

		List<Comment> givenList = new List<Comment> ();
		Button friendRequestButton = new Button {BackgroundColor= Color.Transparent,};
		Button groupRequestButton = new Button {BackgroundColor= Color.Transparent,};
		Button FindNewFriendsButton = new Button {BackgroundColor= Color.Transparent,};
		Button FindNewGroupsButton = new Button {BackgroundColor= Color.Transparent,};

		public UserProfile (Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent ();

			usersPhoto.Source = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=150&width=150";
			/*
			var userPicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
			usersPhoto.Source = ImageSource.FromUri (userPicUri);
			*/



			if (userProfile != null) {
				listMaker.createList (profileGrid, userProfile.Friends, null, friendButtons, null,null, userProfile, FindNewFriendsButton);
				listMaker.createList (groupGrid, null, userProfile.Groups, groupButtons, null,null, userProfile, FindNewGroupsButton);
				givenList = userProfile.Comments;
				infoView.Content = new InspectProfile (userProfile);

			} else if(userGroup != null) {
				listMaker.createList (profileGrid, userGroup.Members, null, friendButtons, null,null, userProfile, null);
				friendsButton.Text = "Members";
				givenList = userGroup.Comments;
				infoView.Content = new InspectGroup (userGroup);
				groupsButton.IsVisible = false;

			} else if(eventObject != null) {
				listMaker.createList (profileGrid, eventObject.Attendees, null, friendButtons,null, null, null, null);
				friendsButton.Text = "Attendees";
				givenList = eventObject.Comments;

				bool eventNotJoined = true;
				for (int i = 0; i < App.userProfile.JoinedEvents.Count; i++) {
					if (App.userProfile.JoinedEvents [i].EventId == eventObject.EventId) {
						eventNotJoined = false;
					}
				}
				System.Diagnostics.Debug.WriteLine ("" + eventNotJoined);
				infoView.Content = new InspectEvent (eventObject, eventNotJoined);
				groupsButton.IsVisible = false;
			}


			if (givenList != null) {
				List<Comment> displayedList = new List<Comment> ();
				for (int i = givenList.Count - 1; i > -1; i--) {
					displayedList.Add (givenList [i]);
				}
				WallList.ItemsSource = displayedList;
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


			System.Diagnostics.Debug.WriteLine (friendButtons.Count + " friendButtons");


			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					int counter = 0;
					for(int i = 0; i < friendButtons.Count; i++){
						if(friendButtons[i] == button) counter = i;
					}
						
					Profile profile = null;

					if (userProfile != null) {
						profile = userProfile.Friends[counter];
					} 
					else if(userGroup != null) { 
						profile = userGroup.Members[counter];
					} 
					else if(eventObject != null) { 
						profile = eventObject.Attendees[counter];
					}
					App.coreView.setContentView (new UserProfile (profile, null, null), "UserProfile");
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					App.coreView.setContentView (new UserProfile (null, userProfile.Groups[int.Parse(button.Text)], null), "UserProfile");
				};
			}

			FindNewFriendsButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new InviteView (userProfile, null, null, userProfile.RecievedFriendRequests), "UserProfile");
			};

			FindNewGroupsButton.Clicked += (sender, e) => {

			};
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
					App.coreView.setContentView (new UserProfile (null, null, newEvent), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}
	}
}

