using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEvent : ContentView
	{
		ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();

		public ManageEvent ()
		{
			InitializeComponent ();
			GetEventsMatchingOwner (ManageEventList);
			ManageEventList.ItemSelected += OnItemSelected;
		}

		private async void GetEventsMatchingOwner(ListView listView)
		{
			DataManager dataManager = new DataManager();
			var eve = await dataManager.GetEventsWithOwnerId();
			for (int i = 0; i < eve.Count; i++) {
				EventForLists EveForLis = new EventForLists (eve[i]);
				listEvents.Add (EveForLis);
			}
			listView.ItemsSource = listEvents;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;
			var eveForLis = ManageEventList.SelectedItem as EventForLists;
			App.coreView.setContentView(new InspectEvent(eveForLis.eve, 2), 0);
		}
	}
}

