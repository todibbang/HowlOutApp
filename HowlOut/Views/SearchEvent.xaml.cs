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
			//GetAllEvents (SearchEventList);
			SearchEventList.ItemSelected += OnItemSelected;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(SearchEventList.SelectedItem == null)
				return;
			var eveForLis = SearchEventList.SelectedItem as EventForLists;
			App.coreView.setContentView(new InspectEvent(eveForLis.eve, 1), "InspectEvent");
			SearchEventList.SelectedItem = null;
		}

		public void updateList(){
			var eve = App.coreView.searchEventList;
			listEvents.Clear ();
			for (int i = 0; i < eve.Count; i++) {
				EventForLists EveForLis = new EventForLists (eve [i]);
				listEvents.Add (EveForLis);
			}
			SearchEventList.ItemsSource = listEvents;
		}
	}
}

