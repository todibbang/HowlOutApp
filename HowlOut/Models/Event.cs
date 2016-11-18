using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public class Event
	{
		public string EventId {get; set;}
		public string Title {get; set;}
		public string Description {get; set;}
		public string ImageSource { get; set; }

		public Visibility Visibility { get; set; }
		public List<EventType> EventTypes { get; set; }

		public Profile ProfileOwner {get; set;}
		public Organization OrganizationOwner { get; set;}
		public Group GroupSpecific { get; set; }

		public double Latitude { get; set; }                //Position of the event
		public double Longitude { get; set; }               //Position of the event
		public string AddressName { get; set; }             //Name of the position / address

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public int MinAge { get; set; }
		public int MaxAge { get; set; }

		public int MinSize { get; set; }
		public int MaxSize { get; set; }

		public int NumberOfAttendees {get; set;}
		public List <Profile> Attendees {get; set;}			//People attending the event
		public List <Profile> Followers {get; set;}			//People following the event
		public List <Profile> InvitedProfiles {get; set;}	//People invited to the event
		public List <Group> InvitedGroups {get; set;}		//Groups invited to the event

		public List <Comment> Comments {get; set;}
		public List <Conversation> Conversations { get; set; }
		public SearchSettings ProfileSearchSettings { get; set; }

		public Event()
		{
			EventTypes = new List<EventType>();
		}

	}
}

