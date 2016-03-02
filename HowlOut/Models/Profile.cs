using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Profile
	{
		public string ProfileId {get; set;}

		public int LoyaltyRating {get; set;}
		public int Likes {get; set;}
		public string Name {get; set;}
		public int Age {get; set;}

        //public List <string> UserIDsFollowed {get; set;}
        //public List <string> EventIDsFollowed {get; set;}

		public List<Profile> Friends { get; set; }					//Your friends
		public List<Profile> SentFriendRequests { get; set; }		//List of people who you want to be friends with
		public List<Profile> RecievedFriendRequests { get; set; }	//List of people who want to be your friend

		public List<Group> Groups { get; set; }						//Groups you are in
		public List<Group> GroupsInviteTo { get; set; }				//List of groups you have been invited to

        public List<Event> JoinedEvents { get; set; } 				//Events you currently have joined
		public List<Event> EventsInviteTo { get; set; } 			//Events you have been invited to
        public List<Event> FollowedEvents { get; set; }				//Events you are following
		public List<Event> AttendedEvents { get; set; }				//Events you have already attended

		public List <Comment> Comments {get; set;}					//Comments for the users wall
        //public List <string> FriendIDs {get; set;}

		public Profile ()
		{

		}
	}
}

