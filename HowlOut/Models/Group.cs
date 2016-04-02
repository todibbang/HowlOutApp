using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public class Group
	{
		public string GroupId {get; set;}
		public string Name {get; set;}						//The name of the group
		public Profile Owner {get; set;}
		public bool Public {get; set;}
		public int NumberOfMembers {get; set;}
		//public List <Profile> Administrator {get; set;}
		public List <Profile> Members {get; set;}			//People in the event
		public List <Profile> InvitedProfiles {get; set;}	//People which has been invited to join the groups
		public List <Comment> Comments {get; set;}			//List of comments within the group

		public Group ()
		{
			Members = new List<Profile> ();
			InvitedProfiles = new List<Profile> ();
			Comments = new List<Comment> ();
		}
	}
}

