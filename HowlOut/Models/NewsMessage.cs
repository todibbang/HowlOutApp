using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class NewsMessage
	{
		public MessageType TypeOfMessage { get; set; }
		public string HeaderMessage { get; set; }
		public List<Profile> HeaderProfiles { get; set; }
		public Event ContentEvent { get; set; }
		public Group ContentGroup { get; set; }

		public NewsMessage ()
		{
			
		}

		public enum MessageType {
			FriendJoinedEvent,
			FriendJoinedGroup,
			FriendCreatedEvent,
			FollowedProfileHasCreatedEvent,
			PicturesAddedToEvent,
			FacebookFriendHasCreatedProfile,
			GroupInvite,
			PersonallyInvitedToEvent,
			YourGroupInvitedToEvent,
			EventHolderWhosEventPreviouslyAttendedHasCreatedEvent,
		}
	}
}

