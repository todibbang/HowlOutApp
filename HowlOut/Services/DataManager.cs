﻿using ModernHttpClient;
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

using System.Globalization;

namespace HowlOut
{
	public class DataManager
	{
		private HttpClient httpClient;
		private HttpClient httpClientNoHeader;
		public EventApiManager EventApiManager { get; set; }
		public ProfileApiManager ProfileApiManager { get; set; }
		public GroupApiManager GroupApiManager { get; set; }
		//public OrganizationApiManager OrganizationApiManager { get; set; }
		public MessageApiManager MessageApiManager { get; set; }
		public UtilityManager UtilityManager { get; set; }

		public DataManager ()
		{
			httpClient = new HttpClient (new NativeMessageHandler ());
			httpClientNoHeader = new HttpClient(new NativeMessageHandler());
			httpClient.DefaultRequestHeaders.Add("apiKey", App.StoredApiKey);
			EventApiManager = new EventApiManager (httpClient);
			ProfileApiManager = new ProfileApiManager (httpClient);
			GroupApiManager = new GroupApiManager (httpClient);
			//OrganizationApiManager = new OrganizationApiManager(httpClient);
			MessageApiManager = new MessageApiManager(httpClient);
			UtilityManager = new UtilityManager ();
		}

		public Uri GetFacebookProfileImageUri (string facebookUserId)
		{
			return new Uri ("https://graph.facebook.com/v2.5/" + facebookUserId + "/picture?height=300&width=300");
		}

		public async Task update ()
		{
			App.userProfile = await ProfileApiManager.GetLoggedInProfile ();
		}



		public async Task<ObservableCollection<Address>> AutoCompletionPlace (string input)
		{
			System.Diagnostics.Debug.WriteLine("Before3");
			string path = "https://dawa.aws.dk/autocomplete?q=" + input;
			ObservableCollection<Address> addresses = new ObservableCollection<Address> ();

			try { 
				var response = await httpClientNoHeader.GetAsync (new Uri (path));
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
			System.Diagnostics.Debug.WriteLine("After3");
			return addresses;

		}

		public async Task<Position> GetCoordinates (string input)
		{
			//input = Regex.Replace (input, "http", "https");

			string path = input;

			string adgangspunkt = "";

			Position position = new Position ();

			try { 
				var response = await httpClientNoHeader.GetAsync (new Uri (path));

				System.Diagnostics.Debug.WriteLine ("Success getting new coords");

				if (response.IsSuccessStatusCode) {
					System.Diagnostics.Debug.WriteLine ("Success getting new coords");

					adgangspunkt = await response.Content.ReadAsStringAsync ();
					//adgangspunkt = JsonConvert.DeserializeObject<string>(content);

					System.Diagnostics.Debug.WriteLine (adgangspunkt);
					//Regex.Replace(adgangspunkt, ".", ",");
					var substrings = Regex.Split (adgangspunkt, "koordinater");

					position = new Position (double.Parse(substrings [1].Substring (35, 16), CultureInfo.InvariantCulture), double.Parse(substrings [1].Substring (11, 16), CultureInfo.InvariantCulture));
					//position.Latitude = double.Parse(substrings[1].Substring(35, 16));
					//position


					//System.Diagnostics.Debug.WriteLine(substrings[1].Substring(35, 16) + ", " + substrings[1].Substring(11, 16));	
					//System.Diagnostics.Debug.WriteLine(double.Parse(substrings[1].Substring(35, 16), CultureInfo.InvariantCulture) +", "+ position.Longitude);
					//position.Latitude = Convert.ToDouble(substrings [1].Substring (11, 16));
					//position.Longitude = Convert.ToDouble(substrings [1].Substring (35, 16));
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("COULD NOT RECIEVE DATA!!!!!");
				System.Diagnostics.Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}


			return position;
		}


		public async Task<bool> addFriendRequest(Profile profile, bool acceptOrDecline)
		{
			bool success = await ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, acceptOrDecline);
			if (success) {
				App.userProfile = await ProfileApiManager.GetLoggedInProfile ();
				await loadUpdatedProfile(profile);
				App.coreView.setContentViewReplaceCurrent(new InspectController(profile), 1);
				App.coreView.updateHomeView();
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not sent, try again.", "Ok");
			}
			return success;
		}

		public async Task<bool> sendProfilesInviteToEvent(Event eve, List<Profile> profiles)
		{
			bool success = await EventApiManager.InviteProfilesToEvent(eve.EventId, profiles);
			if (!success)
			{
				await App.coreView.displayAlertMessage("Error", "An error happened and one or more profiles was not invited", "Ok");
				return false;
			}
			else {
				return true;
			}
		}

		public async Task<bool> AttendTrackEvent(Event eve, bool attendOrUnattend, bool joinOrTrack)
		{
			var Continue = false;
			string action = "";
			if (joinOrTrack)
			{
				if (attendOrUnattend)
				{
					action = "You are about to join an event, continue?";
					Continue = await App.coreView.displayConfirmMessage("Join", action, "Yes", "No");
				}
				else {
					action = "You are about to leave an event, continue?";
					Continue = await App.coreView.displayConfirmMessage("Leave", action, "Yes", "No");
				}
			}
			else {
				Continue = true;
			}


			if (Continue)
			{
				bool success = await EventApiManager.AttendOrTrackEvent(eve.EventId, attendOrUnattend, joinOrTrack);
				if (!success)
				{
					await App.coreView.displayAlertMessage("Error", "An error happened and one or more profiles was not invited", "Ok");
					return false;
				}
				else {
					if (App.coreView.contentViews.Count > 1)
					{
						App.coreView.setContentViewReplaceCurrent(new InspectController(await App.coreView._dataManager.EventApiManager.GetEventById(eve.EventId)), 1);
					}
					App.coreView.updateHomeView();
					return true;
				}
			}
			App.coreView.GetLoggedInProfile();
			return Continue;
		}



		public async Task<bool> sendProfileInviteToGroup(Group group, List<Profile> profiles)
		{
			bool success = await GroupApiManager.InviteDeclineToGroup(group.GroupId, true, profiles);
			if (!success)
			{
				await App.coreView.displayAlertMessage("Error", "An error happened", "Ok");
				return false;
			}
			else {
				return true;
			}

			System.Diagnostics.Debug.WriteLine("Boo");
		}



		private async Task loadUpdatedProfile(Profile profile)
		{
			App.userProfile = await ProfileApiManager.GetLoggedInProfile ();
			InspectController inspect = new InspectController(profile);
			App.coreView.setContentViewWithQueue (inspect);
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
			if ((eve.ProfileOwners != null && eve.ProfileOwners.Exists(p => p.ProfileId == App.StoredUserFacebookId)) || (eve.GroupOwner != null && eve.GroupOwner.ProfileOwners.Exists(p => p.ProfileId == App.StoredUserFacebookId)))
			{
				yours = true;
			}
			return yours;
		}

		public bool IsEventJoined(Event eve)
		{
			bool yours = false;
			/*
			if (IsEventYours (eve)) {
				return true;
			}  */
			for (int i = 0; i < eve.Attendees.Count; i++) {
				if (eve.Attendees[i].ProfileId == App.userProfile.ProfileId) {
					yours = true;
				}
			}
			return yours;
		}

		public bool AreYouGroupOwner(Group group)
		{
			bool you = false;
			if (group.ProfileOwners != null && group.ProfileOwners.Exists(p => p.ProfileId == App.StoredUserFacebookId)) {
				you = true;
			}
			return you;
		}

		public bool AreYouGroupMember(Group group)
		{
			bool you = false;
			if (AreYouGroupOwner(group))
			{
				return true;
			}
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

		public bool checkIfUnseen(string modelId, NotificationModelType modelType)
		{
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				if (n.ModelId == modelId && !n.Seen && n.ModelType == modelType)
				{
					return true;
				}
			}
			return false;
		}

		public bool chechIfConversationUnseen(ConversationModelType cType, string cId)
		{
			NotificationModelType modelType = NotificationModelType.ProfileConversation;
			if(cType == ConversationModelType.Event ) modelType = NotificationModelType.EventConversation;
			if (cType == ConversationModelType.Group) modelType = NotificationModelType.GroupConversation;


			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				if (modelType == NotificationModelType.ProfileConversation)
				{
					if (n.ModelId == cId && !n.Seen && n.ModelType == modelType)
					{
						return true;
					}
				}
				else {
					if (n.SecondModelId == cId && !n.Seen && n.ModelType == modelType)
					{
						return true;
					}
				}
			}
			return false;
		}

