using System;
using System.Collections.Generic;

namespace HowlOut
{
	public class Conversation
	{
		public int ConversationID { get; set; }
		public List <Profile> profiles { get; set; }
		public List <Comment> comments { get; set; }
		public DateTime lastUpdated { get; set; }

		public Conversation()
		{
		}
	}
}
