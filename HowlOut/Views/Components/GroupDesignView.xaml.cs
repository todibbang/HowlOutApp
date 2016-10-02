using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class GroupDesignView : ContentView
	{
		/*
		DataManager _dataManager;
		public GroupDesignView (Group group, Event eventInvitedTo, int dimentions, Design design, bool clickable)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			setTypeSpecificDesign(group, dimentions, design);
			ScaleLayout (group, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				if (clickable)
				{
					InspectController inspect = new InspectController(null, group, null);
					App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
				}
			};

			acceptButton.Clicked += (sender, e) => {
				if (group != null) {
					if(design.Equals (Design.InviteGroupToEvent)) {
						sendEventInviteToGroup (group, eventInvitedTo);
					} else if (_dataManager.AreYouGroupOwner (group) || _dataManager.AreYouGroupMember (group)) {
						App.coreView.setContentViewWithQueue (new InviteView (group, null, InviteView.WhatToShow.PeopleToInviteToGroup), "InviteView", null);
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						acceptGroupInvite(group);
					} 
				}
			};

			declineButton.Clicked += (sender, e) => {
				if (group != null) {
					if (_dataManager.AreYouGroupOwner (group)) {
						App.coreView.setContentViewWithQueue (new CreateGroup (group), "", null);
					} else if (_dataManager.AreYouGroupMember (group)) {
						_dataManager.GroupApiManager.LeaveGroup(group.GroupId,App.userProfile.ProfileId);
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						_dataManager.GroupApiManager.DeclineGroupInvite(group.GroupId, App.userProfile.ProfileId);
					} 
				} 
			};
		}

		private void ScaleLayout(Group group, int dimentions, Design design){

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


			infoLabel.FontSize = (int)(0.1 * dimentions);
			buttonLayout.HeightRequest = (int)(0.16 * dimentions);
			acceptButton.BorderRadius = (int)(0.08 * dimentions);
			declineButton.BorderRadius = (int)(0.08 * dimentions);
			acceptButton.BorderWidth = (0.003 * dimentions);
			declineButton.BorderWidth = (0.003 * dimentions);


			acceptButton.WidthRequest = (acceptButton.Text.Length * .06 * dimentions);
			declineButton.WidthRequest = (acceptButton.Text.Length * .06 * dimentions);


			acceptButton.FontSize = (int)(0.115 * dimentions);
			declineButton.FontSize = (int)(0.115 * dimentions);

			MainButton.IsVisible = true;
			MainButton.BorderRadius = (int) (0.440 * dimentions);
			MainButton.BorderWidth = (int) (0.04 * dimentions);
			MainButton.FontSize = (int) (0.2 * dimentions);
			MainButton.Text = group.NumberOfMembers + "";
			if (group.NumberOfMembers == 0) {
				MainButton.Text = group.Members.Count + 1 + "";
			}
			infoLabel.Text = group.Name;
			ProfileImage.Source = "https://graph.facebook.com/v2.5/" + group.Owner.ProfileId + "/picture?height=" + dimentions + "&width=" + dimentions;
		}

		private void setTypeSpecificDesign(Group group, int dimentions, Design design) {
			if (group != null) {
				if (design.Equals (Design.WithDescription)) {
					declineButton.IsVisible = false;
					acceptButton.IsEnabled = false;
					if (_dataManager.AreYouGroupOwner (group)) {
						acceptButton.Text = " Owner ";
					} else if (_dataManager.AreYouGroupMember (group)) {
						acceptButton.Text = " Member ";
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						acceptButton.Text = " Invite Pending ";
					} else {
						acceptButton.IsVisible = false;
					}
				} else if (design.Equals (Design.WithOptions)) {
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
				}

				//Loyalty.IsVisible = false;
			}

			if (design.Equals (Design.InviteGroupToEvent)) {
				acceptButton.Text = " Invite ";
				acceptButton.IsVisible = true;
				declineButton.IsVisible = false;
			} 
		}

		private async void sendEventInviteToGroup (Group group, Event eve) {
			group = await _dataManager.GroupApiManager.GetGroupById (group.GroupId);
			for(int i = 0; i < group.Members.Count; i++) {
				await _dataManager.sendProfileInviteToEvent(eve, group.Members[i]);
			}
			acceptButton.Text = " Invited ";
			acceptButton.IsEnabled = false;
		}

		private async void acceptGroupInvite(Group group) {
			bool success = await _dataManager.GroupApiManager.JoinGroup(group.GroupId);
			if (success) {
				acceptButton.Text = " Joined ";
				acceptButton.IsEnabled = false;
			}
		}

		public enum Design {
			Plain,
			WithName,
			WithDescription,
			WithOptions,
			InviteGroupToEvent
		}
		*/
	}
}


