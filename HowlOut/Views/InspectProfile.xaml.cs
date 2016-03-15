using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		
		DataManager dataManager = new DataManager ();
		UtilityManager utilityManager = new UtilityManager ();
		Profile inspectedProfile;

		bool isProfileYou = false;
		bool isProfileFriend = false;
		bool hasProfileSentYouFriendRequest = false;
		bool haveYouSentProfileFriendRequest = false;

		public InspectProfile (Profile profile)
		{
			InitializeComponent ();

			if (IsProfileYou (profile)) {
				System.Diagnostics.Debug.WriteLine ("Profile is you");
				friendButton.IsVisible = false;
				unFriendButton.IsVisible = false;
				isProfileYou = true;
				System.Diagnostics.Debug.WriteLine (profile.SentFriendRequests.Count + "" + profile.RecievedFriendRequests.Count);
			} else {
				if (IsProfileFriend (profile)) {
					System.Diagnostics.Debug.WriteLine ("Profile is friend");
					friendButton.IsVisible = false;
					isProfileFriend = true;
				} else if (HasProfileSentYouFriendRequest (profile)) {
					System.Diagnostics.Debug.WriteLine ("Profile want's to be your friend");
					hasProfileSentYouFriendRequest = true;
					friendButton.Text = "Accept Request";
					unFriendButton.Text = "Decline Request";
				} else if (HaveYouSentProfileFriendRequest (profile)) {
					System.Diagnostics.Debug.WriteLine ("Profile is waiting to reply to your friend request");
					friendButton.IsVisible = false;
					unFriendButton.IsVisible = false;
					waitButton.IsVisible = true;
					haveYouSentProfileFriendRequest = true;
				} else {
					unFriendButton.IsVisible = false;
					System.Diagnostics.Debug.WriteLine ("Profile is not your friend");
				}
			}

			inspectedProfile = profile;

			Likes.Text = profile.Likes + "";
			Loyalty.Text = profile.LoyaltyRating + "";
			NameAndAge.Text = profile.Name + ", " + profile.Age;

			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);

			profileLayout.Content = new ProfileDesignView (profile, null, 80, true);

			friendButton.Clicked += (sender, e) => {
				if(hasProfileSentYouFriendRequest) {
					utilityManager.acceptFriendRequest(profile);
				} else {
					utilityManager.sendFriendRequest(profile);
				}
			};
			unFriendButton.Clicked += (sender, e) => {
				if(isProfileFriend) {
					utilityManager.removeFriend(profile);
				} else if(hasProfileSentYouFriendRequest) {
					utilityManager.declineFriendRequest(profile);
				}
			};
		}
			
		private bool IsProfileYou(Profile profile)
		{
			bool you = false;
			if (profile.ProfileId == App.userProfile.ProfileId) {
				you = true;
			}
			return you;
		}

		private bool IsProfileFriend(Profile profile)
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

		private bool HasProfileSentYouFriendRequest(Profile profile)
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

		private bool HaveYouSentProfileFriendRequest(Profile profile)
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
	}
}

