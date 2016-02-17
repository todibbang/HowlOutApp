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

		public List <string> UserIDsFollowed {get; set;}
		public List <string> EventIDsFollowed {get; set;}

		public List <string> FriendIDs {get; set;}

		public Profile ()
		{

		}
	}
}

