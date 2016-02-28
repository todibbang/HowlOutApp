using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class UserProfile : ContentView
	{
		public UserProfile ()
		{
			InitializeComponent ();

			Likes.Text = "2";
			Loyalty.Text = "100%";
			NameAndAge.Text = "Name and Age";

			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(App.StoredUserFacebookId);
			Image.Source = ImageSource.FromUri(profilePicUri);
		}
	}
}

