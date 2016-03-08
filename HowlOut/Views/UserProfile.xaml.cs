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

		Button friendRequestButton = new Button ();
		Button groupRequestButton = new Button ();

		public UserProfile (Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent ();

			usersPhoto.Source = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=150&width=150";
			/*
			var userPicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
			usersPhoto.Source = ImageSource.FromUri (userPicUri);
			*/
			List<Comment> givenList = new List<Comment> ();


			if (userProfile != null) {
				listMaker.createList (profileGrid, userProfile.Friends, null, friendButtons, userProfile, friendRequestButton);
				System.Diagnostics.Debug.WriteLine ("friendRequestButton " + friendRequestButton.Text);
				listMaker.createList (groupGrid, null, userProfile.Groups, groupButtons, userProfile, groupRequestButton);
				givenList = userProfile.Comments;
				infoView.Content = new InspectProfile (userProfile);
			} else if(userGroup != null) {
				listMaker.createList (profileGrid, userGroup.Members, null, friendButtons, userProfile, null);
				friendsButton.Text = "Members";
				givenList = userGroup.Comments;
				infoView.Content = new InspectGroup (userGroup);
				groupsButton.IsVisible = false;
			} else if(eventObject != null) {
				listMaker.createList (profileGrid, eventObject.Attendees, null, friendButtons, null, null);
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

			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					Profile profile = null;
					if (userProfile != null) {
						profile = userProfile.Friends[int.Parse(button.Text)];
					} 
					else if(userGroup != null) { 
						profile = userGroup.Members[int.Parse(button.Text)];
					} 
					else if(eventObject != null) { 
						profile = eventObject.Attendees[int.Parse(button.Text)];
					}
					App.coreView.setContentView (new UserProfile (profile, null, null), "UserProfile");
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					App.coreView.setContentView (new UserProfile (null, userProfile.Groups[int.Parse(button.Text)], null), "UserProfile");
				};
			}

			friendRequestButton.Clicked += (sender, e) => {
				System.Diagnostics.Debug.WriteLine("This ? Freind Request Button Pressed");

				//App.coreView.setContentView (new TestView (userProfile, null, null, userProfile.RecievedFriendRequests), "UserProfile");
				App.coreView.setContentView (new InviteView (userProfile, null, null, userProfile.RecievedFriendRequests), "UserProfile");
			};

			groupRequestButton.Clicked += (sender, e) => {

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

