﻿using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Profile
	{
		public string ProfileId {get; set;}
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageSource { get; set; }
		public string SmallImageSource { get; set; }
		public string LargeImageSource { get; set; }

		public int LoyaltyRating {get; set;}
		public int Likes {get; set;}
		public int Age { get; set; }

		public List <Profile> Friends { get; set; }	
		public List <Profile> ProfilesFollowingYou { get; set; }		
		public List <Profile> SentFriendRequests { get; set; }	
		public List <Profile> RecievedFriendRequests { get; set; }

		public List <Group> Groups { get; set; }
		public List <Group> GroupsOwned { get; set; }
		public List <Group> GroupsInviteTo { get; set; }
		public List <Group> GroupsInviteToAsOwner { get; set; }

		public List <Event> JoinedEvents { get; set; } 				
		public List <Event> EventsInviteTo { get; set; }
		public List <Event> EventsInviteToAsOwner { get; set; }
		public List <string> EventsFollowed { get; set; }				
		public List <Event> AttendedEvents { get; set; }				

		public List <Comment> Comments {get; set;}					
        public List <Notification> InAppNotifications { get; set; }
		public SearchSettings SearchPreference { get; set; }
		public List<Banner> Banners { get; set; }


		public Profile ()
		{

		}
	}
}

