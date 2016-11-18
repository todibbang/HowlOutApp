using System;
namespace HowlOut
{
	public class NotificationController
	{
		public NotificationController()
		{
		}

		public void NotificationOpenedFromBackground(string modelType, string modelId)
		{

		}

		public void HandlePushNotification(string modelType, string modelId, bool fromBackground)
		{
			System.Diagnostics.Debug.WriteLine(modelType + ", " + modelId);

			App.coreView.notifications.UpdateNotifications(true);


			if (modelType.ToLower().Contains("conversation") || modelType.ToLower().Contains("comment"))
			{
				App.UpdateLiveConversations();
			}

			//GO TO EVENT
			if (modelType == "WrittenToEventComments" ||
			   modelType == "JoinedYourEvent" ||
			   modelType == "InvitedToEvent" ||
			   modelType == "EventCancelled" ||
			   modelType == "FriendJoinedEvent" ||
			   modelType == "FriendCreatedEvent" ||
			   modelType == "GroupInvitedToEvent")
			{
				if (fromBackground)
				{
					App.coreView.GoToSelectedEvent(modelId);
				}
			}

			//GO TO CONVERSATION
			if (modelType == "WrittenToProfileConversation" ||
			   modelType == "WrittenToGroupConversation" ||
			   modelType == "WrittenToOrganizationConversation" ||
			   modelType == "WrittenToEventConversation"// ||
			   )
			{
				if (fromBackground)
				{
					App.coreView.GoToSelectedConversation(modelId);
				}
				else {
					App.coreView.setConversationsNoti(-1);
				}
			}

			//GO TO GROUP
			if (modelType == "WrittenToGroupComments" ||
			   modelType == "InvitedToGroup" ||
			   modelType == "RequestedToJoinGroup")
			{
				if (fromBackground)
				{
					App.coreView.GoToSelectedGroup(modelId);
				}
			}

			//GO TO ORGANIZATION
			if (modelType == "WrittenToOrganizationComments" ||
			   modelType == "InvitedToOrganization")
			{
				if (fromBackground)
				{
					App.coreView.GoToSelectedOrganization(modelId);
				}
			}

			//GO TO PROFILE
			if (modelType == "RequestedToFriend" ||
			   modelType == "AcceptedToFriend" )
			{
				if (fromBackground)
				{
					App.coreView.GoToSelectedProfile(modelId);
				}
			}

			App.coreView.updateHomeView();
		}
	}
}
