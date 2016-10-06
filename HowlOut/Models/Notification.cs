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

		public Notification()
		{
			List<Profile> HeaderProfiles = new List<Profile>();
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
