using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public class Group
	{
		public string GroupId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageSource { get; set; }
		public string SmallImageSource { get; set; }
		public string LargeImageSource { get; set; }
		public int NumberOfActiveEvents { get; set; }

		public GroupVisibility Visibility { get; set; }

		public List<Profile> ProfileOwners { get; set; }

		public int NumberOfMembers { get; set; }
		public List <Profile> Members { get; set; }	
		public List <Profile> InvitedProfiles { get; set; }
		public List <Profile> InvitedProfilesAsOwner { get; set; }
		public List <Profile> ProfilesRequestingToJoin { get; set; }

		public List <Comment> Comments { get; set; }
		public List<Conversation> Conversations { get; set; }

		public Group ()
		{
			Members = new List <Profile> ();
			InvitedProfiles = new List <Profile> ();
			Comments = new List <Comment> ();
			ProfilesRequestingToJoin = new List <Profile>();
		}
	}
}

