using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ProfileDesignView : ContentView
	{
		DataManager _dataManager;
		public ProfileDesignView (Profile profile, Group groupInvitedTo, Event eventInvitedTo, int dimentions, Design design, bool clickable)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			ScaleLayout (profile, dimentions, design);

			setTypeSpecificDesign (profile, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				if(clickable) {
					App.coreView.setContentViewWithQueue(new InspectController(profile,null,null),"");
				}
			};

			acceptButton.Clicked += (sender, e) => {
				if(profile != null) {
					if (design.Equals (Design.InviteProfileToEvent)) {
						sendEventInviteToProfile(profile, eventInvitedTo);
					} else if(design.Equals(Design.InviteProfileToGroup)) {
						sendGroupInviteToProfile(profile, groupInvitedTo);
					} else if (_dataManager.IsProfileYou (profile)) {
						//Edit Profile ??
					} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
						_dataManager.acceptFriendRequest(profile, true);
					} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
						//Cancel the sent friend request
					} else {
						_dataManager.sendFriendRequest(profile);
					}
				} 
			};

			declineButton.Clicked += (sender, e) => {
				if(profile != null) {
					if(_dataManager.IsProfileFriend (profile)) {
						_dataManager.removeFriend(profile);
					} else if(_dataManager.HasProfileSentYouFriendRequest (profile)) {
						_dataManager.declineFriendRequest(profile);
					}
				} 
			};
		}

		private void ScaleLayout(Profile profile, int dimentions, Design design){

			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			if (design.Equals (Design.Plain)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				infoLayout.IsVisible = false;
			} else if (design.Equals (Design.WithName)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.2 });
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 0.2});
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.6 });
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 0.3});
				profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 0.3});
			}


			infoLabel.FontSize = (int) (0.1 * dimentions);
			acceptButton.FontSize = (int) (0.115 * dimentions);
			declineButton.FontSize = (int) (0.115 * dimentions);
			Likes.BorderRadius = (int) (0.175 * dimentions);
			Loyalty.BorderRadius = (int)(0.175 * dimentions);
			Likes.BorderWidth = (int) (0.025 * dimentions);
			Loyalty.BorderWidth = (int)(0.025 * dimentions);

			Likes.Text = "";
			Loyalty.Text = "";

			if (profile != null) {
				infoLabel.Text = profile.Name + ", " + profile.Age;
				infoLabel.HeightRequest = dimentions * 0.3;
				ProfileImage.Source = "https://graph.facebook.com/v2.5/" + profile.ProfileId + "/picture?height=" + dimentions + "&width=" + dimentions;
				ProfileImage.IsVisible = true;
			} 
		}

		private void setTypeSpecificDesign(Profile profile, int dimentions, Design design) {
			if(profile != null) {
				if (design.Equals (Design.WithDescription)) {
					declineButton.IsVisible = false;
					acceptButton.IsEnabled = false;
					if (_dataManager.IsProfileYou (profile)) {
						acceptButton.Text = " You ";
					} else if (_dataManager.IsProfileFriend (profile)) {
						acceptButton.Text = " Friend ";
					} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
						acceptButton.Text = " Sent You Request ";
					} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
						acceptButton.Text = " Request Sent ";
					} else {
						acceptButton.IsVisible = false;
					}
				} else if (design.Equals (Design.WithOptions)) {
					if (_dataManager.IsProfileYou (profile)) {
						acceptButton.Text = " Edit ";
						acceptButton.IsEnabled = false;
						declineButton.IsVisible = false;
					} else if (_dataManager.IsProfileFriend (profile)) {
						acceptButton.IsVisible = false;
						declineButton.Text = " Remove Friend ";
					} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
						acceptButton.Text = " Accept ";
						declineButton.Text = " Decline ";
					} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
						acceptButton.Text = " Request Sent ";
						declineButton.IsVisible = false;
					} else {
						acceptButton.Text = " Add Friend ";
						declineButton.IsVisible = false;
					}
				}
				Loyalty.IsVisible = false;
			}

			if (design.Equals (Design.InviteProfileToEvent) || design.Equals(Design.InviteProfileToGroup)) {
				acceptButton.Text = " Invite ";
				acceptButton.IsVisible = true;
				declineButton.IsVisible = false;
			} 
		}

		private async void sendEventInviteToProfile(Profile profile, Event eve) {
			bool success = await _dataManager.sendProfileInviteToEvent(eve, profile);
			if (success) {
				acceptButton.Text = " Invite Sent ";
				acceptButton.IsEnabled = false;
			}
		}

		private async void sendGroupInviteToProfile(Profile profile, Group group) {
			bool success = await _dataManager.sendProfileInviteToGroup(group, profile);
			if (success) {
				acceptButton.Text = " Invite Sent ";
				acceptButton.IsEnabled = false;
			}
		}


		public enum Design {
			Plain,
			WithName,
			WithDescription,
			WithOptions,
			InviteProfileToEvent,
			InviteProfileToGroup,
		}
	}
}

