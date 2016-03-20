﻿using System;
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
		public string Time  {get; set;}
		public string Attendees  {get; set;}
		public Uri ProfileImageUri { get; set;}

		public string EventAverageLoyalty  {get; set;}
		public string EventHolderLikes  {get; set;}
		public ContentView ProfileContent { get; set;}
		public ContentView GroupContent { get; set;}

		public string EventType1  {get; set;}
		public string EventType2  {get; set;}
		public string EventType3  {get; set;}

		public bool EventType2Visible  {get; set;}
		public bool EventType3Visible  {get; set;}

		public EventForLists (Event theGivenEvent)
		{
			eve = theGivenEvent;
			ProfileImageUri = eve.ProfileImageUri;

			Title = eve.Title;

			Position position = App.lastKnownPosition;
			Distance = util.distance(new Position(eve.Latitude, eve.Longitude), position);

			Time = "" + eve.StartDate.DayOfWeek + " at " + util.getTime(eve.StartDate);

			Attendees = eve.Attendees.Count + "/" + eve.MaxSize;

			EventAverageLoyalty = eve.Attendees [0].LoyaltyRating + "";
			EventHolderLikes = eve.Attendees [0].Likes + "";

			//ProfileContent = new ProfileDesignView (eve.Attendees[0], null, 80, ProfileDesignView.ProfileDesign.Plain);
			//GroupContent = new ProfileDesignView (eve.Attendees[0], null, 80, ProfileDesignView.ProfileDesign.Plain);

			System.Diagnostics.Debug.WriteLine (eve.EventTypes.Count + "");

			EventType2Visible = false;
			EventType3Visible = false;

			EventType1 =  eve.EventTypes [0] + "";
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

