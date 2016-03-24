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
				listMaker.createList (profileGrid, App.userProfile.RecievedFriendRequests, null, null, ListsAndButtons.ListType.Normal, null, null);
			} else { 
				listMaker.createList (groupGrid, null, App.userProfile.GroupsInviteTo, null, ListsAndButtons.ListType.Normal, null, null);
			}
		}
	}
}

