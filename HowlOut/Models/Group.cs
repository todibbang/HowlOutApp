using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Group
	{
		public string Name {get; set;}					//The name of the group

		public List <Profile> Members {get; set;}		//People in the event
		public List <Profile> InvitedProfiles {get; set;}		//People which has been invited to join the groups
		public List <Event> EventsInviteTo {get; set;}

		public List <Comment> Comments {get; set;}		//List of comments within the group

		public Group ()
		{
			
		}
	}
}

