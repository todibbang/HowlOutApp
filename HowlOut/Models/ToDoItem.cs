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
		public ToDoListView ToDoListView { get; set; }
		public string AssignedAndNeeded { get { if (Profiles == null) return "0/" + ProfilesNeeded; return Profiles.Count + "/" + ProfilesNeeded; } }
		public string NumberCompleted { get { if (Completed == null) return "(0)"; return "(" + Completed.FindAll(b => b == true).Count + ")"; } }

		public ToDoItem()
		{
			Completed = new List<bool>();
		}
	}
}
