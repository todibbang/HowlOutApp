using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		public InspectProfile (Profile profile)
		{
			InitializeComponent ();

			Likes.Text = profile.Likes + "";
			Loyalty.Text = profile.LoyaltyRating + "";
			NameAndAge.Text = profile.Name + ", " + profile.Age;

			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
		}
	}
}

