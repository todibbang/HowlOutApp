using System;

namespace HowlOut
{
	public class Comment
	{
		public string Content {get; set;}
<<<<<<< Updated upstream
		public string SenderID {get; set;}
		public DateTime DateAndTime { get; set; }
=======
		public string OwnerId {get; set;}
		public string Time {get; set;}
>>>>>>> Stashed changes

		public Uri ProfileImageUri {
			get {
				return new Uri ("https://graph.facebook.com/v2.5/"+OwnerId+"/picture?height=150&width=150");
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

