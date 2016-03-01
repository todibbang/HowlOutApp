using System;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public class EventForLists
	{
		public Event eve = new Event();

		UtilityManager util = new UtilityManager();

		public string Title  {get; set;}
		public string Distance  {get; set;}
		public string Time  {get; set;}
		public string Attendees  {get; set;}
		public Uri ProfileImageUri { get; set;}

		public string EventAverageLoyalty  {get; set;}
		public string EventHolderLikes  {get; set;}

		public EventForLists (Event theGivenEvent)
		{
			eve = theGivenEvent;
			ProfileImageUri = eve.ProfileImageUri;

			Title = eve.Title;

			Position position = util.getCurrentUserPosition();
			Distance = util.distance(eve.AddressPosition, position);

			Time = "" + eve.StartDate.DayOfWeek + " at " + util.getTime(eve.StartDate);

			Attendees = eve.Attendees.Count + "/" + eve.MaxSize;

			EventAverageLoyalty = eve.Attendees [0].LoyaltyRating + "";
			EventHolderLikes = eve.Attendees [0].Likes + "";

		}
	}
}

