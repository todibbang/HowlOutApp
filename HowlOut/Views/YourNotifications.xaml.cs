using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class YourNotifications : ContentView
	{
		private DataManager _dataManager;

		public YourNotifications()
		{
			InitializeComponent();
			_dataManager = new DataManager();
			UpdateManageList(notificationList);
		}

		public async void UpdateManageList(StackLayout list)
		{

			while (list.Children.Count != 0)
			{
				list.Children.RemoveAt(0);
			}
			ObservableCollection<Notification> notiList = new ObservableCollection<Notification>();
			notiList = await _dataManager.ProfileApiManager.GetNotifications( await _dataManager.EventApiManager.GetEventById("50"), await _dataManager.ProfileApiManager.GetProfile("191571161232364"), await _dataManager.GroupApiManager.GetGroupById("8"));


			var orderedList = new ObservableCollection<Notification>();
			Notification itemToAdd = new Notification();
			while (notiList.Count != 0)
			{
				DateTime Time = notiList[0].SendTime;
				itemToAdd = notiList[0];

				for (int i = 0; i < notiList.Count; i++)
				{
					if (notiList[i].SendTime > Time)
					{
						itemToAdd = notiList[i];
						Time = itemToAdd.SendTime;
					}
				}
				orderedList.Add(itemToAdd);
				notiList.Remove(itemToAdd);
			}


			var dateTimeMonth = DateTime.Now + new TimeSpan(-32, 0, 0, 0);
			int month = dateTimeMonth.Month;
			for (int i = 0; i < orderedList.Count; i++)
			{
				if (month != orderedList[i].SendTime.Month)
				{
					list.Children.Add(
						new Label()
						{
							Text = ("  " + orderedList[i].SendTime.ToString("MMMM")),
							//BackgroundColor = Color.FromHex ("cccccc"),
							TextColor = App.HowlOut,
							FontSize = 25,
							HeightRequest = 40,
							VerticalTextAlignment = TextAlignment.Center
						});
					month = orderedList[i].SendTime.Month;

				}

				list.Children.Add(new NotificationView(orderedList[i]));

			}
			list.Children.Add(new BoxView() { HeightRequest = 120 });
			loading.IsVisible = false;
		}
	}
}
