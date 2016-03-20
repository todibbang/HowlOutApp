using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectController : ContentView
	{
		ListsAndButtons listMaker = new ListsAndButtons();

		private EventApiManager eventApiManager= new EventApiManager (new HttpClient(new NativeMessageHandler()));

		ObservableCollection <Button> profileButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> groupButtons = new ObservableCollection <Button>();
		public CreateEvent createEventView;

		List<Comment> givenList = new List<Comment> ();
		Button FindNewFriendsButton = new Button {BackgroundColor= Color.Transparent,};
		Button FindNewGroupsButton = new Button {BackgroundColor= Color.Transparent,};

		public InspectController (Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent ();

			usersPhoto.Source = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=150&width=150";

			if (userProfile != null) {
				if (userProfile.ProfileId == App.userProfile.ProfileId) {
					listMaker.createList (profileGrid, userProfile.Friends, null, FindNewFriendsButton, "normal", null);
					listMaker.createList (groupGrid, null, userProfile.Groups, FindNewGroupsButton, "normal", null);
					givenList = userProfile.Comments;

					friendsButton.IsVisible = true;
					groupsButton.IsVisible = true;
					wallButton.IsVisible = true;
				}
				infoView.Content = new ProfileDesignView (userProfile, null, null, 200, ProfileDesignView.ProfileDesign.WithButtons);

			} else if(userGroup != null) {
				listMaker.createList (profileGrid, userGroup.Members, null, null, "normal", null);
				friendsButton.Text = "Members";
				givenList = userGroup.Comments;
				infoView.Content = new InspectGroup (userGroup);

			} else if(eventObject != null) {
				listMaker.createList (profileGrid, eventObject.Attendees, null, null, "normal", null);
				profileGrid.IsVisible = true;
				friendsButton.Text = "Attendees";
				givenList = eventObject.Comments;
				wallButton.IsVisible = true;
				friendsButton.IsVisible = true;

				bool eventNotJoined = true;
				for (int i = 0; i < eventObject.Attendees.Count; i++) {
					if (App.userProfile.ProfileId == eventObject.Attendees[i].ProfileId) {
						eventNotJoined = false;
					}
				}
				infoView.Content = new InspectEvent (eventObject, eventNotJoined);
			}


			if (givenList != null) {
				List<Comment> displayedList = new List<Comment> ();
				for (int i = givenList.Count - 1; i > -1; i--) {
					displayedList.Add (givenList [i]);
				}
				WallList.ItemsSource = displayedList;
			}

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


			foreach (Button button in profileButtons) {
				button.Clicked += (sender, e) => {
					int counter = 0;
					for(int i = 0; i < profileButtons.Count; i++){
						if(profileButtons[i] == button) counter = i;
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
					App.coreView.setContentView (new InspectController (profile, null, null), "UserProfile");
				};
			}

			foreach (Button button in groupButtons) {
				button.Clicked += (sender, e) => {
					App.coreView.setContentView (new InspectController (null, userProfile.Groups[int.Parse(button.Text)], null), "UserProfile");
				};
			}

			FindNewFriendsButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new InviteView (userProfile, null, null, userProfile.RecievedFriendRequests), "InviteView");
			};

			FindNewGroupsButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new CreateGroup(), "Create Group");
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
					App.coreView.setContentView (new InspectController (null, null, newEvent), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}
	}
}

