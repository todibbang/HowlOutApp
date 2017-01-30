using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Text.RegularExpressions;


namespace HowlOut
{
	public class EventForLists
	{
		public Event eve = new Event();
		UtilityManager util = new UtilityManager();

		public string Title  {get; set;}
		public string Distance  {get; set;}
		public string topTime  {get; set;}
		public string bottomTime  {get; set;}
		public string allTime { get; set; }

		public string topProfileTime { get; set; }
		public string bottomProfileTime { get; set; }

		public string topDist { get; set; }
		public string bottomDist { get; set; }
		public string allDist { get; set; }

		public string attendingInfo { get; set; }

		public string EventAverageLoyalty  {get; set;}
		public string EventHolderLikes  {get; set;}

		public string EventType1  {get; set;}
		public string EventType2  {get; set;}
		public string EventType3  {get; set;}

		public string EventCategory1 { get; set; }
		public string EventCategory2 { get; set; }
		public string EventCategory3 { get; set; }

		public string EventVisibilityString { get; set; }

		public bool EventType2Visible  {get; set;}
		public bool EventType3Visible  {get; set;}

		public string Banner { get; set;}
		public double BannerHeight { get; set;}
		public double InspectBannerHeight { get; set;}
		public double InspectHalfBannerHeight { get; set; }

		public string ProfileImageSource { get; set;}

		public string EventHolder { get; set; }

		public bool GroupOwner { get; set; }
		public string SpecificGroupText { get; set; }
		public string SpecificGroupImageSource { get; set; }

		public bool unseenUpdate
		{
			get
			{
				return App.notificationController.checkIfUnseen(eve.EventId, NotificationModelType.Event);
			}
		}
		/*
		public bool isOrganizationOwner { get
			{
				if (eve.OrganizationOwner != null) return true;
				return false;
			}set { } } */

		public ContentView EventHolderView { get; set; }

		public EventForLists (Event theGivenEvent)
		{
			eve = theGivenEvent;
			Banner = eve.ImageSource;

			if (eve.ProfileOwners != null)
			{
				ProfileImageSource = eve.ProfileOwners[0].ImageSource;
				EventHolder = eve.ProfileOwners[0].Name;
				//EventHolderView = new ProfileDesignView(eve.ProfileOwners[0], 60, true, GenericDesignView.Design.OnlyImage);
				GroupOwner = false;
			}
			else if(eve.GroupOwner != null){
				ProfileImageSource = eve.GroupOwner.ImageSource;
				EventHolder = eve.GroupOwner.Name;
				//EventHolderView = new GroupDesignView(eve.GroupOwner, 60, GenericDesignView.Design.OnlyImage);
				GroupOwner = true;
			}
			/*
			if (eve.GroupSpecific != null)
			{
				ForSpecificGroup = true;
				SpecificGroupText = "" + eve.GroupSpecific.Name + "";
				SpecificGroupImageSource = eve.GroupSpecific.ImageSource;
			} */

			BannerHeight = (0.56 * App.coreView.Width) - 30;
			InspectBannerHeight = (0.56 * App.coreView.Width);
			InspectHalfBannerHeight = InspectBannerHeight / 2;

			Title = eve.Title;
			Position position = App.lastKnownPosition;
			Distance = util.distance(new Position(eve.Latitude, eve.Longitude), position);
			var Times = util.setTime (eve.StartDate);

			int attendees = eve.NumberOfAttendees;
			/*
			if (eve.Attendees != null)
			{
				attendees = eve.Attendees.Count + 1;
			}
			else {
				attendees = eve.NumberOfAttendees;
			} */

			EventVisibilityString = "Public";
			if (eve.Visibility == EventVisibility.Private)
			{
				EventVisibilityString = "Private";
			}


			attendingInfo = attendees + "/" + eve.MaxSize;

			topTime = eve.StartDate.ToString("ddd dd MMM");
			bottomTime = eve.StartDate.ToString("HH:mm") + "-" + eve.EndDate.ToString("HH:mm");

			topProfileTime = eve.StartDate.ToString("dd");
			bottomProfileTime = eve.StartDate.ToString("MMM");

			topDist = Distance + " km";

			allTime = bottomTime + " " + topTime;
			allDist = bottomDist + " " + topDist + " away";

			string[] addressList = new string[3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++)
			{
				Label label = new Label() { TextColor = Color.FromHex("646464") };
				label.Text = addressList[i];
				label.FontSize = 14;
			}
			try
			{
				/*
				if (addressList.Length == 2)
				{
					bottomDist = addressList[0].Substring(5).Trim();
				}
				else {
					bottomDist = addressList[1].Substring(5).Trim();
				} */
			}
			catch (Exception exc) {}

			if (eve.EventTypes.Count != 0) {
				EventType1 = eve.EventTypes [0] + "";
				EventCategory1 = "category_" + eve.EventTypes[0].ToString().ToLower() + "_ic.jpg";
			}
			if (eve.EventTypes.Count > 1) {
				EventType2 = eve.EventTypes [1] + "";
				EventType2Visible = true;
				EventCategory2 = "category_" + eve.EventTypes[1].ToString().ToLower() + "_ic.jpg";
			}
			if (eve.EventTypes.Count > 2) {
				EventType3 = eve.EventTypes [2] + "";
				EventType3Visible = true;
				EventCategory3 = "category_" + eve.EventTypes[2].ToString().ToLower() + "_ic.jpg";
			}


		}
	}
}

