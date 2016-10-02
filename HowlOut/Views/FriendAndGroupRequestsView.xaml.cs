using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class FriendAndGroupRequestsView : ContentView
	{
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		public FriendAndGroupRequestsView (bool profiles)
		{
			InitializeComponent ();

			if(profiles) {
				listMaker.createList (profileGrid, App.userProfile.RecievedFriendRequests, null, ListsAndButtons.ListType.FriendAndGroupRequests, null, null);
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
				friendsButton.FontAttributes = FontAttributes.Bold;
			} else { 
				listMaker.createList (groupGrid, null, App.userProfile.GroupsInviteTo, ListsAndButtons.ListType.FriendAndGroupRequests, null, null);
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
				groupsButton.FontAttributes = FontAttributes.Bold;
			}

			friendsButton.Clicked  += (sender, e) => {
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
				groupsButton.FontAttributes = FontAttributes.None;
				friendsButton.FontAttributes = FontAttributes.Bold;
			};
			groupsButton.Clicked  += (sender, e) => {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
				groupsButton.FontAttributes = FontAttributes.Bold;
				friendsButton.FontAttributes = FontAttributes.None;
			};
		}
	}
}

