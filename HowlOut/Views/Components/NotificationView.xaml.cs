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

			if (notification.ContentProfile != null){
				image.Source = "https://graph.facebook.com/v2.5/" + notification.ContentProfile.ProfileId + "/picture?height=80&width=80";
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
				System.Diagnostics.Debug.WriteLine(notification.Type);

				if (notification.Type == Notification.MessageType.PersonallyInvitedToEvent ||
					notification.Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent ||
					notification.Type == Notification.MessageType.FollowedProfileHasCreatedEvent ||
				   	notification.Type == Notification.MessageType.FriendCreatedEvent ||
					notification.Type == Notification.MessageType.FriendJoinedEvent ||
					notification.Type == Notification.MessageType.PicturesAddedToEvent ||
					notification.Type == Notification.MessageType.YourGroupInvitedToEvent)
				{
					App.coreView.GoToSelectedEvent(notification.ContentEvent.EventId);
				}
				else if (notification.Type == Notification.MessageType.FacebookFriendHasCreatedProfile ||
						 notification.Type == Notification.MessageType.FriendRequest)
				{
					InspectController inspect = new InspectController(notification.ContentProfile, null, null);
					App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
				}
				else if (notification.Type == Notification.MessageType.FriendJoinedGroup || 
				         notification.Type == Notification.MessageType.GroupRequest)
				{
					App.coreView.GoToSelectedGroup(notification.ContentGroup.GroupId);
				}
			};



			if (notification.Type == Notification.MessageType.FacebookFriendHasCreatedProfile)
			{
				Title.Text = "New Friend On HowlOut.";
				Message.Text = notification.ContentProfile.Name + " has joined HowlOut.";
			}

			if (notification.Type == Notification.MessageType.FollowedProfileHasCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.ContentProfile.Name + " who you've been tracking has created a new event, check it out!";
			}

			if (notification.Type == Notification.MessageType.FriendCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.ContentProfile.Name + " has created a new event, check it out!";
			}

			if (notification.Type == Notification.MessageType.FriendJoinedEvent)
			{
				Title.Text = "Friend Joined Event";
				Message.Text = notification.ContentProfile.Name + " has joined an event!";
			}

			if (notification.Type == Notification.MessageType.FriendJoinedGroup)
			{
				Title.Text = "Friend Joined Wolf Pack";
				Message.Text = notification.ContentProfile.Name + " has joined a Wolf Pack, maybe you should join too!";
			}

			if (notification.Type == Notification.MessageType.FriendRequest)
			{
				Title.Text = "Friend Request";
				Message.Text = notification.ContentProfile.Name + " wants to be your friend!";
			}

			if (notification.Type == Notification.MessageType.GroupRequest)
			{
				Title.Text = "Wolf Pack Invite";
				Message.Text = notification.ContentProfile.Name + " has invited you to join his Wolf Pack!";
			}

			if (notification.Type == Notification.MessageType.PersonallyInvitedToEvent)
			{
				Title.Text = "Event Invite";
				Message.Text = notification.ContentProfile.Name + " has invited you to an event.";
			}

			if (notification.Type == Notification.MessageType.PicturesAddedToEvent)
			{
				Title.Text = "New Pictures";
				Message.Text = "Some pictures has been added to an event you've attended, have a look!";
			}

			if (notification.Type == Notification.MessageType.YourGroupInvitedToEvent)
			{
				Title.Text = "Event Invite";
				Message.Text = "Your WolfPack has been invited to an event.";
			}

			if (notification.Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent)
			{
				Title.Text = "New Event";
				Message.Text = notification.ContentProfile.Name + " hows event you've previously have attended has created a new event!";
			}

			if (notification.Type == Notification.MessageType.SomeoneJoinedYourEvent)
			{
				Title.Text = "New Attendee";
				Message.Text = notification.ContentProfile.Name + " has joined your event " + notification.ContentEvent.Title + ".";
			}


		}
	}
}
