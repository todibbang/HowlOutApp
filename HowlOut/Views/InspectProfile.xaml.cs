using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{
		public InspectProfile (string facebookUserId)
		{
			InitializeComponent ();

			Likes.Text = "24";
			Loyalty.Text = "82%";
			NameAndAge.Text = "Tobias Bjerge Bang, 23";

			//var profilePicUri = new Uri("http://xamarin.com/content/images/pages/forms/example-app.png");
			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(facebookUserId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
		}
	}
}

