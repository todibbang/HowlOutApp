using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ProfileDesignView : ContentView
	{
		DataManager _dataManager;

		public ProfileDesignView (Profile profile, Group group, Event eve, int dimentions, ProfileDesign design)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			ScaleLayout (profile, group, eve, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(profile,group,null),"");
			};

			acceptButton.Clicked += (sender, e) => {
				if (design.Equals (ProfileDesign.Invite)) {
					sendInviteToEvent(profile, eve);
				} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
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
			if (profile != null) {
				if (design.Equals (ProfileDesign.Invite)) {
					acceptButton.Text = " Invite ";
					declineButton.IsVisible = false;
				} else if (_dataManager.IsProfileYou (profile)) {
					acceptButton.Text = " YOU ";
					acceptButton.IsEnabled = false;
					declineButton.IsVisible = false;
				} else if (_dataManager.IsProfileFriend (profile)) {
					acceptButton.IsVisible = false;
					declineButton.Text = " Remove Friend ";
				} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
					System.Diagnostics.Debug.WriteLine (profile.Name + " has sent you a friendRequest");
					acceptButton.Text = " Accept ";
					declineButton.Text = " Decline ";
				} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
					acceptButton.Text = " Friend Request Sent ";
					declineButton.IsVisible = false;
				} else {
					acceptButton.Text = " Send Friend Request ";
					declineButton.IsVisible = false;
				}
			} else {
				acceptButton.IsEnabled = false;
				declineButton.IsVisible = false;
			}
		}

		private void ScaleLayout(Profile profile, Group group, Event eve, int dimentions, ProfileDesign design){

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });

			if (profile != null) {
				if (design.Equals (ProfileDesign.WithButtons) || design.Equals (ProfileDesign.Invite)) {
					profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.5 });
				} else if (design.Equals (ProfileDesign.WithName)) {
					profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.2 });
				} else {
					profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
					infoLabel.IsVisible = false;
				}
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				infoLabel.IsVisible = false;
			}


			infoLabel.FontSize = (int) (0.1 * dimentions);
			acceptButton.FontSize = (int) (0.1 * dimentions);
			declineButton.FontSize = (int) (0.1 * dimentions);
			Likes.BorderRadius = (int) (0.175 * dimentions);
			Loyalty.BorderRadius = (int)(0.175 * dimentions);
			Likes.BorderWidth = (int) (0.025 * dimentions);
			Loyalty.BorderWidth = (int)(0.025 * dimentions);

			Likes.Text = "";
			Loyalty.Text = "";

			if (profile != null) {
				infoLabel.Text = profile.Name + ", " + profile.Age;
				ProfileImage.Source = "https://graph.facebook.com/v2.5/" + profile.ProfileId + "/picture?height=" + dimentions + "&width=" + dimentions;
				ProfileImage.IsVisible = true;
			} else {
				MainButton.IsVisible = true;
				MainButton.BorderRadius = (int) (0.375 * dimentions);
				MainButton.BorderWidth = (int) (0.04 * dimentions);
				if (group != null) {
					MainButton.Text = group.Members.Count + "";
					infoLabel.Text = group.Name;
				} else if (eve != null){
					MainButton.Text = eve.Attendees.Count + "/" + eve.MaxSize;
					infoLabel.Text = eve.Title;
				}
			}
		}

		private async void sendInviteToEvent(Profile profile, Event eve) {
			bool success = await _dataManager.sendInviteToEvent(eve, profile);
			if (success) {
				acceptButton.Text = " Invite Sent ";
				acceptButton.IsEnabled = false;
			}
		}


		public enum ProfileDesign {
			Plain,
			WithName,
			WithButtons,
			Invite
		}
	}
}

