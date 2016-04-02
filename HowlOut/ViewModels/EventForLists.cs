using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace HowlOut
{
	public class EventForLists
	{
		public Event eve = new Event();
		UtilityManager util = new UtilityManager();

		public string Title  {get; set;}
		public string Distance  {get; set;}
		public string BigTime  {get; set;}
		public string SmallTime  {get; set;}

		public string EventAverageLoyalty  {get; set;}
		public string EventHolderLikes  {get; set;}

		public string EventType1  {get; set;}
		public string EventType2  {get; set;}
		public string EventType3  {get; set;}

		public bool EventType2Visible  {get; set;}
		public bool EventType3Visible  {get; set;}

		public string Banner { get; set;}
		public double BannerHeight { get; set;}


		public EventForLists (Event theGivenEvent)
		{
			eve = theGivenEvent;
			Banner = eve.BannerName;
			BannerHeight = (0.524 * App.coreView.Width) - 60;
			Title = eve.Title;
			Position position = App.lastKnownPosition;
			Distance = util.distance(new Position(eve.Latitude, eve.Longitude), position);
			var Times = util.setTime (eve.StartDate);
			BigTime = Times [0];
			SmallTime = Times [1];
			EventAverageLoyalty = eve.Owner.LoyaltyRating + "";
			EventHolderLikes = eve.Owner.Likes + "";

			if (eve.EventTypes.Count != 0) {
				System.Diagnostics.Debug.WriteLine ("EventTypes works");
				EventType1 = eve.EventTypes [0] + "";
			}
			if (eve.EventTypes.Count > 1) {
				EventType2 = eve.EventTypes [1] + "";
				EventType2Visible = true;
			}
			if (eve.EventTypes.Count > 2) {
				EventType3 = eve.EventTypes [2] + "";
				EventType3Visible = true;
			}
		}
	}
}

