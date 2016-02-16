using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Profile
	{
		public string FaceBookID {get; set;}

		public int LoyaltyRating {get; set;}
		public int Likes {get; set;}
		public string Name {get; set;}
		public string Age {get; set;}

		public List <string> WhoUserFollows {get; set;}
		public List <string> WhoFollowsUser {get; set;}

		public List <string> Friends {get; set;}

		public List <string> EventIDsAttending {get; set;}
		public List <string> EventIDsFollowing {get; set;}

		public Profile ()
		{

		}
	}
}

