﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class GroupDesignView : ContentView
	{
		DataManager _dataManager;
		public GroupDesignView (Group group, Event eventInvitedTo, int dimentions, Design design)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			ScaleLayout (group, dimentions, design);
			setTypeSpecificDesign (group, dimentions, design);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(null,group,null),"");
			};

			acceptButton.Clicked += (sender, e) => {
				if (group != null) {
					if(design.Equals (Design.InviteGroupToEvent)) {
						sendEventInviteToGroup (group, eventInvitedTo);
					} else if (_dataManager.AreYouGroupOwner (group) || _dataManager.AreYouGroupMember (group)) {
						App.coreView.setContentView (new InviteView (group, null, InviteView.WhatToShow.PeopleToInviteToGroup), "InviteView");
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						_dataManager.GroupApiManager.JoinGroup(group.GroupId, App.userProfile.ProfileId);
					} else {
						_dataManager.GroupApiManager.JoinGroup(group.GroupId, App.userProfile.ProfileId);
					}
				}
			};

			declineButton.Clicked += (sender, e) => {
				if (group != null) {
					if (_dataManager.AreYouGroupOwner (group)) {
						App.coreView.setContentView (new CreateGroup (group), "");
					} else if (_dataManager.AreYouGroupMember (group)) {
						_dataManager.GroupApiManager.LeaveGroup(group.GroupId,App.userProfile.ProfileId);
					} else if (_dataManager.AreYouInvitedToGroup(group)) {
						_dataManager.GroupApiManager.DeclineGroupInvite(group.GroupId, App.userProfile.ProfileId);
					} 
				} 
			};
		}

		private void ScaleLayout(Group group, int dimentions, Design design){

			profileGrid.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });
			profileGrid.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			profileLayout.ColumnDefinitions.Add (new ColumnDefinition{ Width = dimentions });

			if (design.Equals (Design.Plain)) {
				profileLayout.RowDefinitions.Add (new RowDefinition{ Height = dimentions });
			} else if (design.Equals (Design.WithName)) {
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

			MainButton.IsVisible = true;
			MainButton.BorderRadius = (int) (0.375 * dimentions);
			MainButton.BorderWidth = (int) (0.04 * dimentions);
			if (group != null) {
				MainButton.Text = group.Members.Count + 1 + "";
				infoLabel.Text = group.Name;
			}
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

				Loyalty.IsVisible = false;
			}

			if (design.Equals (Design.InviteGroupToEvent)) {
				acceptButton.Text = " Invite ";
				acceptButton.IsVisible = true;
				declineButton.IsVisible = false;
			} 
		}

		private async void sendEventInviteToGroup (Group group, Event eve) {
			for(int i = 0; i < group.Members.Count; i++) {
				await _dataManager.sendProfileInviteToEvent(eve, group.Members[i]);
			}
			acceptButton.Text = " Invite Sent ";
			acceptButton.IsEnabled = false;
		}

		public enum Design {
			Plain,
			WithName,
			WithDescription,
			WithOptions,
			InviteGroupToEvent
		}
	}
}

