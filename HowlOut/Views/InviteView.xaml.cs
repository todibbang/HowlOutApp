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

		ObservableCollection <Button> friendButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> acceptButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> declineButtons = new ObservableCollection <Button>();


		public InviteView (Profile userProfile, Group userGroup, Event eventObject, List<Profile> profilesToSelectFrom)
		{
			InitializeComponent ();
			_dataManager = new DataManager ();


			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };

			profileGrid.IsVisible = true;

			if(userProfile != null){
				for (int e = 0; e < userProfile.Friends.Count; e++) {
					profilesNotToInvite.Add (userProfile.Friends [e], userProfile.Friends [e].ProfileId);
				}
				listMaker.createList (profileGrid, profilesToSelectFrom, null, null, "invite");
			} else if (userGroup != null) { 
				for (int e = 0; e < userGroup.Members.Count; e++) {
					profilesNotToInvite.Add (userGroup.Members [e], userGroup.Members [e].ProfileId);
				}
				listMaker.createList (profileGrid, profilesToSelectFrom, null, null, "invite");
			} else if (eventObject != null) { 
				for (int e = 0; e < eventObject.Attendees.Count; e++) {
					profilesNotToInvite.Add (eventObject.Attendees [e], eventObject.Attendees [e].ProfileId);
				}
				listMaker.createList (profileGrid, profilesToSelectFrom, null, null, "invite");
			}

			for (int i = profilesToSelectFrom.Count - 1; i > -1; i--) {
				if (profilesNotToInvite.ContainsValue (profilesToSelectFrom [i].ProfileId)) {
					profilesToSelectFrom.Remove (profilesToSelectFrom [i]);
				}
			}



			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					int counter = 0;
					for(int i = 0; i < friendButtons.Count; i++){
						if(friendButtons[i] == button) counter = i;
					}

					var profile = profilesToSelectFrom[counter];
					App.coreView.setContentView (new InspectController (profile, null, null), "UserProfile");
				};
			}

			foreach (Button button in acceptButtons) {
				button.Clicked += (sender, e) => {
					int counter = 0;
					for(int i = 0; i < friendButtons.Count; i++){
						if(friendButtons[i] == button) counter = i;
					}
					var profile = profilesToSelectFrom[counter];

					if (userProfile != null) {
						acceptFriendRequest(profile, acceptButtons[counter], declineButtons[counter]);
					} else if(eventObject != null) {
						_dataManager.sendInviteToEvent(eventObject, profile);
					}
				};
			}
		}

		private async void acceptFriendRequest(Profile profile, Button acceptButton, Button declineButton)
		{
			bool success = await _dataManager.acceptFriendRequest(profile, false);
			if (success) {
				declineButton.IsVisible = false;
				acceptButton.Text = "Friend Added";
				acceptButton.IsEnabled = false;
			}
		}

		//private void InviteFriendsToProfile
	}
}

