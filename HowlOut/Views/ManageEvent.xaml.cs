using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEvent : ContentView
	{
		public ManageEvent ()
		{
			InitializeComponent ();
			GetEventsMatchingOwner (ManageEventList);
			ManageEventList.ItemSelected += OnItemSelected;
		}

		private async void GetEventsMatchingOwner(ListView listView)
		{
			DataManager dataManager = new DataManager();
			listView.ItemsSource = await dataManager.GetEventsWithOwnerId();
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;
			var eve = ManageEventList.SelectedItem as Event;
			App.coreView.setContentView(new InspectEvent(eve, 2), 0);
		}
	}
}

