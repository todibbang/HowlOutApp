using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class YourNotifications : ContentView
	{
		private DataManager _dataManager;
		int listType = 1;

		List<Profile> profilesAdded;

		public YourNotifications()
		{
			InitializeComponent();
			_dataManager = new DataManager();
			App.setOptionsGrid(optionGrid, new List<string> { "Notifications", "Conversations" }, new List<VisualElement> { updateList, updateList }, new List<Action> { ()=> UpdateNotifications(), () => UpdateConversations() }, null);

			updateList.ItemSelected += OnListItemSelected;
			updateList.Refreshing += async (sender, e) => { await UpdateLists(); };

			peopleToAddConversationList.ItemSelected += OnPeopleToAddListItemSelected;
			addedToConversationList.ItemSelected += OnAddedPeopleListItemSelected;

			cancelCreateConversation.Clicked += (sender, e) => { CreateNewConversation.IsVisible = false; };
			createConversation.Clicked += async (sender, e) => {
				profilesAdded.Add(App.userProfile);
				Conversation conv = await _dataManager.MessageApiManager.CreateConversations(profilesAdded);
				if (conv != null) {
					App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
					CreateNewConversation.IsVisible = false;
				}
			};
		}

		async Task UpdateLists()
		{
			if (listType == 0) { await UpdateNotifications(); }
			else { await UpdateConversations(); }
		}

		public async Task UpdateNotifications()
		{
			loading.IsVisible = true;
			//TODO add correct servercall
			List<Notification> notiList = new List<Notification>(); //await _dataManager.ProfileApiManager.GetNotifications(await _dataManager.EventApiManager.GetEventById("50"), await _dataManager.ProfileApiManager.GetProfile("191571161232364"), await _dataManager.GroupApiManager.GetGroupById("8"));
			notiList = notiList.OrderByDescending(c => c.SendTime).ToList();
			ObservableCollection<GroupedNotifications> groupedNotifications = new ObservableCollection<GroupedNotifications>();
			if (notiList.Count > 0)
			{
				GroupedNotifications monthGroup = null;
				int month = notiList[0].SendTime.Month;
				for (int d = 0; d < notiList.Count; d++)
				{
					if (d == 0) { monthGroup = new GroupedNotifications() { Date = (notiList[d].SendTime.ToString("MMMMM")) }; }
					if (month != notiList[d].SendTime.Month)
					{
						month = notiList[d].SendTime.Month;
						groupedNotifications.Add(monthGroup);
						monthGroup = new GroupedNotifications() { Date = (notiList[d].SendTime.ToString("MMMMM")) };
					}
					monthGroup.Add(notiList[d]);
					if (d == notiList.Count - 1) { groupedNotifications.Add(monthGroup); }
				}
			}
			updateList.ItemsSource = groupedNotifications;
			updateList.IsRefreshing = false;
			loading.IsVisible = false;
			listType = 0;
		}

		public async Task UpdateConversations()
		{
			loading.IsVisible = true;
			//TODO add correct servercall
			List<Conversation> conList = await _dataManager.MessageApiManager.GetConversations();
			conList = conList.OrderByDescending(c => c.LastUpdated).ToList();
			ObservableCollection<GroupedConversations> groupedConversations = new ObservableCollection<GroupedConversations>();
			if (conList.Count > 0)
			{
				GroupedConversations monthGroup = null;
				int month = conList[0].LastUpdated.Month;
				for (int d = 0; d < conList.Count; d++)
				{
					if (d == 0) { monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) }; }
					if (month != conList[d].LastUpdated.Month)
					{
						month = conList[d].LastUpdated.Month;
						groupedConversations.Add(monthGroup);
						monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) };
					}
					monthGroup.Add(conList[d]);
					if (d == conList.Count - 1) { groupedConversations.Add(monthGroup); }
				}
			}
			updateList.ItemsSource = groupedConversations;
			updateList.IsRefreshing = false;
			loading.IsVisible = false;
			listType = 1;
		}

		public void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (updateList.SelectedItem == null) { return; }
			if (listType == 0) {
				var selectedNotification = updateList.SelectedItem as Notification;

				if (selectedNotification.Type == Notification.MessageType.PersonallyInvitedToEvent ||
						selectedNotification.Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent ||
						selectedNotification.Type == Notification.MessageType.FollowedProfileHasCreatedEvent ||
						   selectedNotification.Type == Notification.MessageType.FriendCreatedEvent ||
						selectedNotification.Type == Notification.MessageType.FriendJoinedEvent ||
						selectedNotification.Type == Notification.MessageType.PicturesAddedToEvent ||
						selectedNotification.Type == Notification.MessageType.YourGroupInvitedToEvent)
				{
					App.coreView.GoToSelectedEvent(selectedNotification.ContentEvent.EventId);
				}
				else if (selectedNotification.Type == Notification.MessageType.FacebookFriendHasCreatedProfile ||
						 selectedNotification.Type == Notification.MessageType.FriendRequest)
				{
					InspectController inspect = new InspectController(selectedNotification.ContentProfile);
					App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
				}
				else if (selectedNotification.Type == Notification.MessageType.FriendJoinedGroup ||
						 selectedNotification.Type == Notification.MessageType.GroupRequest)
				{
					App.coreView.GoToSelectedGroup(selectedNotification.ContentGroup.GroupId);
				}
			} else if (listType == 1) {
				var selectedConversation = updateList.SelectedItem as Conversation;
				App.coreView.setContentViewWithQueue(new ConversationView(selectedConversation), "Conversation", null);
			}
			updateList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}

		public void ShowNewConversation()
		{
			profilesAdded = new List<Profile>();
			CreateNewConversation.IsVisible = true;
			peopleToAddConversationList.ItemsSource = App.userProfile.Friends;
			addedToConversationList.ItemsSource = null;

		}

		public void OnPeopleToAddListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (peopleToAddConversationList.SelectedItem == null) { return; }
			var selectedProfile = peopleToAddConversationList.SelectedItem as Profile;
			if (!profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Add(selectedProfile);
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			peopleToAddConversationList.SelectedItem = null;
		}
		public void OnAddedPeopleListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (addedToConversationList.SelectedItem == null) { return; }
			var selectedProfile = addedToConversationList.SelectedItem as Profile;
			if (profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Remove(profilesAdded.Find(p => p.ProfileId == selectedProfile.ProfileId));
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			addedToConversationList.SelectedItem = null;
		}


		/*

		public void OnNotificationItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (notificationList.SelectedItem == null) { return; }
			var selectedNotification = notificationList.SelectedItem as Notification;

			if (selectedNotification.Type == Notification.MessageType.PersonallyInvitedToEvent ||
					selectedNotification.Type == Notification.MessageType.EventHolderWhosEventPreviouslyAttendedHasCreatedEvent ||
					selectedNotification.Type == Notification.MessageType.FollowedProfileHasCreatedEvent ||
				   	selectedNotification.Type == Notification.MessageType.FriendCreatedEvent ||
					selectedNotification.Type == Notification.MessageType.FriendJoinedEvent ||
					selectedNotification.Type == Notification.MessageType.PicturesAddedToEvent ||
					selectedNotification.Type == Notification.MessageType.YourGroupInvitedToEvent)
			{
				App.coreView.GoToSelectedEvent(selectedNotification.ContentEvent.EventId);
			}
			else if (selectedNotification.Type == Notification.MessageType.FacebookFriendHasCreatedProfile ||
					 selectedNotification.Type == Notification.MessageType.FriendRequest)
			{
				InspectController inspect = new InspectController(selectedNotification.ContentProfile, null, null);
				App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
			}
			else if (selectedNotification.Type == Notification.MessageType.FriendJoinedGroup ||
					 selectedNotification.Type == Notification.MessageType.GroupRequest)
			{
				App.coreView.GoToSelectedGroup(selectedNotification.ContentGroup.GroupId);
			}
			notificationList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}





		// Updates Conversation list //
		public async Task UpdateConversations()
		{
			loading.IsVisible = true;
			//TODO add correct servercall
			List<Conversation> conList = await _dataManager.ProfileApiManager.GetConversations();
			conList = conList.OrderByDescending(c => c.LastUpdated).ToList();
			ObservableCollection<GroupedConversations> groupedConversations = new ObservableCollection<GroupedConversations>();
			if (conList.Count > 0)
			{
				GroupedConversations monthGroup = null;
				int month = conList[0].LastUpdated.Month;
				for (int d = 0; d < conList.Count; d++) {
					if (d == 0) { monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) }; }
					if (month != conList[d].LastUpdated.Month)
					{
						month = conList[d].LastUpdated.Month;
						groupedConversations.Add(monthGroup);
						monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) };
					}
					monthGroup.Add(conList[d]);
					if (d == conList.Count - 1) { groupedConversations.Add(monthGroup); }
				}
			}
			conversationList.ItemsSource = groupedConversations;
			conversationList.IsRefreshing = false;
			loading.IsVisible = false;
		}

		public void OnConversationItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (conversationList.SelectedItem == null) { return; }
			var selectedConversation = conversationList.SelectedItem as Conversation;
			App.coreView.setContentViewWithQueue(new ConversationView(selectedConversation, false), "Conversation", null);
			conversationList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
*/
		public ScrollView getScrollView() { return null; }
	}
}
