using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class SearchEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public SearchEventTemplate ()
		{
			InitializeComponent ();
		}
		public SearchEventTemplate (Event eve)
		{
			InitializeComponent ();
			BindingContext = new EventForLists(eve);

			ProfilView.Content = new ProfileDesignView (eve.Owner, null, null, 80, ProfileDesignView.ProfileDesign.Plain);
			EventView.Content = new ProfileDesignView (null, null, eve, 80, ProfileDesignView.ProfileDesign.Plain);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(null,null,eve),"");
			};
		}
	}
}

