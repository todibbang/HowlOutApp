using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		DataManager dataManager = new DataManager ();
		Profile inspectedProfile;

		public InspectProfile (Profile profile)
		{
			InitializeComponent ();
			inspectedProfile = profile;

			Likes.Text = profile.Likes + "";
			Loyalty.Text = profile.LoyaltyRating + "";
			NameAndAge.Text = profile.Name + ", " + profile.Age;

			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);

			friendRequestButton.Clicked += (sender, e) => 
			{

			};
		}

		private async void sendFriendRequest()
		{
			bool success = await dataManager.sendFriendRequest (App.userProfile, inspectedProfile);
			if (success) {
				friendRequestButton.Text = "Request sent";
			} else {
				await App.coreView.displayAlertMessage ("Error", "Something happened and the friend request was not sent, try again.", "Ok");
			}
		}
	}
}

