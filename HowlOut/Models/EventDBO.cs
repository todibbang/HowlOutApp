using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class EventDBO
	{
		public string Title {get; set;}

		public string Description {get; set;}
		public string OwnerId {get; set;}

		public List <string> EventTypes {get; set;}

		public string PositionCoordinates {get; set;}
		public string PositionName {get; set;}

		public List <string> AttendingIDs {get; set;}
		public List <string> FollowerIDs {get; set;}

		public DateTime StartDate {get; set;}
		public DateTime EndDate {get; set;}

		public int MinAge {get; set;}
		public int MaxAge {get; set;}

		public int MinSize {get; set;}
		public int MaxSize {get; set;}

		public List <Comment> Comments {get; set;}

		public bool Public {get; set;}

		public EventDBO ()
		{
		}
	}
}

