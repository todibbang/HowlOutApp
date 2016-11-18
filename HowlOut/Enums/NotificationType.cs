using System;
namespace HowlOut
{
	public enum NotificationType
	{
		WrittenToProfileConversation,
		WrittenToEventConversation,
		WrittenToGroupConversation,
		WrittenToOrganizationConversation,
		WrittenToEventComments,
		WrittenToGroupComments,
		WrittenToOrganizationComments,
		JoinedYourEvent,
		InvitedToEvent,
		EventCancelled,
		EventEdited,
		RequestedToFriend,
		AcceptedToFriend,
		InvitedToGroup,
		RequestedToJoinGroup,
		InvitedToOrganization,
		FriendJoinedEvent,
		FriendCreatedEvent,
		GroupInvitedToEvent
	}
}
