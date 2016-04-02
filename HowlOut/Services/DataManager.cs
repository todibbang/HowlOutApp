using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public class DataManager
	{
		private HttpClient httpClient;
		public EventApiManager EventApiManager { get; set; }
		public ProfileApiManager ProfileApiManager { get; set; }
		public GroupApiManager GroupApiManager { get; set; }
		public UtilityManager UtilityManager { get; set; }

		public DataManager ()
		{
			httpClient = new HttpClient (new NativeMessageHandler ());
			EventApiManager = new EventApiManager (httpClient);
			ProfileApiManager = new ProfileApiManager (httpClient);
			GroupApiManager = new GroupApiManager (httpClient);
			UtilityManager = new UtilityManager ();
		}

		public Uri GetFacebookProfileImageUri (string facebookUserId)
		{
			return new Uri ("https://graph.facebook.com/v2.5/" + facebookUserId + "/picture?height=300&width=300");
		}

		public async Task update ()
		{
			App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.StoredUserFacebookId);
		}



		public async Task<ObservableCollection<Address>> AutoCompletionPlace (string input)
		{
			string path = "https://dawa.aws.dk/autocomplete?q=" + input;
			ObservableCollection<Address> addresses = new ObservableCollection<Address> ();

			try { 
				var response = await httpClient.GetAsync (new Uri (path));
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					addresses = JsonConvert.DeserializeObject<ObservableCollection<Address>> (content);

					for (int i = 0; i < addresses.Count; i++) {
						System.Diagnostics.Debug.WriteLine ("forslagstekst: " + addresses [i].forslagstekst + " " + addresses [i].data.href);
					}
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}

			return addresses;

		}

		public async Task<Position> GetCoordinates (string input)
		{
			input = Regex.Replace (input, "http", "https");

			string path = input;

			string adgangspunkt = "";

			Position position = new Position ();

			try { 
				var response = await httpClient.GetAsync (new Uri (path));

				System.Diagnostics.Debug.WriteLine ("Success getting new coords");

				if (response.IsSuccessStatusCode) {
					System.Diagnostics.Debug.WriteLine ("Success getting new coords");

					adgangspunkt = await response.Content.ReadAsStringAsync ();
					//adgangspunkt = JsonConvert.DeserializeObject<string>(content);

					System.Diagnostics.Debug.WriteLine (adgangspunkt);
					var substrings = Regex.Split (adgangspunkt, "koordinater");

					position = new Position (Convert.ToDouble (substrings [1].Substring (35, 16)), Convert.ToDouble (substrings [1].Substring (11, 16)));
					//position.Latitude = Convert.ToDouble(substrings [1].Substring (11, 16));
					//position.Longitude = Convert.ToDouble(substrings [1].Substring (35, 16));
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}


			return position;
		}


		public async void sendFriendRequest(Profile profile)
		{
			Profile newProfile = null;
			bool success = await ProfileApiManager.RequestFriend(profile.ProfileId, App.userProfile.ProfileId);
			if (success) {
				App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.userProfile.ProfileId);
				await loadUpdatedProfile(profile);
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not sent, try again.", "Ok");
			}
		}

		public async Task<bool> acceptFriendRequest(Profile profile, bool goToProfile)
		{
			bool success = await ProfileApiManager.AcceptFriend(profile.ProfileId, App.userProfile.ProfileId);
			if (success) {
				App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.userProfile.ProfileId);
				if(goToProfile) await loadUpdatedProfile(profile);
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not accepted, try again.", "Ok");
			}
			return success;
		}

		public async void declineFriendRequest(Profile profile)
		{
			bool success = await ProfileApiManager.DeclineFriendRequest(profile.ProfileId, App.userProfile.ProfileId);
			if (success) {
				App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.userProfile.ProfileId);
				await loadUpdatedProfile(profile);
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not accepted, try again.", "Ok");
			}
		}

		public async void removeFriend(Profile profile)
		{
			bool success = await ProfileApiManager.RemoveFriend(profile.ProfileId, App.userProfile.ProfileId);
			if (success) {
				App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.userProfile.ProfileId);
				await loadUpdatedProfile(profile);
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not sent, try again.", "Ok");
			}
		}

		private async Task loadUpdatedProfile(Profile profile)
		{
			App.userProfile = await ProfileApiManager.GetLoggedInProfile (App.userProfile.ProfileId);
			App.coreView.setContentView (new InspectController (profile, null, null), "UserProfile");
		}

		public async Task<bool> sendProfileInviteToEvent(Event eve, Profile profile)
		{
			List <string> IdsToInvite = new List<string> ();
			IdsToInvite.Add (profile.ProfileId);
			bool success = await EventApiManager.InviteToEvent(eve.EventId, IdsToInvite);
			if (!success) {
				await App.coreView.displayAlertMessage ("Error", "An error happened and " + profile.Name + " was not invited", "Ok");
				return false;
			} else {
				return true;
			}
		}

		public async Task<bool>  sendProfileInviteToGroup(Group group, Profile profile)
		{
			List<string> profileIds = new List<string> ();
			profileIds.Add (profile.ProfileId);
			bool success = await GroupApiManager.InviteToGroup(group.GroupId, profileIds);
			if (!success) {
				await App.coreView.displayAlertMessage ("Error", "An error happened and " + profile.Name + " was not invited to the group " + group.Name, "Ok");
				return false;
			} else {
				return true;
			}

			System.Diagnostics.Debug.WriteLine ("Boo");
		}

		public async void leaveEvent(Event eve)
		{
			bool leaveConfirmed = await App.coreView.displayConfirmMessage("Warning", "You are about to leave this event, would you like to continue?", "Yes", "No");
			if (leaveConfirmed) {
				bool hasLeft = await EventApiManager.UnattendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasLeft) {
					await App.coreView.displayAlertMessage ("Event Left", "You have successfully left the event.", "Ok");
					App.coreView.setContentView (new EventView (), "Event");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Left", "An error happened and you have not yet left the event, try again.", "Ok");
				}
			}
		}

		public async void joinEvent(Event eve)
		{
			bool joinConfirmed = await App.coreView.displayConfirmMessage("Joining", "You are about to join this event, would you like to continue?", "Yes", "No");
			if (joinConfirmed) {
				bool hasJoined = await EventApiManager.AttendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasJoined) {
					Event eventWhenJoined = await EventApiManager.GetEventById (eve.EventId);
					await App.coreView.displayAlertMessage ("Event Joined", "You have successfully joined the event.", "Ok");
					App.coreView.setContentView (new InspectController (null, null, eventWhenJoined), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}

		public async void followEvent(Event eve)
		{
			bool followConfirmed = await App.coreView.displayConfirmMessage("Following", "You are about to follow this event, would you like to continue?", "Yes", "No");
			if (followConfirmed) {
				bool hasFollowed = await EventApiManager.FollowEvent (eve.EventId, App.userProfile.ProfileId);
				if (hasFollowed) {
					await App.coreView.displayAlertMessage ("Event Followed", "You have successfully followed the event.", "Ok");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Followed", "An error happened and you have not yet followed the event, try again.", "Ok");
				}
			}
		}

		public async void declineEvent(Event eve)
		{
			bool hasFollowed = await EventApiManager.DeclineEventInvite (eve.EventId, App.userProfile.ProfileId);
			if (!hasFollowed) {
				await App.coreView.displayAlertMessage ("Event Not Declined", "An error happened and you have not yet declined the invite, try again.", "Ok");
			}
		}

		public bool IsProfileYou(Profile profile)
		{
			bool you = false;
			if (profile.ProfileId == App.userProfile.ProfileId) {
				you = true;
			}
			return you;
		}

		public bool IsProfileFriend(Profile profile)
		{
			bool friend = false;
			var yourFriends = App.userProfile.Friends;
			for (int i = 0; i < yourFriends.Count; i++) {
				if (profile.ProfileId == yourFriends [i].ProfileId) {
					friend = true;
				}
			}
			return friend;
		}

		public bool HasProfileSentYouFriendRequest(Profile profile)
		{
			bool requested = false;
			var yourRecievedFriendRequests = App.userProfile.RecievedFriendRequests;
			for (int i = 0; i < yourRecievedFriendRequests.Count; i++) {
				if (profile.ProfileId == yourRecievedFriendRequests [i].ProfileId) {
					requested = true;
				}
			}
			return requested;
		}

		public bool HaveYouSentProfileFriendRequest(Profile profile)
		{
			bool requested = false;
			var yourSentFriendRequests = App.userProfile.SentFriendRequests;
			for (int i = 0; i < yourSentFriendRequests.Count; i++) {
				if (profile.ProfileId == yourSentFriendRequests [i].ProfileId) {
					requested = true;
				}
			}
			return requested;
		}

		public bool IsEventYours(Event eve)
		{
			bool yours = false;
			if (eve.Owner.ProfileId == App.userProfile.ProfileId) {
				yours = true;
			}
			return yours;
		}

		public bool IsEventJoined(Event eve)
		{
			bool yours = false;
			if (IsEventYours (eve)) {
				yours = true;
			} else {
				for(int i = 0; i < eve.Attendees.Count; i++) {
					if (eve.Attendees [i].ProfileId == App.userProfile.ProfileId) {
						yours = true;
					}
				}
			}
			return yours;
		}

		public bool AreYouGroupOwner(Group group)
		{
			bool you = false;
			if (group.Owner.ProfileId == App.userProfile.ProfileId) {
				you = true;
			}
			return you;
		}

		public bool AreYouGroupMember(Group group)
		{
			bool you = false;
			for (int i = 0; i < group.Members.Count; i++) {
				if (group.Members [i].ProfileId == App.userProfile.ProfileId) {
					you = true;
				}
			}
			return you;
		}
		public bool AreYouInvitedToGroup(Group group)
		{
			bool you = false;
			for (int i = 0; i < App.userProfile.GroupsInviteTo.Count; i++) {
				if (App.userProfile.GroupsInviteTo [i].GroupId == group.GroupId) {
					you = true;
				}
			}
			return you;
		}
	}
}
