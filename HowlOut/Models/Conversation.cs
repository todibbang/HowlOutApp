using System;
using System.Collections.Generic;
using System.Linq;

namespace HowlOut
{
	public class Conversation
	{
		public string ConversationID { get; set; }
		public List <Profile> Profiles { get; set; }
		public List <Comment> Comments { get; set; }

		public DateTime LastUpdated {  get {
				if (Comments != null && Comments.Count > 0) { return Comments.OrderByDescending(c => c.DateAndTime).ToList()[0].DateAndTime; }
				return DateTime.Now;
			} set { this.LastUpdated = value; }
		}

		public string Header { get {
				string text = "";
				char[] splitString = {' ', ','};
				if (Profiles != null) {
					if (Profiles.Count > 2) {
						foreach (Profile p in Profiles) {
							if(p.ProfileId != App.userProfile.ProfileId) {
								text += p.Name.Split(splitString)[0] + ", ";
							}
						}
					}
					else { text += Profiles.Find(p => p.ProfileId != App.userProfile.ProfileId).Name; } } 
				return text;
			} set { this.Header = value; }
		}

		public string Content { get {
				if (Comments != null && Comments.Count - 1 >= 0 && Comments[Comments.Count - 1] != null) { return Comments[Comments.Count - 1].Content; } 
				return "";
			} set { this.Content = value; }
		}

		public string Time {
			get { return LastUpdated.ToString("dddd HH:mm - dd MMMMM yyyy"); }
			set { this.Time = value; }
		}

		public Conversation()
		{
		}
	}
}
