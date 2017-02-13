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
				if (App.notificationController.chechIfConversationUnseen(ModelType, ConversationID ))
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
				if (App.notificationController.chechIfConversationUnseen(ModelType, ConversationID))
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
					else if(Profiles.Count == 1) { 
						text = Profiles[0].Name; 
					} 
					        
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
				if (LastMessage != null) { return LastMessage.MessageText; } 
				return "";
			} set { this.Content = value; }
		}

		public string Time {
			get {
				if (LastUpdated != null)
				{
					return LastUpdated.ToString("dddd HH:mm - dd MMMMM yyyy");
				}
				return "";
				}
			set { this.Time = value; }
		}

		public string ContentImageSource{
			get
			{
				if (Profiles == null) return "empty";

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
						return ""+eventGroupName;
					}
					return "";
				}
				if (ModelType == ConversationModelType.Group)
				{
					if (!string.IsNullOrWhiteSpace(eventGroupName))
					{
						return "" + eventGroupName;
					}
					return "";
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

		public ConversationSubType SubType { get; set; }

		// ExpenShare: Dictionary<senderId, List<Tuple<ReceiverId, amount, MoneySentDescription, status>>> subTypeDictionary { get; set; }
		// ToDoList: Dictionary<"ToDoList", List<Tuple<ResponsibleId, deadlineInTicks, Description, status>>> subTypeDictionary { get; set; }
		// Doodle1: Dictionary<"Options", List<Tuple<OptionId, deadlineInTicks, Description, status>>> subTypeDictionary { get; set; }
		// Doodle2: Dictionary<profileId, List<Tuple<OptionId, -, Description, status>>> subTypeDictionary { get; set; }
		public Dictionary<string, List<Tuple<string, string, string, StatusOptions>>> subTypeDictionary { get; set; }



		// Tuple<OptionId, double, string, StatusOptions>
		public List<Tuple<string, double, string, StatusOptions>> DoodleList { get; set; }
		public List<ToDoItem> ToDoItems { get; set; }

		//public Dictionary<string, List<Tuple<string, double, string, StatusOptions>>> subTypeDictionary { get; set; }
	}

	public enum StatusOptions
	{
		MoneyRequestSent,
		MoneyReceivedConfirmationSent,
		Confirmed,
		Declined,

		NotStarted,
		InProgress,
		Completed
	}

	public enum ConversationSubType
	{
		None,
		ExpenShare,
		Doodle,
		ToDoList
	}
}