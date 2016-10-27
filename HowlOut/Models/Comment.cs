using System;

namespace HowlOut
{
	public class Comment
	{
		public string Content {get; set;} 
		public string SenderId {get; set;}
		public DateTime DateAndTime { get; set; }

		public string ImageSource { get; set;  }

		public string Time
		{
			get
			{
				return DateAndTime.ToString("dddd HH:mm - dd MMMMM yyyy");
			}
			set
			{
				this.Time = value;
			}
		}

		public Comment ()
		{
			
		}
	}
}

