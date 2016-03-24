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

			setTypeSpecificDesign (profile, group, eve, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(profile,group,eve),"");
			};

			acceptButton.Clicked += (sender, e) => {
				if (design.Equals (ProfileDesign.InviteProfileToEvent)) {
					sendEventInviteToProfile(profile, eve);
				} 
				else if(design.Equals(ProfileDesign.InviteProfileToGroup)) {
					sendGroupInviteToProfile(profile, group);
				}
				else if(design.Equals (ProfileDesign.InviteGroupToEvent)) {
					sendEventInviteToGroup (group, eve);
				} else if(profile != null) {
					if (_dataManager.IsProfileYou (profile)) {
						//Edit Profile ??
					} else if (_dataManager.IsProfileFriend (profile)) {
						//Like
					} else if (_dataManager.HasProfileSentYouFriendRequest (profile)) {
						_dataManager.acceptFriendRequest(profile, true);
					} else if (_dataManager.HaveYouSentProfileFriendRequest (profile)) {
						//Cancel the sent friend request
					} else {
						_dataManager.sendFriendRequest(profile);
					}
					Loyalty.IsVisible = false;
				} else if (group != null) {
					if (_dataManager.AreYouGroupOwner (group) || _dataManager.AreYouGroupMember (group)) {
						App.coreView.setContentView (new InviteView (group, null, InviteView.WhatToShow.PeopleToInviteToGroup), "InviteView");
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						_dataManager.GroupApiManager.JoinGroup(group.GroupId, App.userProfile.ProfileId);
					} else {
						_dataManager.GroupApiManager.JoinGroup(group.GroupId, App.userProfile.ProfileId);
					}
				}
			};

			declineButton.Clicked += (sender, e) => {
				if(_dataManager.IsProfileFriend (profile)) {
					_dataManager.removeFriend(profile);
				} else if(_dataManager.HasProfileSentYouFriendRequest (profile)) {
					_dataManager.declineFriendRequest(profile);
				} else if (group != null) {
					if (_dataManager.AreYouGroupOwner (group)) {
						App.coreView.setContentView (new CreateGroup (), "");
					} else if (_dataManager.AreYouGroupMember (group)) {
						_dataManager.GroupApiManager.LeaveGroup(group.GroupId,App.userProfile.ProfileId);
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						_dataManager.GroupApiManager.DeclineGroupInvite(group.GroupId, App.userProfile.ProfileId);
					} 
				} 
			};





		}

		private void ScaleLayout(Profile profile, Group group, Event eve, int dimentions, ProfileDesign design){

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });

			if (design.Equals (ProfileDesign.Plain)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			} else if (design.Equals (ProfileDesign.WithName)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.2 });
			} else {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions * 1.5 });
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

		private void setTypeSpecificDesign(Profile profile, Group group, Event eve, int dimentions, ProfileDesign design) {
			if(profile != null) {
				if (_dataManager.IsProfileYou (profile)) {
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
					acceptButton.Text = " Request Sent ";
					declineButton.IsVisible = false;
				} else {
					acceptButton.Text = " Add Friend ";
					declineButton.IsVisible = false;
				}
				Loyalty.IsVisible = false;
			} else if (group != null) {
				if (_dataManager.AreYouGroupOwner (group)) {
					acceptButton.Text = " Invite ";
					declineButton.Text = " Edit ";
				} else if (_dataManager.AreYouGroupMember (group)) {
					acceptButton.Text = " Invite ";
					declineButton.Text = " Leave ";
				} else if (_dataManager.AreYouInvitedToGroup(group)) {
					acceptButton.Text = " Accept ";
					declineButton.Text = " Decline ";
				} else {
					acceptButton.Text = " Join ";
					declineButton.IsVisible = false;
				}
				Loyalty.IsVisible = false;
			} else {
				Likes.IsVisible = false;
			}

			if (design.Equals (ProfileDesign.InviteProfileToEvent) || design.Equals(ProfileDesign.InviteProfileToGroup) || 
				design.Equals (ProfileDesign.InviteGroupToEvent)) {
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

		private async void sendEventInviteToGroup (Group group, Event eve) {
			for(int i = 0; i < group.Members.Count; i++) {
				await _dataManager.sendProfileInviteToEvent(eve, group.Members[i]);
			}
		}

		private async void sendGroupInviteToProfile(Profile profile, Group group) {
			bool success = await _dataManager.sendProfileInviteToGroup(group, profile);
			if (success) {
				acceptButton.Text = " Invite Sent ";
				acceptButton.IsEnabled = false;
			}
		}


		public enum ProfileDesign {
			Plain,
			WithName,
			WithButtons,
			InviteProfileToEvent,
			InviteProfileToGroup,
			InviteGroupToEvent
		}
	}
}

