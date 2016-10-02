using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class NotificationView : ContentView
	{
		public NotificationView(Notification notification)
		{
			InitializeComponent();

			time.Text = notification.SendTime.ToString("dddd HH:mm - dd MMMMM yyyy");

			if (notification.HeaderProfiles[0] != null){
				image.Source = "https://graph.facebook.com/v2.5/" + notification.HeaderProfiles[0].ProfileId + "/picture?height=80&width=80";
			} else if(notification.ContentEvent != null) {
				if (notification.ContentEvent.Owner != null) {
					image.Source = "https://graph.facebook.com/v2.5/" + notification.ContentEvent.Owner.ProfileId + "/picture?height=80&width=80";
				}
				else {
					image.Source = notification.ContentEvent.OrganisationOwner.ImageSource;
				}

			} else if(notification.ContentGroup != null) {
				image.Source = "https://graph.facebook.com/v2.5/" + notification.ContentGroup.Owner.ProfileId + "/picture?height=80&width=80";
			}

			SubjectButton.Clicked += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine(notification.TypeOfMessage);

				if (notification.TypeOfMessage == Notification.NotificationType.PersonallyInvitedToEvent ||
					notification.TypeOfMessage == Notification.NotificationType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent ||
					notification.TypeOfMessage == Notification.NotificationType.FollowedProfileHasCreatedEvent ||
				   	notification.TypeOfMessage == Notification.NotificationType.FriendCreatedEvent ||
					notification.TypeOfMessage == Notification.NotificationType.FriendJoinedEvent ||
					notification.TypeOfMessage == Notification.NotificationType.PicturesAddedToEvent ||
					notification.TypeOfMessage == Notification.NotificationType.YourGroupInvitedToEvent)
				{
					App.coreView.GoToSelectedEvent(notification.ContentEvent.EventId);
				}
				else if (notification.TypeOfMessage == Notification.NotificationType.FacebookFriendHasCreatedProfile ||
						 notification.TypeOfMessage == Notification.NotificationType.FriendRequest)
				{
					InspectController inspect = new InspectController(notification.HeaderProfiles[0], null, null);
					App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
				}
				else if (notification.TypeOfMessage == Notification.NotificationType.FriendJoinedGroup || 
				         notification.TypeOfMessage == Notification.NotificationType.GroupRequest)
				{
					App.coreView.GoToSelectedGroup(notification.ContentGroup.GroupId);
				}
			};



			if (notification.TypeOfMessage == Notification.NotificationType.FacebookFriendHasCreatedProfile)
			{
				Title.Text = "New Friend On HowlOut.";
				Message.Text = notification.HeaderProfiles[0].Name + " has joined HowlOut.";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.FollowedProfileHasCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.HeaderProfiles[0].Name + " who you've been tracking has created a new event, check it out!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.FriendCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.HeaderProfiles[0].Name + " has created a new event, check it out!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.FriendJoinedEvent)
			{
				Title.Text = "Friend Joined Event";
				Message.Text = notification.HeaderProfiles[0].Name + " has joined an event!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.FriendJoinedGroup)
			{
				Title.Text = "Friend Joined Wolf Pack";
				Message.Text = notification.HeaderProfiles[0].Name + " has joined a Wolf Pack, maybe you should join too!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.FriendRequest)
			{
				Title.Text = "Friend Request";
				Message.Text = notification.HeaderProfiles[0].Name + " wants to be your friend!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.GroupRequest)
			{
				Title.Text = "Wolf Pack Invite";
				Message.Text = notification.HeaderProfiles[0].Name + " has invited you to join his Wolf Pack!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.PersonallyInvitedToEvent)
			{
				Title.Text = "Event Invite";
				Message.Text = notification.HeaderProfiles[0].Name + " has invited you to an event.";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.PicturesAddedToEvent)
			{
				Title.Text = "New Pictures";
				Message.Text = "Some pictures has been added to an event you've attended, have a look!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.YourGroupInvitedToEvent)
			{
				Title.Text = "Event Invite";
				Message.Text = "Your WolfPack has been invited to an event.";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.HeaderProfiles[0].Name + " hows event you've previously have attended has created a new event!";
			}

			if (notification.TypeOfMessage == Notification.NotificationType.SomeoneJoinedYourEvent)
			{
				Title.Text = "New Attendee";
				Message.Text = notification.HeaderProfiles[0].Name + " has joined your event " + notification.ContentEvent.Title + ".";
			}


		}
	}
}
