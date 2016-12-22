using System;
using System.Threading.Tasks;

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

		public void HandlePushNotification(string modelType, string modelId, bool fromBackground, string alert)
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
				App.coreView.ShowNotification(() => {App.coreView.GoToSelectedEvent(modelId); }, alert);
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
				App.coreView.ShowNotification(() => { App.coreView.GoToSelectedConversation(modelId); }, alert);
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
				App.coreView.ShowNotification(() => { App.coreView.GoToSelectedGroup(modelId); }, alert);
				if (fromBackground)
				{
					App.coreView.GoToSelectedGroup(modelId);
				}
			}



			//GO TO PROFILE
			if (modelType == "RequestedToFriend" ||
			   modelType == "AcceptedToFriend" )
			{
				App.coreView.ShowNotification(() => { App.coreView.GoToSelectedProfile(modelId); }, alert);
				if (fromBackground)
				{
					App.coreView.GoToSelectedProfile(modelId);
				}
			}

			App.coreView.updateHomeView();
		}
	}
}