		public async Task setConversationSeen(string modelId, ConversationModelType mType)
		{
			NotificationModelType modelType = NotificationModelType.ProfileConversation;
			if (mType == ConversationModelType.Event) modelType = NotificationModelType.EventConversation;
			if (mType == ConversationModelType.Group) modelType = NotificationModelType.GroupConversation;
			List<Notification> notiToRemove = new List<Notification>();;
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				System.Diagnostics.Debug.WriteLine(n.ModelId + ", " + n.ModelType);

				if (((n.SecondModelId == modelId && modelType != NotificationModelType.ProfileConversation) || 
				     (n.ModelId == modelId && modelType == NotificationModelType.ProfileConversation)) 
				    && !n.Seen && n.ModelType == modelType)
				{
					n.Seen = true;
					notiToRemove.Add(n);
					await MessageApiManager.SetNotificationSeen(n.InAppNotificationId);
				}
			}
			foreach (Notification n in notiToRemove) App.coreView.notifications.unseenNotifications.Remove(n);
			//await App.coreView.notifications.UpdateNotifications(false);
			await App.coreView.yourConversatios.UpdateConversations(false);
			//await App.coreView.otherConversatios.UpdateConversations(false);
		}

		public async Task setUpdateSeen(string modelId, NotificationModelType modelType)
		{
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				System.Diagnostics.Debug.WriteLine(n.ModelId + ", " + n.ModelType);

				if (n.ModelId == modelId && !n.Seen && n.ModelType == modelType)
				{
					n.Seen = true;
					await MessageApiManager.SetNotificationSeen(n.InAppNotificationId);
				}
			}
			await App.coreView.notifications.UpdateNotifications(false);
			if (modelType == NotificationModelType.Event) { App.coreView.joinedEvents.UpdateList(false, ""); }
			else if (modelType == NotificationModelType.ProfileConversation) { 
				await App.coreView.yourConversatios.UpdateConversations(false);
				//await App.coreView.otherConversatios.UpdateConversations(false);
			}
		}

		public async Task setNotificationSeen(string id)
		{
			App.coreView.notifications.unseenNotifications.Find(n => n.InAppNotificationId == id).Seen = true;
			await App.coreView.notifications.UpdateNotifications(false);   
			await MessageApiManager.SetNotificationSeen(id);
		}
	}
}
