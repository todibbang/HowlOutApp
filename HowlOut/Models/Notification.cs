using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public class Notification
	{
		public string InAppNotificationId { get; set; }
		public NotificationModelType ModelType { get; set; }
		public string ModelId { get; set; }
		public NotificationType NotificationType { get; set; }
		public string ContentName { get; set; }
		public string ContentImageSource { get; set; }
		public string SenderName { get; set; }
		public bool Seen { get; set; }
		public DateTime SendTime { get; set; }
		public virtual Profile Profile { get; set; }
		public string SecondModelId { get; set; }

		public FontAttributes fontAttributes 
		{ 
			get 
			{
				if (Seen)
				{
					return FontAttributes.None;
				}
				return FontAttributes.Bold;
				} set { } 
		}
		public Color textColor
		{
			get
			{
				if (Seen)
				{
					return App.NormalTextColor;
				}
				return App.HowlOut;
			}
			set { }
		}

		public string Header { get {

				//Events
				if (NotificationType == NotificationType.InvitedToEvent)
				{
					return "You have been invited to an event";
				}
				if (NotificationType == NotificationType.JoinedYourEvent)
				{
					return "Profile joined your event";
				}
				if (NotificationType == NotificationType.EventCancelled)
				{
					return "Event Cancelled";
				}
				if (NotificationType == NotificationType.FriendJoinedEvent)
				{
					return "Friend Joined Event";
				}
				if (NotificationType == NotificationType.FriendCreatedEvent)
				{
					return "Friend Created Event";
				}
				if (NotificationType == NotificationType.GroupInvitedToEvent)
				{
					return "Your group has been invited to an event";
				}

				// Friends
				if (NotificationType == NotificationType.RequestedToFriend)
				{
					return "Friend Request";
				}
				if (NotificationType == NotificationType.AcceptedToFriend)
				{
					return "Friend Request Accepted";
				}

				//Groups
				if (NotificationType == NotificationType.InvitedToGroup)
				{
					return "Group Invite";
				}
				if (NotificationType == NotificationType.RequestedToJoinGroup)
				{
					return "Profile Wants To Join Your Group";
				}

				//Organizations
				if (NotificationType == NotificationType.InvitedToOrganization)
				{
					return "Organization Invite";
				}

				//Communication
				if (NotificationType == NotificationType.WrittenToEventComments)
				{
					return "New Event Comment";
				}
				if (NotificationType == NotificationType.WrittenToGroupComments)
				{
					return "New Group Comment";
				}
				if (NotificationType == NotificationType.WrittenToOrganizationComments)
				{
					return "New Organization Comment";
				}
				if (NotificationType == NotificationType.WrittenToEventConversation)
				{
					return "New Event Message";
				}
				if (NotificationType == NotificationType.WrittenToGroupConversation)
				{
					return "New Group Message";
				}
				if (NotificationType == NotificationType.WrittenToOrganizationConversation)
				{
					return "New Organization Message";
				}


				return NotificationType.ToString();
				} set {
				this.Header = value;
				}
			}
		public string Content {
			get
			{
				/*
				//Events
				if (NotificationType == NotificationType.InvitedToEvent)
				{
					return SenderName + " has invited you to the event " + ContentName;
				}
				if (NotificationType == NotificationType.JoinedYourEvent)
				{
					return SenderName + " joined your event " + ContentName;
				}
				if (NotificationType == NotificationType.EventCancelled)
				{
					return SenderName + " cancelled the event '" + ContentName + "' you were attending";
				}
				if (NotificationType == NotificationType.FriendJoinedEvent)
				{
					return SenderName + " joined your event " + ContentName;
				}
				if (NotificationType == NotificationType.FriendCreatedEvent)
				{
					return SenderName + " created an event: " + ContentName;
				}
				if (NotificationType == NotificationType.GroupInvitedToEvent)
				{
					return "Your group has been invited to the event " + ContentName;
				}

				// Friends
				if (NotificationType == NotificationType.RequestedToFriend)
				{
					return SenderName + " wants to be your friend";
				}
				if (NotificationType == NotificationType.AcceptedToFriend)
				{
					return SenderName + " accepted your friend request";
				}

				//Groups
				if (NotificationType == NotificationType.InvitedToGroup)
				{
					return SenderName + " invited you to join the group " + ContentName;
				}
				if (NotificationType == NotificationType.RequestedToJoinGroup)
				{
					return SenderName + " wants to join your group " + ContentName;
				}

				//Organizations
				if (NotificationType == NotificationType.InvitedToOrganization)
				{
					return SenderName + " invited you to join the organization " + ContentName;
				}

				//Communication
				if (NotificationType == NotificationType.WrittenToEventComments)
				{
					return SenderName + " has written a comment in the event " + ContentName;
				}
				if (NotificationType == NotificationType.WrittenToGroupComments)
				{
					return SenderName + " has written a comment in the group " + ContentName;
				}
				if (NotificationType == NotificationType.WrittenToOrganizationComments)
				{
					return SenderName + " has written a comment in the organization " + ContentName;
				}
				if (NotificationType == NotificationType.WrittenToEventConversation)
				{
					return SenderName + " has sent a message in a conversation for the event " + ContentName;
				}
				if (NotificationType == NotificationType.WrittenToGroupConversation)
				{
					return SenderName + " has sent a message in a conversation for the group " + ContentName;
				}
				if (NotificationType == NotificationType.WrittenToOrganizationConversation)
				{
					return SenderName + " has sent a message in a conversation for the organization " + ContentName;
				}
				*/
				return ContentName;
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
		/*
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
		*/

		public Notification()
		{
		}
	}
}
