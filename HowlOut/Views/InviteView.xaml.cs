using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InviteView : ContentView
	{
		EventApiManager eventApiManager= new EventApiManager (new HttpClient(new NativeMessageHandler()));

		ListsAndButtons listMaker = new ListsAndButtons();
		ObservableCollection <Button> inviteButtons = new ObservableCollection <Button>();


		public InviteView (Profile userProfile, Group userGroup, Event eventObject, List<Profile> profilesToSelectFrom)
		{
			InitializeComponent ();

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };
			ObservableCollection <Profile> profilesToInvite = new ObservableCollection <Profile>();


			if(userProfile != null){
				for (int e = 0; e < userProfile.Friends.Count; e++) {
					profilesNotToInvite.Add (userProfile.Friends [e], userProfile.Friends [e].ProfileId);
				}
				friendGroupButtons.IsVisible = false;
				inviteButton.IsVisible = false;

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

			listMaker.createList (profileGrid, profilesToSelectFrom, null, inviteButtons, null, null);


			foreach (Button button in inviteButtons) {
				button.Clicked += (sender, e) => {
					Profile profile = null;
					if (userProfile != null) {
						profile = profilesToSelectFrom[int.Parse(button.Text)];
					} 
					else if(userGroup != null) { 
						profile = userGroup.Members[int.Parse(button.Text)];
					} 
					else if(eventObject != null) { 
						profile = eventObject.Attendees[int.Parse(button.Text)];
					}
					System.Diagnostics.Debug.WriteLine("BLA NBKA fksjngsljkdfng");

					App.coreView.setContentView (new UserProfile (profile, null, null), "UserProfile");
				};
			}

			/*

			foreach (Button button in inviteButtons) {
				button.Clicked += (sender, e) => 
				{
					

					if (userProfile != null) { 
						App.coreView.setContentView (new UserProfile (profilesToSelectFrom[int.Parse(button.Text)], null, null), "UserProfile");
						//profile = profilesToSelectFrom[int.Parse(button.Text)];
					} 
					//else if(userGroup != null) { profile = userGroup.Members[int.Parse(button.Text)];} 
					//else if(eventObject != null) { profile = eventObject.Attendees[int.Parse(button.Text)];}

					if(!profilesToInvite.Contains(profile)) {
						profilesToInvite.Add(profile);
					}
				};
			}
	*/

			inviteButton.Clicked += (sender, e) => {
				if(userGroup != null) {
					sendInviteToGroup(userGroup, profilesToInvite);
				} else if(eventObject != null) {
					sendInviteToEvent(eventObject, profilesToInvite);
				}
			};
		}


		private async void sendInviteToEvent(Event eve, ObservableCollection<Profile> profiles)
		{
			List <string> IdsToInvite = new List<string> ();

			for (int i = 0; i < profiles.Count; i++) {
				IdsToInvite.Add (profiles[i].ProfileId);
			}

			/*
			for (int i = 0; i < groupsToInvite.Count; i++) {
				for (int e = 0; e < groupsToInvite [i].Members.Count; e++) {
					IdsToInvite.Add (groupsToInvite [i].Members [e].ProfileId);
				}
			}
			*/
			await eventApiManager.InviteToEvent(eve.EventId, IdsToInvite);
		}

		private async void sendInviteToGroup(Group group, ObservableCollection<Profile> profiles)
		{
			GroupApiManager groupManager = new GroupApiManager ();
			List <string> IdsToInvite = new List<string> ();

			for (int i = 0; i < profiles.Count; i++) {
				IdsToInvite.Add (profiles[i].ProfileId);
			}
			//await groupManager.InviteToEvent(group.GroupId, IdsToInvite);
		}

	}
}

