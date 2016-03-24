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
					if (userProfile.RecievedFriendRequests.Count > 0) {
						listMaker.createList (profileGrid, userProfile.Friends, null, FindNewFriendsButton, ListsAndButtons.ListType.Normal, null, null);
					} else {
						listMaker.createList (profileGrid, userProfile.Friends, null, null, ListsAndButtons.ListType.Normal, null, null);
					}

					if (userProfile.GroupsInviteTo.Count > 0) {
						listMaker.createList (groupGrid, null, userProfile.Groups, FindNewGroupsButton, ListsAndButtons.ListType.Normal, null, null);
					} else {
						listMaker.createList (groupGrid, null, userProfile.Groups, null, ListsAndButtons.ListType.Normal, null, null);
					}

					givenList = userProfile.Comments;

					friendsButton.IsVisible = true;
					groupsButton.IsVisible = true;
					wallButton.IsVisible = true;
				}
				infoView.Content = new ProfileDesignView (userProfile, null, null, 200, ProfileDesignView.ProfileDesign.WithButtons);

			} else if(userGroup != null) {
				listMaker.createList (profileGrid, userGroup.Members, null, null, ListsAndButtons.ListType.Normal, null, null);
				friendsButton.Text = "Members";
				givenList = userGroup.Comments;
				//infoView.Content = new InspectGroup (userGroup);
				infoView.Content = new ProfileDesignView (null, userGroup,null,200, ProfileDesignView.ProfileDesign.WithButtons);

			} else if(eventObject != null) {
				listMaker.createList (profileGrid, eventObject.Attendees, null, null, ListsAndButtons.ListType.Normal, null, null);
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


			FindNewFriendsButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new FriendAndGroupRequestsView(true), "");
			};

			FindNewGroupsButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new FriendAndGroupRequestsView(false), "");
			};

			postCommentButton.Clicked += (sender, e) => {
				PostNewComment(eventObject, commentEntry.Text);
			};
		}

		private async void PostNewComment(Event eve, string comment)
		{
			if(!string.IsNullOrWhiteSpace(comment))
			{
				Event newEvent = await eventApiManager.AddCommentToEvent(eve.EventId, new Comment {
					Content = comment, SenderID = App.StoredUserFacebookId, DateAndTime = DateTime.Now.ToLocalTime(),
				});
				if (newEvent != null) {
					commentEntry.Text = "";
					App.coreView.setContentView (new InspectController (null, null, newEvent), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
				}
			}
		}
	}
}

