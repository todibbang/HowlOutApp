using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace HowlOut
{
	public class Comment
	{
		public string Content {get ; set;} 
		public string SenderId {get; set;}
		public DateTime DateAndTime { get; set; }

		public double totalAmount { get; set; }
		public string expensePaiedById { get; set; }

		//public Dictionary<string, double> uniqueAmounts { get; set; }



		public Conversation conversation { get; set; }

		public string MessageText { 
			get
			{
				if (Content == null) return "";

				if (Content.Contains(".IMAGESTART.") && Content.Contains(".IMAGEEND."))
				{
					int endIndex = Content.IndexOf(".IMAGESTART.");
					return Content.Substring(0, endIndex);
				}
				else if (Content.Contains(".UNIQUEAMOUNTSSTART."))
				{
					int endIndex = Content.IndexOf(".UNIQUEAMOUNTSSTART.");
					return Content.Substring(0, endIndex);
				}

				return Content;
			}
			set { } 
		}

		public string ImageText
		{
			get
			{
				if (Content == null) return "";
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

		public Dictionary<string, double> privateUniqueAmounts { get; set; }

		public Dictionary<string, double> uniqueAmounts { 
			get {
				if (privateUniqueAmounts != null) return privateUniqueAmounts;

				privateUniqueAmounts = new Dictionary<string, double>();
				if (Content.Contains(".UNIQUEAMOUNTSSTART."))
				{
					int startIndex = Content.IndexOf(".UNIQUEAMOUNTSSTART.") + ".UNIQUEAMOUNTSSTART.".Length;
					int endIndex = Content.IndexOf(".UNIQUEAMOUNTSEND.", startIndex);
					string s = Content.Substring(startIndex, endIndex - startIndex);

					var st = s.Split(new string []{ ".UNIQUEAMOUNT."}, StringSplitOptions.None);


					foreach (string dir in st)
					{
						if (dir.Contains(".UA."))
						{
							var stdir = dir.Split(new string[] { ".UA." }, StringSplitOptions.None);
							privateUniqueAmounts.Add(stdir[0], double.Parse(stdir[1]));
						}
					}
					return privateUniqueAmounts;
				}
				return privateUniqueAmounts;
			}
			set { privateUniqueAmounts = value; } 
		} 


		public string ImageSource { get; set;  }

		public string SenderName { get {
				if (profilesInvolved != null && profilesInvolved.ContainsKey(SenderId))
				{
					return profilesInvolved[SenderId];
				}
				return "";
			} 
		}

		public bool displayImage { get {
				if (SenderId == App.userProfile.ProfileId || SenderId == "1") { return false; }
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
				if (SenderId == App.userProfile.ProfileId || SenderId == "1") { return 3;}
				return 1;
			}
		}

		public LayoutOptions horizontal
		{
			get
			{
				if (SenderId == App.userProfile.ProfileId || SenderId == "1") { return LayoutOptions.EndAndExpand; }
				return LayoutOptions.StartAndExpand;
			}
		}

		public Color bgColor
		{
			get
			{
				if (SenderId == App.userProfile.ProfileId || SenderId == "1") { return Color.Gray; }
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



		// WeShare Values




		private Dictionary<string, string> privatProfilesInvolved { get; set; }

		public Dictionary<string, string> profilesInvolved { get
			{
				if (privatProfilesInvolved == null)
				{
					privatProfilesInvolved = new Dictionary<string, string>();

					if (conversation != null)
					{
						foreach (Profile p in conversation.Profiles)
						{
							privatProfilesInvolved.Add(p.ProfileId, p.Name);
						}
					}
				}
				return privatProfilesInvolved; 
			}
			set { privatProfilesInvolved = value; }
		}

		public double myShare { 
			get {
				try
				{
					if (uniqueAmounts.Count > 0)
					{
						if (uniqueAmounts.ContainsKey(App.userProfile.ProfileId))
						{
							return Math.Round(uniqueAmounts[App.userProfile.ProfileId], 2);
						}
						else if (uniqueAmounts.ContainsKey("1"))
						{
							return Math.Round(uniqueAmounts["1"], 2);
						}
						else {
							double othersPart = 0;

							foreach (KeyValuePair<string, double> entry in uniqueAmounts)
							{
								othersPart += entry.Value;
							}

							return Math.Round((totalAmount - othersPart) / (profilesInvolved.Count - uniqueAmounts.Count), 2);
						}
					}
					return Math.Round(totalAmount / profilesInvolved.Count, 2);
				}
				catch (Exception exc) {}
				return 0.0;
			} 
			set { myShare = value; } 
		}

		public string expensePaiedByName
		{
			get
			{
				try
				{
					return profilesInvolved[expensePaiedById];
				}
				catch (Exception exc) {}
				return "";
			}
		}

		public bool weShareComment { get 
			{
				if (totalAmount != null  && totalAmount > 0) return true;
				return false;
			} 
		}
	}
}

