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
		EventApiManager eventApiManager= new EventApiManager (new HttpClient(new NativeMessageHandler()));
		UtilityManager utilityManager = new UtilityManager ();

		ListsAndButtons listMaker = new ListsAndButtons();
		DataManager dataManager = new DataManager();

		ObservableCollection <Button> friendButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> acceptButtons = new ObservableCollection <Button>();
		ObservableCollection <Button> declineButtons = new ObservableCollection <Button>();
		Button friendRequestButton = new Button ();


		public InviteView (Profile userProfile, Group userGroup, Event eventObject, List<Profile> profilesToSelectFrom)
		{
			InitializeComponent ();

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };
			ObservableCollection <Profile> profilesToInvite = new ObservableCollection <Profile>();

			profileGrid.IsVisible = true;

			if(userProfile != null){
				for (int e = 0; e < userProfile.Friends.Count; e++) {
					profilesNotToInvite.Add (userProfile.Friends [e], userProfile.Friends [e].ProfileId);
				}
			} else if (userGroup != null) { 
				for (int e = 0; e < userGroup.Members.Count; e++) {
					profilesNotToInvite.Add (userGroup.Members [e], userGroup.Members [e].ProfileId);
				}
			} else if (eventObject != null) { 
				for (int e = 0; e < eventObject.Attendees.Count; e++) {
					profilesNotToInvite.Add (eventObject.Attendees [e], eventObject.Attendees [e].ProfileId);
				}
			}

			for (int i = profilesToSelectFrom.Count - 1; i > -1; i--) {
				if (profilesNotToInvite.ContainsValue (profilesToSelectFrom [i].ProfileId)) {
					profilesToSelectFrom.Remove (profilesToSelectFrom [i]);
				}
			}

			listMaker.createList (profileGrid, profilesToSelectFrom, null, friendButtons, acceptButtons, declineButtons, userProfile, friendRequestButton);

			foreach (Button button in friendButtons) {
				button.Clicked += (sender, e) => {
					int counter = 0;
					for(int i = 0; i < friendButtons.Count; i++){
						if(friendButtons[i] == button) counter = i;
					}

					var profile = profilesToSelectFrom[counter];
					App.coreView.setContentView (new UserProfile (profile, null, null), "UserProfile");
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
						utilityManager.acceptFriendRequest(profile);
					} else if(eventObject != null) {
						utilityManager.sendInviteToEvent(eventObject, profile);
					}
				};
			}
		}


	}
}

