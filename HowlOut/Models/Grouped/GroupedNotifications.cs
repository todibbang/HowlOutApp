using System;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public class GroupedNotifications : ObservableCollection<Notification>
	{
		public string Date { get; set; }

		public GroupedNotifications()
		{
		}
	}
}
