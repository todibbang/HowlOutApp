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
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		ListsAndButtons.ListType ListType = ListsAndButtons.ListType.Normal;

		private List<Profile> profilesToFind = new List<Profile>();
		private List<Group> groupsToFind = new List<Group>();
		private List<Profile> profilesNotToFind = new List<Profile>();

		public InviteView (Group groupObject, Event eventObject, WhatToShow whatToShow)
		{
			InitializeComponent ();

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };

			if (whatToShow.Equals (WhatToShow.PeopleToInviteToEvent)) {
				profilesToFind = App.userProfile.Friends;
				profilesNotToFind = eventObject.Attendees;
				groupsToFind = App.userProfile.Groups;
				ListType = ListsAndButtons.ListType.InviteToEvent;
			} else if (whatToShow.Equals (WhatToShow.PeopleToInviteToGroup)) {
				profilesToFind = App.userProfile.Friends;
				profilesNotToFind = groupObject.Members;
				ListType = ListsAndButtons.ListType.InviteToGroup;
				buttonsLayout.IsVisible = false;
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



			friendsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
			};
			groupsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
			};
		}

		public enum WhatToShow {
			PeopleToInviteToEvent,
			PeopleToInviteToGroup,
		}
	}
}

