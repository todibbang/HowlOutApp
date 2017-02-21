using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HowlOut
{
	public class NotificationController
	{

		public int unseenNotifications = 0;
		public int unseenCommunications = 0;

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
					setConversationsNoti(-1);
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

			App.coreView.updateMainViews(4);
		}

		public bool checkIfUnseen(string modelId, NotificationModelType modelType)
		{
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				if (n.ModelId == modelId && !n.Seen && n.ModelType == modelType)
				{
					return true;
				}
			}
			return false;
		}

		public bool chechIfConversationUnseen(ConversationModelType cType, string cId)
		{
			NotificationModelType modelType = NotificationModelType.ProfileConversation;
			if (cType == ConversationModelType.Event) modelType = NotificationModelType.EventConversation;
			if (cType == ConversationModelType.Group) modelType = NotificationModelType.GroupConversation;


			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				if (modelType == NotificationModelType.ProfileConversation)
				{
					if (n.ModelId == cId && !n.Seen && n.ModelType == modelType)
					{
						return true;
					}
				}
				else {
					if (n.SecondModelId == cId && !n.Seen && n.ModelType == modelType)
					{
						return true;
					}
				}
			}
			return false;
		}

		public async Task setConversationSeen(string modelId, ConversationModelType mType)
		{
			NotificationModelType modelType = NotificationModelType.ProfileConversation;
			if (mType == ConversationModelType.Event) modelType = NotificationModelType.EventConversation;
			if (mType == ConversationModelType.Group) modelType = NotificationModelType.GroupConversation;
			List<Notification> notiToRemove = new List<Notification>(); ;
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				System.Diagnostics.Debug.WriteLine(n.ModelId + ", " + n.ModelType);

				if (((n.SecondModelId == modelId && modelType != NotificationModelType.ProfileConversation) ||
					 (n.ModelId == modelId && modelType == NotificationModelType.ProfileConversation))
					&& !n.Seen && n.ModelType == modelType)
				{
					n.Seen = true;
					notiToRemove.Add(n);
					await App.coreView._dataManager.MessageApiManager.SetNotificationSeen(n.InAppNotificationId);
				}
			}
			foreach (Notification n in notiToRemove) App.coreView.notifications.unseenNotifications.Remove(n);
			//await App.coreView.notifications.UpdateNotifications(false);
			await App.coreView.yourConversatios.UpdateConversations(false);
			//await App.coreView.otherConversatios.UpdateConversations(false);
		}

		public async Task setUpdateSeen(string modelId, NotificationModelType modelType)
		{
			foreach (Notification n in App.coreView.notifications.unseenNotifications)
			{
				System.Diagnostics.Debug.WriteLine(n.ModelId + ", " + n.ModelType);

				if (n.ModelId == modelId && !n.Seen && n.ModelType == modelType)
				{
					n.Seen = true;
					await App.coreView._dataManager.MessageApiManager.SetNotificationSeen(n.InAppNotificationId);
				}
			}
			await App.coreView.notifications.UpdateNotifications(false);
			if (modelType == NotificationModelType.Event) { /*App.coreView.joinedEvents.UpdateList(false, "");*/ }
			else if (modelType == NotificationModelType.ProfileConversation)
			{
				await App.coreView.yourConversatios.UpdateConversations(false);
				//await App.coreView.otherConversatios.UpdateConversations(false);
			}
		}

		public async Task setNotificationSeen(string id)
		{
			App.coreView.notifications.unseenNotifications.Find(n => n.InAppNotificationId == id).Seen = true;
			await App.coreView.notifications.UpdateNotifications(false);
			await App.coreView._dataManager.MessageApiManager.SetNotificationSeen(id);
		}




		public async void updateLocalEventNotifications(string eveID)
		{
			Event e = await App.coreView._dataManager.EventApiManager.GetEventById(eveID);
			updateLocalEventNotifications(e, true);
		}

		public void updateLocalEventNotifications(Event eve, bool update)
		{
			try
			{
				Plugin.LocalNotifications.CrossLocalNotifications.Current.Cancel((int.Parse(eve.EventId) * 2) + 1);
			}
			catch (Exception ecx) { }
			try
			{
				Plugin.LocalNotifications.CrossLocalNotifications.Current.Cancel((int.Parse(eve.EventId) * 2));
			}
			catch (Exception ecx) { }

			if (update)
			{
				if (eve.StartDate > DateTime.Now.AddHours(2).AddMinutes(1))
				{
					Plugin.LocalNotifications.CrossLocalNotifications.Current.Show("Event: " + eve.Title, eve.Title + " is starting in 2 hours!", (int.Parse(eve.EventId) * 2), eve.StartDate.ToLocalTime().AddHours(-2));
				}
				if (eve.StartDate > DateTime.Now.AddDays(1).AddMinutes(1))
				{
					Plugin.LocalNotifications.CrossLocalNotifications.Current.Show("Event: " + eve.Title, eve.Title + " is starting in 1 day!", (int.Parse(eve.EventId) * 2) + 1, eve.StartDate.ToLocalTime().AddDays(-1));
				}
			}
		}


		public void setHowlsNoti(int i)
		{
			if (i > 0)
			{
				unseenNotifications = i;
			}
			else if (i < 0)
			{
				unseenNotifications++;
			}
			else {
				unseenNotifications = 0;
			}

			/*





			var bottomBar = App.coreView.btmBar;
			var notiButton = App.coreView.notiButton;
			if (i > 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = i + "";

				notiButton.IsVisible = true;
				notiButton.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = int.Parse(bottomBar.homeNoti.Text) + 1 + "";

				notiButton.IsVisible = true;
				notiButton.Text = int.Parse(bottomBar.homeNoti.Text) + 1 + "";
			}
			else {
				bottomBar.homeNoti.IsVisible = false;
				bottomBar.homeNoti.Text = 0 + "";

				notiButton.IsVisible = false;
				notiButton.Text = 0 + "";
			} */
		}

		public void setConversationsNoti(int i)
		{
			if (i > 0)
			{
				unseenCommunications = i;
			}
			else if (i < 0)
			{
				unseenCommunications++;
			}
			else {
				unseenCommunications = 0;
			}




			/*



			var bottomBar = App.coreView.btmBar;
			if (i > 0)
			{
				bottomBar.conversationNoti.IsVisible = true;
				bottomBar.conversationNoti.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.conversationNoti.IsVisible = true;
				bottomBar.conversationNoti.Text = int.Parse(bottomBar.conversationNoti.Text) + 1 + "";
			}
			else {
				bottomBar.conversationNoti.IsVisible = false;
				bottomBar.conversationNoti.Text = 0 + "";
			} */
		}
	}
}
