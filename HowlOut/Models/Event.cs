using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Event
	{
		public string EventID {get; set;}
		public string Title {get; set;}
		public string Time {get; set;}
		public string Position {get; set;}
		public string Description {get; set;}
		public string OwnerID {get; set;}
		public string AttendingIDs {get; set;}

		public string LowerLimit {get; set;}
		public string UpperLimit {get; set;}

		public List <string> UsersFollowingEventIDs {get; set;}

		public bool Public {get; set;}

		public Event ()
		{

		}
	}
}

