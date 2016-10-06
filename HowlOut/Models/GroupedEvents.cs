using System;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public class GroupedEvents : ObservableCollection<EventForLists>
	{
		public string Date { get; set; }

		public GroupedEvents()
		{
		}
	}
}
