using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class YourNotifications : ContentView, ViewModelInterface
	{
		private DataManager _dataManager;
		List<Notification> notiList = new List<Notification>();
		public List<Notification> unseenNotifications = new List<Notification>();

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public YourNotifications()
		{
			InitializeComponent();
			_dataManager = new DataManager();
			UpdateNotifications(true);

			updateList.ItemSelected += OnListItemSelected;
			updateList.Refreshing += async (sender, e) => { await UpdateLists(); };
		}

		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationLabel("Notifications", null);

			/*
			bar.setRightButton("ic_share.png").Clicked += async (sender, e) =>
			{
				await App.coreView._dataManager.AttendTrackEvent(eve, false, true);
			};

			bar.showNewConversationButton(true, App.coreView.profileConversatios); */
		}

		public void viewExitFocus() { }

		public ContentView getContentView() { return this; }

		async Task UpdateLists()
		{
			await UpdateNotifications(true); 
		}

		public async Task UpdateNotifications(bool update)
		{
			nothingToLoad.IsVisible = false;
			//TODO add correct servercall
			if (update) { notiList = await _dataManager.MessageApiManager.GetNotifications(); }
			if (notiList == null || notiList.Count == 0)
			{
				nothingToLoad.IsVisible = true;
				updateList.IsRefreshing = false;
				return;
			}
			unseenNotifications.Clear();

			notiList = notiList.OrderByDescending(c => c.SendTime).ToList();

			int n = 0;
			List<Notification> commentNoti = new List<Notification>();
			foreach (Notification c in notiList)
			{
				if (c.ModelType == NotificationModelType.ProfileConversation || c.ModelType == NotificationModelType.EventConversation || 
				                c.ModelType == NotificationModelType.GroupConversation)
				{
					commentNoti.Add(c);
				}
				else if (!c.Seen)
				{
					n++;
				}
				if (!c.Seen)
				{
					unseenNotifications.Add(c);
				}
			}

			foreach (Notification c in commentNoti)
			{
				notiList.Remove(c);
			}
			//notiList.RemoveAll(noti => noti.ModelType == NotificationModelType.ProfileConversation);
			App.coreView.setHowlsNoti(n);

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
		}

		public async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (updateList.SelectedItem == null) { return; }
			var selectedNotification = updateList.SelectedItem as Notification;

			_dataManager.setNotificationSeen(selectedNotification.InAppNotificationId);

			if (selectedNotification.ModelType == NotificationModelType.Event ||
			    selectedNotification.ModelType == NotificationModelType.EventConversation)
			{
				await App.coreView.GoToSelectedEvent(selectedNotification.ModelId);
			} 
			else if (selectedNotification.ModelType == NotificationModelType.Group ||
			         selectedNotification.ModelType == NotificationModelType.GroupConversation)
			{
				await App.coreView.GoToSelectedGroup(selectedNotification.ModelId);
			}
			else if (selectedNotification.ModelType == NotificationModelType.Profile )
			{
				await App.coreView.GoToSelectedProfile(selectedNotification.ModelId);
			}
			else if (selectedNotification.ModelType == NotificationModelType.ProfileConversation)
			{
				await App.coreView.GoToSelectedConversation(selectedNotification.ModelId);
			}

			if (selectedNotification.ModelType == NotificationModelType.EventConversation ||
			    selectedNotification.ModelType == NotificationModelType.GroupConversation)
			{
				await App.coreView.GoToSelectedConversation(selectedNotification.SecondModelId);
			}

			updateList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
		public ScrollView getScrollView() { return null; }
	}
}
