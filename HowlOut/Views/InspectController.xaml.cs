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

		private DataManager _dataManager = new DataManager ();


		public CreateEvent createEventView;

		List<Comment> givenCommentList = new List<Comment> ();
		List<Profile> profileList = new List<Profile> ();
		Button FindNewFriendsButton = new Button {BackgroundColor= Color.Transparent,};
		Button FindNewGroupsButton = new Button {BackgroundColor= Color.Transparent,};

		public InspectController (Profile userProfile, Group userGroup, Event eventObject)
		{
			InitializeComponent ();

			usersPhoto.Source = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=150&width=150";

			if (userProfile != null) {
				if (userProfile.ProfileId == App.userProfile.ProfileId) {
					userProfile = App.userProfile;
					if (userProfile.RecievedFriendRequests.Count > 0) {
						listMaker.createList (profileGrid, userProfile.Friends, null, FindNewFriendsButton, ListsAndButtons.ListType.Normal, null, null);
					} else {
						listMaker.createList (profileGrid, userProfile.Friends, null, null, ListsAndButtons.ListType.Normal, null, null);
					}

					if (userProfile.GroupsInviteTo.Count > 0) {
						listMaker.createList (groupGrid, null, userProfile.Groups, FindNewGroupsButton, ListsAndButtons.ListType.Normal, null, null);
					} else {
						//TODO following three lines are dummy data
						var groups = new List<Group> ();
						groups.Add (new Group(){Name = "PlaceHolderGroup", Owner=App.userProfile, Public = true, Members = new List<Profile>()});
						listMaker.createList (groupGrid, null, groups, null, ListsAndButtons.ListType.Normal, null, null);


						//listMaker.createList (groupGrid, null, userProfile.Groups, null, ListsAndButtons.ListType.Normal, null, null);
					}
					friendsButton.IsVisible = true;
					groupsButton.IsVisible = true;
				}
				infoView.Content = new ProfileDesignView (userProfile, null, null, 200, ProfileDesignView.ProfileDesign.WithButtons);

			} else if(userGroup != null) {
				profileList = userGroup.Members;
				profileList.Add (userGroup.Owner);
				listMaker.createList (profileGrid, profileList, null, null, ListsAndButtons.ListType.Normal, null, null);
				friendsButton.Text = "Members";
				givenCommentList = userGroup.Comments;
				wallButton.IsVisible = true;
				friendsButton.IsVisible = true;
				infoView.Content = new ProfileDesignView (null, userGroup,null,200, ProfileDesignView.ProfileDesign.WithButtons);

			} else if(eventObject != null) {
				profileList = eventObject.Attendees;
				//profileList.Add (eventObject.Owner);
				listMaker.createList (profileGrid, profileList, null, null, ListsAndButtons.ListType.Normal, null, null);
				profileGrid.IsVisible = true;
				friendsButton.Text = "Attendees";
				givenCommentList = eventObject.Comments;
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


			if (givenCommentList != null) {
				List<Comment> displayedList = new List<Comment> ();
				for (int i = givenCommentList.Count - 1; i > -1; i--) {
					displayedList.Add (givenCommentList [i]);
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
				Event newEvent = await _dataManager.EventApiManager.AddCommentToEvent(eve.EventId, new Comment {
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

