using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Notification
	{
		public NotificationType TypeOfMessage { get; set; }
		public List<Profile> ContentProfiles { get; set; }
		public Event ContentEvent { get; set; }
		public Group ContentGroup { get; set; }
		public DateTime SendTime { get; set; }

		public Notification()
		{
			List<Profile> HeaderProfiles = new List<Profile>();
		}

		public enum NotificationType
		{
			FriendJoinedEvent,
			FriendJoinedGroup,
			FriendCreatedEvent,
			FollowedProfileHasCreatedEvent,
			PicturesAddedToEvent,
			FacebookFriendHasCreatedProfile,
			GroupRequest,
			PersonallyInvitedToEvent,
			YourGroupInvitedToEvent,
			EventHolderWhosEventPreviouslyAttendedHasCreatedEvent,
			FriendRequest,
			SomeoneJoinedYourEvent,
			EventYoureAttendedStartingTomorrow,
			EventYoureAttendingStartingInThreeHours,
		}
	}
}
