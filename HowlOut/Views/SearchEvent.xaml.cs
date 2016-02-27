using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class SearchEvent : ContentView
	{
		ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();

		public SearchEvent ()
		{
			InitializeComponent ();
			GetAllEvents (SearchEventList);
			SearchEventList.ItemSelected += OnItemSelected;
		}

		private async void GetAllEvents(ListView listView)
		{
			DataManager dataManager = new DataManager();
			var eve = await dataManager.GetAllEvents();
			for (int i = 0; i < eve.Count; i++) {
				EventForLists EveForLis = new EventForLists (eve[i]);
				listEvents.Add (EveForLis);
			}
			listView.ItemsSource = listEvents;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(SearchEventList.SelectedItem == null)
				return;
			var eveForLis = SearchEventList.SelectedItem as EventForLists;
			App.coreView.setContentView(new InspectEvent(eveForLis.eve, 1), 0);

		}
	}
}

