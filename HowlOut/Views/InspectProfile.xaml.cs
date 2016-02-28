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

			Likes.Text = "0";
			Loyalty.Text = "0";
			NameAndAge.Text = profile.Name + ", " + profile.Age;

			//var profilePicUri = new Uri("http://xamarin.com/content/images/pages/forms/example-app.png");
			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
		}
	}
}

