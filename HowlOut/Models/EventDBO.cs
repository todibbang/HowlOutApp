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
		public int AttendingAmount { get; set; }

		public List <string> FollowerIDs { get; set; }
		public int FollowersAmount { get; set; }

		public string StartTime {get; set;}
		public string EndTime {get; set;}

		public string StartDate {get; set;}
		public string EndDate {get; set;}

		public string MinAge {get; set;}
		public string MaxAge {get; set;}

		public string MinSize {get; set;}
		public string MaxSize {get; set;}

		public bool Public {get; set;}

		public string Time {get; set;}
		public string Position {get; set;}
		public int CurrentUsers {get; set;}
		public int TotalUsers {get; set;}
		public int Followers {get; set;}

		public EventDBO ()
		{
		}
	}
}

