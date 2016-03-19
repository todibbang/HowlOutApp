using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ProfileDesignView : ContentView
	{
		DataManager _dataManager;

		public ProfileDesignView (Profile profile, Group group, int dimentions, ProfileDesign design)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			ScaleLayout (profile, group, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(profile,group,null),"");
			};

			acceptButton.Clicked += (sender, e) => {
				if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
					_dataManager.acceptFriendRequest(profile, true);
				} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
					acceptButton.Text = "Friend Request Sent";
					declineButton.IsVisible = false;
				} else if (_dataManager.IsProfileFriend (profile)) {
					
				} else {
					_dataManager.sendFriendRequest(profile);
				}
			};

			declineButton.Clicked += (sender, e) => {
				if(_dataManager.IsProfileFriend (profile)) {
					_dataManager.removeFriend(profile);
				} else if(_dataManager.HasProfileSentYouFriendRequest (profile)) {
					_dataManager.declineFriendRequest(profile);
				}
			};

			if (_dataManager.IsProfileYou (profile)) {
				acceptButton.Text = "YOU";
				acceptButton.IsEnabled = false;
				declineButton.IsVisible = false;
			} else if (_dataManager.IsProfileFriend (profile)) {
				acceptButton.IsVisible = false;
				declineButton.Text = "Remove Friend";
			} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
				acceptButton.Text = "Accept";
				declineButton.Text = "Decline";
			} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
				acceptButton.Text = "Friend Request Sent";
				declineButton.IsVisible = false;
			} 

		}

		private void ScaleLayout(Profile profile, Group group, int dimentions, ProfileDesign design){

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			if (design.Equals (ProfileDesign.WithButtons)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.5 });
			} else if(design.Equals (ProfileDesign.WithName)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.2 });
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				infoLabel.IsVisible = false;
			}

			infoLabel.Text = profile.Name + ", " + profile.Age;
			infoLabel.FontSize = (int) (0.1 * dimentions);
			acceptButton.FontSize = (int) (0.1 * dimentions);
			declineButton.FontSize = (int) (0.1 * dimentions);
			Likes.BorderRadius = (int) (0.175 * dimentions);
			Loyalty.BorderRadius = (int)(0.175 * dimentions);
			Likes.BorderWidth = (int) (0.025 * dimentions);
			Loyalty.BorderWidth = (int)(0.025 * dimentions);

			Likes.Text = "";
			Loyalty.Text = "";

			ProfileImage.Source = "https://graph.facebook.com/v2.5/" + profile.ProfileId + "/picture?height="+dimentions+"&width="+dimentions;

		}


		public enum ProfileDesign {
			Plain,
			WithName,
			WithButtons
		}
	}
}

