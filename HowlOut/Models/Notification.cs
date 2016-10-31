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
		public string InAppNotificationId { get; set; }

		public string Header { get {

				if (Type == Notification.MessageType.FriendJoined)
				{
					return "Friend Joined Event";
				}

				if (Type == Notification.MessageType.FriendCreatedEvent)
				{
					return "Friend Created Event";
				}

				if (Type == Notification.MessageType.GroupRequest)
				{
					return "Group Invite";
				}

				if (Type == Notification.MessageType.FriendRequest)
				{
					return "Friend Request";
				}

				if (Type == Notification.MessageType.ProfileInvitedToEvent)
				{
					return "You have been invited to an event";
				}

				if (Type == MessageType.GroupInvitedToEvent)
				{
					return "Your group has been invited to an event";
				}

				if (Type == Notification.MessageType.SomeoneJoinedYourEvent)
				{
					return "Profile joined your event";
				}


				return "";
				} set {
				this.Header = value;
				}
			}
		public string Content {
			get
			{
				if (ContentProfile == null)
				{
					ContentProfile = new Profile() { Name = "Profile" };
				}
				if (Type == Notification.MessageType.FriendJoined)
				{
					return ContentProfile.Name + " has joined an event!";
				}

				if (Type == Notification.MessageType.FriendCreatedEvent)
				{
					return ContentProfile.Name + " has created a new event, check it out!";
				}

				if (Type == Notification.MessageType.GroupRequest)
				{
					return ContentProfile.Name + " has invited you to join his Wolf Pack!";
				}

				if (Type == Notification.MessageType.FriendRequest)
				{
					return ContentProfile.Name + " wants to be your friend!";
				}

				if (Type == Notification.MessageType.ProfileInvitedToEvent)
				{
					return ContentProfile.Name + " has invited you to an event.";
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

		public string ImageSource
		{
			get
			{
				if (ContentProfile == null || string.IsNullOrWhiteSpace(ContentProfile.ImageSource))
				{
					return "default_icon.png";
				}
				return ContentProfile.ImageSource;
			}
			set
			{
				this.ImageSource = value;
			}
		}

		public Notification()
		{
			List<Profile> HeaderProfiles = new List<Profile>();


		}

		public enum MessageType
		{
			FriendJoined, FriendCreatedEvent, GroupRequest, ProfileInvitedToEvent, GroupInvitedToEvent, FriendRequest, SomeoneJoinedYourEvent
		}
	}
}
