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


			App.coreView.setContentView (new UserProfile (null, null, eveForLis.eve, false, true), "UserProfile");
			//App.coreView.setContentView(new InspectEvent(eveForLis.eve, 1), "InspectEvent");
			SearchEventList.SelectedItem = null;
		}

		public void updateList(ObservableCollection<Event> evelist){
			//var eve = App.coreView.searchEventList;
			listEvents.Clear ();
			for (int i = 0; i < evelist.Count; i++) 
			{
				System.Diagnostics.Debug.WriteLine ("List: " + i + "");
				EventForLists EveForLis = new EventForLists (evelist [i]);
				listEvents.Add (EveForLis);

			}
			SearchEventList.ItemsSource = listEvents;
		}
	}
}

