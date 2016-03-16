using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ProfileDesignView : ContentView
	{
		public ProfileDesignView (Profile profile, Group group, int dimentions, bool info)
		{
			InitializeComponent ();

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });

			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });

			if (info) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.4 });
				infoLabel.Text = profile.Name + ", " + profile.Age;
				infoLabel.FontSize = (int) (0.1 * dimentions);
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				infoLabel.IsVisible = false;
			}


			Likes.BorderRadius = (int) (0.175 * dimentions);
			Loyalty.BorderRadius = (int)(0.175 * dimentions);
			Likes.BorderWidth = (int) (0.025 * dimentions);
			Loyalty.BorderWidth = (int)(0.025 * dimentions);

			Likes.Text = "";
			Loyalty.Text = "";
			/*
			DataManager dataManager = new DataManager();
			var profilePicUri = dataManager.GetFacebookProfileImageUri(profile.ProfileId);
			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
*/
			ProfileImage.Source = "https://graph.facebook.com/v2.5/" + profile.ProfileId + "/picture?height="+dimentions+"&width="+dimentions;

			if (info) {
				
			} else {
				
			}
		}
	}
}

