using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Event
	{
		public string EventId {get; set;}

		public string Title {get; set;}

		public string Description {get; set;}
		public string OwnerId {get; set;}

        //public List <string> EventTypes {get; set;}

        public List<EventType> EventTypes { get; set; }

        public double Latitude {get; set;}
		public double Longitude {get; set;}
		public string PositionName {get; set;}

		public List <Profile> Attendees {get; set;}
		public List <Profile> Followers {get; set;}
        
		public DateTime StartDate {get; set;}
		public DateTime EndDate {get; set;}

		public int MinAge {get; set;}
		public int MaxAge {get; set;}

		public int MinSize {get; set;}
		public int MaxSize {get; set;}

		public List <Comment> Comments {get; set;}

		public bool Public {get; set;}

		public Uri ProfileImageUri {
			get {
				return new Uri ("https://graph.facebook.com/v2.5/"+OwnerId+"/picture?height=150&width=150");
			}
			set{
				this.ProfileImageUri = value;
			}
		}

		public Event ()
		{
			EventTypes = new List<EventType> ();
			Attendees = new List<Profile> ();
			Followers = new List<Profile> ();
		}
	}
}

