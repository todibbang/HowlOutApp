using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;
using ImageCircle.Forms.Plugin.Abstractions;

namespace HowlOut
{
	public partial class InviteView : ContentView
	{
		DataManager _dataManager;

		ListsAndButtons listMaker = new ListsAndButtons();

		ListsAndButtons.ListType ListType = ListsAndButtons.ListType.Normal;

		private List<Profile> profilesToFind = new List<Profile>();
		private List<Group> groupsToFind = new List<Group>();
		private List<Profile> profilesNotToFind = new List<Profile>();

		public InviteView (Group groupObject, Event eventObject, WhatToShow whatToFind)
		{
			InitializeComponent ();
			_dataManager = new DataManager ();
			profileGrid.IsVisible = true;

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };

			if (whatToFind.Equals (WhatToShow.FriendRequests)) {
				profilesToFind = App.userProfile.RecievedFriendRequests;
				buttons.IsVisible = false;
			} else if (whatToFind.Equals (WhatToShow.GroupRequests)) {
				groupsToFind = App.userProfile.GroupsInviteTo;
				buttons.IsVisible = false;
				groupGrid.IsVisible = true;
				profileGrid.IsVisible = false;
			} else if (whatToFind.Equals (WhatToShow.PeopleToInviteToEvent)) {
				profilesToFind = App.userProfile.Friends;
				profilesNotToFind = eventObject.Attendees;
				groupsToFind = App.userProfile.Groups;
				ListType = ListsAndButtons.ListType.InviteToEvent;
			} else if (whatToFind.Equals (WhatToShow.PeopleToInviteToGroup)) {
				profilesToFind = App.userProfile.Friends;
				profilesNotToFind = groupObject.Members;
				ListType = ListsAndButtons.ListType.InviteToGroup;
			} else if(whatToFind.Equals(WhatToShow.NewPeople)) {
				searchLayout.IsVisible = true;
			}

			for (int i = 0; i < profilesNotToFind.Count; i++) {
				profilesNotToInvite.Add (profilesNotToFind [i], profilesNotToFind [i].ProfileId);
			}

			for (int i = profilesToFind.Count - 1; i > -1; i--) {
				if (profilesNotToInvite.ContainsValue (profilesToFind [i].ProfileId)) {
					profilesToFind.Remove (profilesToFind [i]);
				}
			}

			if(profilesToFind.Count > 0) listMaker.createList (profileGrid, profilesToFind, null, null, ListType, eventObject, groupObject);
			if(groupsToFind.Count > 0) listMaker.createList (groupGrid, null, groupsToFind, null, ListType, eventObject, groupObject);

			searchBar.TextChanged += (sender, e) => {
				if(searchBar.Text == "" || searchBar.Text == null) { 
					
				} else {
					updateAutocompleteList(groupObject, eventObject);
				}
			};

			friendsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
			};
			groupsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
			};
		}

		public async void updateAutocompleteList(Group groupObject, Event eventObject)
		{
			var profileSearchResult = App.userProfile.Friends;
			var groupSearchResult = await _dataManager.GroupApiManager.GetAllGroups ();
			groupSearchResult.Add (new Group () {Owner = App.userProfile, Name = "Test Group", Public = true, Members = new List<Profile>() });
			listMaker.createList (profileGrid, profileSearchResult, null, null, ListType, eventObject, groupObject);
			listMaker.createList (groupGrid, null, groupSearchResult, null, ListType, eventObject, groupObject);
		}

		public enum WhatToShow {
			FriendRequests,
			GroupRequests,
			NewPeople,
			PeopleToInviteToEvent,
			PeopleToInviteToGroup,
			Plain,
			MyFriendsAndGroups
		}
	}
}

