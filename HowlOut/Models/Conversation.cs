using System;
using System.Collections.Generic;
using System.Linq;

namespace HowlOut
{
	public class Conversation
	{
		public string ConversationID { get; set; }
		public List <Profile> Profiles { get; set; }
		public List <Comment> Messages { get; set; }

		public DateTime LastUpdated {  get {
				if (Messages != null && Messages.Count > 0) { return Messages.OrderByDescending(c => c.DateAndTime).ToList()[0].DateAndTime; }
				return new DateTime();
			} set { this.LastUpdated = value; }
		}

		public string Header { get {
				string text = "";
				char[] splitString = {' ', ','};
				if (Profiles != null) {
					if (Profiles.Count > 2)
					{
						foreach (Profile p in Profiles)
						{
							if (p.ProfileId != App.userProfile.ProfileId)
							{
								text += p.Name.Split(splitString)[0] + ", ";
							}
						}
					}
					else if (Profiles.Count == 2) {
						text = Profiles.Find(p => p.ProfileId != App.userProfile.ProfileId).Name;
					}
					else { text = Profiles[0].Name; } 
				} 
				return text;
			} set { this.Header = value; }
		}

		public string Content { get {
				if (Messages != null && Messages.Count - 1 >= 0 && Messages[Messages.Count - 1] != null) { return Messages[Messages.Count - 1].Content; } 
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
