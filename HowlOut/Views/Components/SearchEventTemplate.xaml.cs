using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class SearchEventTemplate : ContentView
	{
		public SearchEventTemplate ()
		{
			InitializeComponent ();
		}
		public SearchEventTemplate (EventForLists eve)
		{
			InitializeComponent ();
			BindingContext = eve;

			ProfilView.Content = new ProfileDesignView (eve.eve.Owner, null, null, 80, ProfileDesignView.ProfileDesign.Plain);
			EventView.Content = new ProfileDesignView (null, null, eve.eve, 80, ProfileDesignView.ProfileDesign.Plain);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new InspectController(null,null,eve.eve),"");
			};
		}
	}
}

