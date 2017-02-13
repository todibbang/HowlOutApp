using System;
using System.Collections.Generic;
namespace HowlOut
{
	public class ToDoItem
	{
		public string OptionDescription { get; set; }
		public int ProfilesNeeded { get; set; }
		public List<Profile> Profiles { get; set; }
		public List<bool> Completed { get; set; }
		public int HeightRequest { get { return 100 + (ProfilesNeeded * 40); } }
		public ToDoListView ToDoListView { get; set; }
	}
}
