using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class ProfileDesignView : ContentView
	{
		DataManager _dataManager;
		public ProfileDesignView (Profile profile, Group grp, Event eve, int dimentions, Design design, Show show, bool clickable)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();

			setTypeSpecificDesign(profile, grp, eve, design, show);

			ScaleLayout (profile, grp, dimentions, show);

			SubjectButton.Clicked += (sender, e) => {
				if(clickable) {
					InspectController inspect = null;
					if (show == Show.Profile) { inspect = new InspectController(profile, null, null); }
					if (show == Show.Group) { inspect = new InspectController(null, grp, null); }
					App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
				}
			};

			inviteButton.Clicked += (sender, e) =>
			{
				if (eve != null) { sendInvite(profile, eve, null); }
				else if(grp != null) { sendInvite(profile, null, grp); }
			};

			editButton.Clicked += async (sender, e) =>
			{
				if (editProfile.IsVisible)
				{
					await App.scrollTo(0);
					await Task.Delay(500);
					editProfile.IsVisible = false;
				}
				else {
					editProfile.IsVisible = true;
					await App.scrollTo(editButton);
				}
			};

			sendFriendRequestButton.Clicked += (sender, e) =>
			{
				if (show == Show.Profile)
				{
					_dataManager.sendFriendRequest(profile);
				}
				else if (show == Show.Group)
				{
					_dataManager.GroupApiManager.JoinGroup(grp.GroupId);
				}
			};

			removeFriendButton.Clicked += (sender, e) => { 
				if (show == Show.Profile)
				{
					_dataManager.removeFriend(profile);
				}
				else if (show == Show.Group)
				{
					_dataManager.GroupApiManager.LeaveGroup(grp.GroupId, App.userProfile.ProfileId);
				}
			};

			acceptFriendRequestButton.Clicked += async (sender, e) => { 
				if (show == Show.Profile)
				{
					await _dataManager.acceptFriendRequest(profile, true);
				}
				else if (show == Show.Group)
				{
					acceptGroupInvite(grp);
				}
			};

			declineFriendRequestButton.Clicked += (sender, e) => {  
				if (show == Show.Profile)
				{
					_dataManager.declineFriendRequest(profile);
				}
				else if (show == Show.Group)
				{
					_dataManager.GroupApiManager.DeclineGroupInvite(grp.GroupId, App.userProfile.ProfileId);
				}

			};

			acceptJoinRequestButton.Clicked += async (sender, e) => { await _dataManager.EventApiManager.AcceptJoinRequest(profile.ProfileId, true); };

			declineJoinRequestButton.Clicked += async (sender, e) => { await _dataManager.EventApiManager.AcceptJoinRequest(profile.ProfileId, false); };

			inviteProfilesToGroupButton.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InviteView(grp, null, InviteView.WhatToShow.PeopleToInviteToGroup), "InviteView", null); };
		}

		private void ScaleLayout(Profile profile, Group grp, int dimentions, Show show){

			pictureGrid.HeightRequest = dimentions;
			pictureGrid.WidthRequest = dimentions;
			infoLayout.HeightRequest = dimentions * 0.1;
			buttonLayout.HeightRequest = dimentions * 0.16;
			infoLabel.FontSize = dimentions * 0.115;

			MainButton.BorderRadius = (int)(0.440 * dimentions);
			MainButton.BorderWidth = (int)(0.04 * dimentions);
			MainButton.FontSize = (int)(0.2 * dimentions);


			setButtonDimentions(inviteButton, dimentions);
			setButtonDimentions(editButton, dimentions);
			setButtonDimentions(sendFriendRequestButton, dimentions);
			setButtonDimentions(removeFriendButton, dimentions);
			setButtonDimentions(acceptFriendRequestButton, dimentions);
			setButtonDimentions(declineFriendRequestButton, dimentions);
			setButtonDimentions(acceptJoinRequestButton, dimentions);
			setButtonDimentions(declineJoinRequestButton, dimentions);
			setButtonDimentions(inviteProfilesToGroupButton, dimentions);

			if (show == Show.Profile)
			{
				infoLabel.Text = profile.Name + ", " + profile.Age;
				ProfileImage.Source = "https://graph.facebook.com/v2.5/" + profile.ProfileId + "/picture?height=" + dimentions + "&width=" + dimentions;
			}
			else if (show == Show.Group)
			{
				ProfileImage.Source = "https://graph.facebook.com/v2.5/" + grp.Owner.ProfileId + "/picture?height=" + dimentions + "&width=" + dimentions;
				MainButton.IsVisible = true;
				infoLabel.Text = grp.Name;
				MainButton.Text = grp.NumberOfMembers + "";
				if (grp.NumberOfMembers == 0)
				{
					MainButton.Text = grp.Members.Count + 1 + "";
				}
			}
		}

		void setButtonDimentions(Button b, int dimentions)
		{
			b.BorderRadius = (int)(0.08 * dimentions);
			b.BorderWidth = (0.003 * dimentions);
			b.WidthRequest = (dimentions * 0.6);
			b.FontSize = (int)(0.115 * dimentions);
		}


		private void setTypeSpecificDesign(Profile profile, Group grp, Event eve, Design design, Show show) {

			if (design == Design.Inspect)
			{
				if (show == Show.Profile)
				{

					if (_dataManager.IsProfileYou(profile))
					{
						editButton.IsVisible = true;
						userProfileSettings(App.userProfile);
					}
					else if (_dataManager.IsProfileFriend(profile))
					{
						removeFriendButton.IsVisible = true;
					}
					else {
						if (_dataManager.HasProfileSentYouFriendRequest(profile))
						{
							acceptFriendRequestButton.IsVisible = true;
							declineFriendRequestButton.IsVisible = true;
						}
						else {
							sendFriendRequestButton.IsVisible = true;
						}
					}
				}
				else if (show == Show.Group)
				{
					if (_dataManager.AreYouGroupOwner(grp))
					{
						editButton.IsVisible = true;
						inviteProfilesToGroupButton.IsVisible = true;
					}
					else if (_dataManager.AreYouGroupMember(grp))
					{
						removeFriendButton.IsVisible = true;
					}
					else {
						if (_dataManager.AreYouInvitedToGroup(grp))
						{
							acceptFriendRequestButton.IsVisible = true;
							declineFriendRequestButton.IsVisible = true;
						}
					}
				}

			}
			else if (design == Design.Invite)
			{
				if (show == Show.Profile)
				{
					inviteButton.IsVisible = true;
				}
				else if (show == Show.Group)
				{
					inviteButton.IsVisible = true;
				}
			}
			else if (design == Design.ListAsOwner)
			{


				/*
				if (eve != null)
				{
					if (eve.RequestingToJoin.Exists(p => p.ProfileId == profile.ProfileId))
					{
						acceptJoinRequestButton.IsVisible = true;
						declineJoinRequestButton.IsVisible = true;
					}
				}
				*/


			}

		}

		private async void sendInvite(Profile profile, Event eve, Group grp) {
			bool success = false;
			if (eve != null) { success = await _dataManager.sendProfileInviteToEvent(eve, profile); }
			else if (grp != null) { success = await _dataManager.sendProfileInviteToGroup(grp, profile); }
			if (success) {
				inviteButton.Text = " Invited ";
				inviteButton.IsEnabled = false;
			}
		}

		private async void sendEventInviteToGroup(Group group, Event eve)
		{
			group = await _dataManager.GroupApiManager.GetGroupById(group.GroupId);
			for (int i = 0; i < group.Members.Count; i++)
			{
				await _dataManager.sendProfileInviteToEvent(eve, group.Members[i]);
			}
			inviteButton.Text = " Invited ";
			inviteButton.IsEnabled = false;
		}

		private async void acceptGroupInvite(Group group)
		{
			bool success = await _dataManager.GroupApiManager.JoinGroup(group.GroupId);
			if (success)
			{
				inviteButton.Text = " Joined ";
				inviteButton.IsEnabled = false;
			}
		}


		public enum Design {
			Inspect,
			ListSimple,
			ListAsOwner,
			Invite
		}

		public enum Show
		{
			Profile,
			Group,
			Event
		}



		void userProfileSettings(Profile user)
		{
			description.Text = user.Description;


			updateProfileButton.Clicked += (sender, e) =>
			{
				_dataManager.ProfileApiManager.UpdateProfile(user);
			};

			logOutButton.Clicked += (sender, e) =>
			{
				
			};
		}
	}
}

