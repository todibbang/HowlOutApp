using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Event
	{
		public string EventId {get; set;}
		public string Title {get; set;}
		public string Time {get; set;}
		public string Position {get; set;}
		public string Description {get; set;}
		public string OwnerId {get; set;}
		public string AttendingIDs {get; set;}

        public int CurrentUsers { get; set; }
        public int TotalUsers { get; set; }
        public int Followers { get; set; }

        public string LowerLimit {get; set;}
		public string UpperLimit {get; set;}

		public List <string> UsersFollowingEventIDs {get; set;}

		public bool Public {get; set;}

		public Event ()
		{

		}
	}
}

