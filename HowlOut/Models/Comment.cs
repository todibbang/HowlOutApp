using System;
using Xamarin.Forms;

namespace HowlOut
{
	public class Comment
	{
		public string Content {get; set;} 
		public string SenderId {get; set;}
		public DateTime DateAndTime { get; set; }

		public string MessageText { 
			get
			{
				
				return Content;
			}
			set { } 
		}

		public string ImageText
		{
			get
			{
				if (Content.Contains(".IMAGESTART.") && Content.Contains(".IMAGEEND."))
				{
					int startIndex = Content.IndexOf(".IMAGESTART.") + ".IMAGESTART.".Length;
					int endIndex = Content.IndexOf(".IMAGEEND.", startIndex);
					return Content.Substring(startIndex, endIndex - startIndex);
				}

				return "";
			}
			set { }
		}

		public string ImageSource
		{
			get;
		set; 
		}

		public bool displayImage { get {
				if (SenderId == App.userProfile.ProfileId) { return false; }
				return true;
			}  
		}

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

		public int column
		{
			get
			{
				if (SenderId == App.userProfile.ProfileId) { return 3;}
				return 1;
			}
		}

		public LayoutOptions horizontal
		{
			get
			{
				if (SenderId == App.userProfile.ProfileId) { return LayoutOptions.EndAndExpand; }
				return LayoutOptions.StartAndExpand;
			}
		}

		public Color bgColor
		{
			get
			{
				if (SenderId == App.userProfile.ProfileId) { return Color.Gray; }
				return App.HowlOut;
			}
		}

		public bool displayMessageImage
		{
			get
			{
				return true;
			}
		}

		public Comment ()
		{
			
		}
	}
}

