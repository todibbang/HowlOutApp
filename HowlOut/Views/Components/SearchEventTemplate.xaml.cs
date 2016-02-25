using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class SearchEventTemplate : ContentView
	{
		UtilityManager util = new UtilityManager();

		public SearchEventTemplate ()
		{
			InitializeComponent ();
			//Event eve = this.BindingContext;
			//Position position = util.getCurrentUserPosition();

			//time.Text = "" + eve.StartDate.DayOfWeek + " at " + util.getTime(eve.StartDate);
			//distance.Text = "" + util.distance(eve.Latitude, eve.Longitude, position.Latitude, position.Longitude);

		}
	}
}

