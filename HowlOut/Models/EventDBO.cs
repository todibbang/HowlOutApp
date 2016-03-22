using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public class EventDBO
	{
		//public string EventId {get; set;}

        public string Title { get; set; }
        public string Description { get; set; }
		public Profile Owner { get; set; }

		public List<EventType> EventTypes { get; set; }

		public double Latitude {get; set;}				//Position of the event
		public double Longitude {get; set;}				//Position of the event
		public string AddressName {get; set;}				//Name of the position / address

		public List <Profile> Attendees {get; set;}			//People attending the event
		public List <Profile> Followers {get; set;}			//People following the event
		public List <Profile> InvitedProfiles {get; set;}	//People invited to the event
		public List <Group> InvitedGroups {get; set;}		//Groups invited to the event

		public DateTime StartDate {get; set;}
		public DateTime EndDate {get; set;}

		public int MinAge {get; set;}
		public int MaxAge {get; set;}

		public int MinSize {get; set;}
		public int MaxSize {get; set;}

		public List <Comment> Comments {get; set;}

        public bool Public { get; set; }

        public EventDBO ()
		{
		}
	}
}

