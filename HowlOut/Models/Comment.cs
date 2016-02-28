using System;

namespace HowlOut
{
	public class Comment
	{
		public string Content {get; set;} 
		public string SenderID {get; set;}
		public DateTime DateAndTime { get; set; }

		public Uri ProfileImageUri {
			get {
				return new Uri ("https://graph.facebook.com/v2.5/"+SenderID+"/picture?height=150&width=150");
			}
			set{
				this.ProfileImageUri = value;
			}
		}

		public Comment ()
		{
		}
	}
}

