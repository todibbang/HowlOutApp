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

			attendingInfo.Text = (eve.NumberOfAttendees + 1) + "/" + eve.MaxSize;

			//ProfilView1.SetBinding (Content, "ProfileLayout");
			//ProfilView0.Children.Add (new ContentView(){Content = "{}"});

			//ProfilView.Content = new ProfileDesignView (eve.Owner, null, null, 80, ProfileDesignView.Design.Plain, false);
			//EventView.Content = new EventDesignView (eve, 80, EventDesignView.Design.Plain);

			SubjectButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue(new InspectController(null,null,eve),"");
			};
		}
	}
}

