using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace HowlOut
{
	public class Conversation
	{
		public string ConversationID { get; set; }
		public List <Profile> Profiles { get; set; }
		public List <Comment> Messages { get; set; }
		public string Title { get; set; }
		public string eventGroupName { get; set; }
		//public Group Group { get; set; }
		//public Event Event { get; set; }
		//public Organization Organization { get; set; }
		public Comment LastMessage { get; set; }

		public ConversationModelType ModelType { get; set; }
		public string ModelId { get; set; }

		public FontAttributes fontAttributes
		{
			get
			{
				if (App.coreView._dataManager.chechIfConversationUnseen(ModelType, ConversationID ))
				{
					return FontAttributes.Bold;
				} 
				return FontAttributes.None;
			}
			set { }
		}
		public Color textColor
		{
			get
			{
				if (App.coreView._dataManager.chechIfConversationUnseen(ModelType, ConversationID))
				{
					return App.HowlOut;
				}
				return App.NormalTextColor;
			}
			set { }
		}



		public string Header { get {
				if (!string.IsNullOrWhiteSpace(Title))
				{
					return Title;
				}

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

						text = text.Substring(0, text.Length - 2);
					}
					else if (Profiles.Count == 2) {
						text = Profiles.Find(p => p.ProfileId != App.userProfile.ProfileId).Name;
					}
					else { text = Profiles[0].Name; } 
				} 
				return text;
			} set { this.Header = value; }
		}

		public DateTime LastUpdated { get {
				if (LastMessage != null) { return LastMessage.DateAndTime; }
				return new DateTime();
			} set { this.LastUpdated = value; }
		}

		public string Content { get {
				if (LastMessage != null) { return LastMessage.Content; } 
				return "";
			} set { this.Content = value; }
		}

		public string Time {
			get { return LastUpdated.ToString("dddd HH:mm - dd MMMMM yyyy"); }
			set { this.Time = value; }
		}

		public string ContentImageSource{
			get
			{
				if (Messages != null && Messages.Count > 0) { 
					string id = Messages.OrderByDescending(c => c.DateAndTime).ToList()[0].SenderId;
					//string id = Messages.OrderByDescending(c => c.DateAndTime).ToList().Find(c => c.SenderId != App.StoredUserFacebookId).SenderId;
					return Profiles.Find(p => p.ProfileId == id).ImageSource; 
				}
				else if (Profiles != null && Profiles.Count > 1) { return Profiles.Find(p => p.ProfileId != App.StoredUserFacebookId).ImageSource;}
				return "default_icon.png";
			}
			set { this.ContentImageSource = value; }
		}

		public string SpecificInfo
		{
			get
			{
				if (ModelType == ConversationModelType.Event)
				{
					if (!string.IsNullOrWhiteSpace(eventGroupName))
					{
						return "Event: "+eventGroupName;
					}
					return "Event: ";
				}
				if (ModelType == ConversationModelType.Group)
				{
					if (!string.IsNullOrWhiteSpace(eventGroupName))
					{
						return "Group: " + eventGroupName;
					}
					return "Group: ";
				}
				return "";
			}
			set { this.SpecificInfo = value; }
		}

		public bool ShowSpecificInfo
		{
			get
			{
				if (ModelType != ConversationModelType.Profile) return true;
				return false;
			}
			set { this.ShowSpecificInfo = value; }
		}


		public Conversation()
		{
		}
	}
}
