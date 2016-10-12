using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Notification
	{
		public MessageType Type { get; set; }
		public Profile ContentProfile { get; set; }
		public Event ContentEvent { get; set; }
		public Group ContentGroup { get; set; }
		public DateTime SendTime { get; set; }
		public bool Read { get; set; }

		public string Header { get {
				if (Type == Notification.MessageType.FacebookFriendHasCreatedProfile)
				{
					return "New Friend On HowlOut.";
				}

				if (Type == Notification.MessageType.FollowedProfileHasCreatedEvent)
				{
					return "New Event";
				}

				if (Type == Notification.MessageType.FriendCreatedEvent)
				{
					return "New Event";
				}

				if (Type == Notification.MessageType.FriendJoinedEvent)
				{
					return "Friend Joined Event";
				}

				if (Type == Notification.MessageType.FriendJoinedGroup)
				{
					return "Friend Joined Wolf Pack";
				}

				if (Type == Notification.MessageType.FriendRequest)
				{
					return "Friend Request";
				}

				if (Type == Notification.MessageType.GroupRequest)
				{
					return "Wolf Pack Invite";
				}

				if (Type == Notification.MessageType.PersonallyInvitedToEvent)
				{
					return "Event Invite";
				}

				if (Type == Notification.MessageType.PicturesAddedToEvent)
				{
					return "New Pictures";
				}

				if (Type == Notification.MessageType.YourGroupInvitedToEvent)
				{
					return "Event Invite";
				}

				if (Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent)
				{
					return "New Event";
				}

				if (Type == Notification.MessageType.SomeoneJoinedYourEvent)
				{
					return "New Attendee";
				}

				return "";
				} set {
				this.Header = value;
				}
			}
		public string Content {
			get
			{
				if (Type == Notification.MessageType.FacebookFriendHasCreatedProfile)
				{
					return ContentProfile.Name + " has joined HowlOut.";
				}

				if (Type == Notification.MessageType.FollowedProfileHasCreatedEvent)
				{
					return ContentProfile.Name + " who you've been tracking has created a new event, check it out!";
				}

				if (Type == Notification.MessageType.FriendCreatedEvent)
				{
					return ContentProfile.Name + " has created a new event, check it out!";
				}

				if (Type == Notification.MessageType.FriendJoinedEvent)
				{
					return ContentProfile.Name + " has joined an event!";
				}

				if (Type == Notification.MessageType.FriendJoinedGroup)
				{
					return ContentProfile.Name + " has joined a Wolf Pack, maybe you should join too!";
				}

				if (Type == Notification.MessageType.FriendRequest)
				{
					return ContentProfile.Name + " wants to be your friend!";
				}

				if (Type == Notification.MessageType.GroupRequest)
				{
					return ContentProfile.Name + " has invited you to join his Wolf Pack!";
				}

				if (Type == Notification.MessageType.PersonallyInvitedToEvent)
				{
					return ContentProfile.Name + " has invited you to an event.";
				}

				if (Type == Notification.MessageType.PicturesAddedToEvent)
				{
					return "Some pictures has been added to an event you've attended, have a look!";
				}

				if (Type == Notification.MessageType.YourGroupInvitedToEvent)
				{
					return "Your WolfPack has been invited to an event.";
				}

				if (Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent)
				{
					return ContentProfile.Name + " hows event you've previously have attended has created a new event!";
				}

				if (Type == Notification.MessageType.SomeoneJoinedYourEvent)
				{
					return ContentProfile.Name + " has joined your event " + ContentEvent.Title + ".";
				}

				return "";
			}
			set
			{
				this.Content = value;
			}
		}
		public string Time
		{
			get
			{
				return SendTime.ToString("dddd HH:mm - dd MMMMM yyyy");
			}
			set
			{
				this.Time = value;
			}
		}
		public Notification()
		{
			List<Profile> HeaderProfiles = new List<Profile>();

			/*
			Time = SendTime.ToString("dddd HH:mm - dd MMMMM yyyy");


			if (Type == Notification.MessageType.FacebookFriendHasCreatedProfile)
			{
				Title = "New Friend On HowlOut.";
				Message = ContentProfile.Name + " has joined HowlOut.";
			}

			if (Type == Notification.MessageType.FollowedProfileHasCreatedEvent)
			{
				Title = "New Event";
				Message = ContentProfile.Name + " who you've been tracking has created a new event, check it out!";
			}

			if (Type == Notification.MessageType.FriendCreatedEvent)
			{
				Title = "New Event";
				Message = ContentProfile.Name + " has created a new event, check it out!";
			}

			if (Type == Notification.MessageType.FriendJoinedEvent)
			{
				Title = "Friend Joined Event";
				Message = ContentProfile.Name + " has joined an event!";
			}

			if (Type == Notification.MessageType.FriendJoinedGroup)
			{
				Title = "Friend Joined Wolf Pack";
				Message = ContentProfile.Name + " has joined a Wolf Pack, maybe you should join too!";
			}

			if (Type == Notification.MessageType.FriendRequest)
			{
				Title = "Friend Request";
				Message = ContentProfile.Name + " wants to be your friend!";
			}

			if (Type == Notification.MessageType.GroupRequest)
			{
				Title = "Wolf Pack Invite";
				Message = ContentProfile.Name + " has invited you to join his Wolf Pack!";
			}

			if (Type == Notification.MessageType.PersonallyInvitedToEvent)
			{
				Title = "Event Invite";
				Message = ContentProfile.Name + " has invited you to an event.";
			}

			if (Type == Notification.MessageType.PicturesAddedToEvent)
			{
				Title = "New Pictures";
				Message = "Some pictures has been added to an event you've attended, have a look!";
			}

			if (Type == Notification.MessageType.YourGroupInvitedToEvent)
			{
				Title = "Event Invite";
				Message = "Your WolfPack has been invited to an event.";
			}

			if (Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent)
			{
				Title = "New Event";
				Message = ContentProfile.Name + " hows event you've previously have attended has created a new event!";
			}

			if (Type == Notification.MessageType.SomeoneJoinedYourEvent)
			{
				Title = "New Attendee";
				Message = ContentProfile.Name + " has joined your event " + ContentEvent.Title + ".";
			}


*/



		}

		public enum MessageType
		{
			FriendJoined, 
			FriendCreatedEvent, 
			GroupRequest, 
			ProfileInvitedToEvent, 
			GroupInvitedToEvent, 
			FriendRequest, 
			SomeoneJoinedYourEvent,




			FriendJoinedEvent,
			FriendJoinedGroup,
			FollowedProfileHasCreatedEvent,
			PicturesAddedToEvent,
			FacebookFriendHasCreatedProfile,
			PersonallyInvitedToEvent,
			YourGroupInvitedToEvent,
			EventHolderWhosEventPreviouslyAttendedHasCreatedEvent,
			EventYoureAttendedStartingTomorrow,
			EventYoureAttendingStartingInThreeHours,
		}
	}
}
