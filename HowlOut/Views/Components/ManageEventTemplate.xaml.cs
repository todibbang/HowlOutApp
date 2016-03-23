using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public ManageEventTemplate ()
		{
			InitializeComponent ();
		}
		public ManageEventTemplate (EventForLists eve)
		{
			InitializeComponent ();
			BindingContext = eve;

			distance.Text = eve.Distance + " km";
			var times = _dataManager.UtilityManager.setTime (eve.eve.StartDate);
			time.Text = times[0] + " " + times[1];
			//address.Text = "At " + eve.eve.AddressName;

			ProfilView.Content = new ProfileDesignView (eve.eve.Owner, null, null, 80, ProfileDesignView.ProfileDesign.Plain);
			EventView.Content = new ProfileDesignView (null, null, eve.eve, 80, ProfileDesignView.ProfileDesign.Plain);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(null,null,eve.eve),"");
			};
		}
	}
}

