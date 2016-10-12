using System;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public class GroupedConversations : ObservableCollection<Conversation>
	{
		public string Date { get; set; }

		public GroupedConversations()
		{
		}
	}
}
