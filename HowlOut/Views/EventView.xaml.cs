using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class EventView : ContentView
	{
		List<VisualElement> eventViews = new List<VisualElement>();

		private DataManager _dataManager;
		private StandardButton standardButton = new StandardButton();
		bool beingRepositioned = false;
		int currentView = 0;

		string ID;
		int EventViewType;

		public int lastCarouselView = 0;


		public EventView (int viewType)
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			EventViewType = viewType;

			if (EventViewType == 1) {
				eventViews.Add(new EventListView(0));
				eventViews.Add(new EventListView(1));
				App.setOptionsGrid(optionGrid, new List<string> { "Explore", "Friends" }, new List<VisualElement> { null, null }, new List<Action> {() => { eventCarousel.Position = 0; }, () => { eventCarousel.Position = 1; } }, eventCarousel);
			} else if (EventViewType != 10) {
				eventViews.Add(new EventListView(2));
				eventViews.Add(new EventListView(3));
				App.setOptionsGrid(optionGrid, new List<string> { "Join", "Track" }, new List<VisualElement> { null, null }, new List<Action> { () => { eventCarousel.Position = 0; }, () => { eventCarousel.Position = 1; } }, eventCarousel);
			}

			eventCarousel.ItemsSource = eventViews;

			eventCarousel.PositionSelected += (sender, e) =>
			{
				lastCarouselView = eventCarousel.Position;
			};

			/*
			searchEventList.ItemSelected += OnItemSelected;
			searchEventList.IsPullToRefreshEnabled = true;
			searchEventList.Refreshing += (sender, e) => { UpdateList(); };
			*/
			getFacebookEvents();
		}

		public async Task setLastCarousel()
		{
			eventCarousel.Position = 0;
			await Task.Delay(20);
			eventCarousel.Position = lastCarouselView;
		}

		public async Task getFacebookEvents()
		{
			//TODO fix facebook server call
			//ObservableCollection<FaceBookEvent> facebookEvents = await _dataManager.EventApiManager.getFacebookEvents();
			//facebookEvents = await _dataManager.EventApiManager.getFacebookEvents();
			System.Diagnostics.Debug.WriteLine("bæ");
		}

		/*
		public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			ListView list = null;
			if (searchEventList.SelectedItem != null)
			{
				list = searchEventList;
			}
			else return;

			var selectedEvent = list.SelectedItem as EventForLists;

			InspectController inspect = new InspectController(null, null, selectedEvent.eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
			list.SelectedItem = null;

			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}

		void OnPanUpdated (object sender, PanUpdatedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.TotalX + " " + e.TotalX);
		}
			
		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);

			InspectController inspect = new InspectController(null, null, eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());

		}

		public async void UpdateManageList(int listToUpdate){
			loading.IsVisible = true;
			ObservableCollection<Event> evelist = new ObservableCollection<Event>();
			currentView = listToUpdate;
			var first = DateTime.Now;
			if (listToUpdate == 0) {
				evelist = await _dataManager.EventApiManager.SearchEvents ();

			} else if (listToUpdate == 1) {
				evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId (ID);

			} else if (listToUpdate == 3) {
				evelist = await _dataManager.ProfileApiManager.GetEventsFollowed ();

			} else if (listToUpdate == 2) {
				evelist = await _dataManager.ProfileApiManager.GetEventsInvitedTo ();
				var evesAttended = await _dataManager.EventApiManager.GetEventsWithOwnerId (ID);
				for (int i = evelist.Count -1; i > -1; i--) {
					for (int m = 0; m < evesAttended.Count; m++) {
						if (evelist [i].EventId == evesAttended[m].EventId) {
							evelist.RemoveAt (i);
							break;
						}
					}
				}
			}
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");

			var orderedList = new ObservableCollection<Event>();

			Event itemToAdd = new Event();
			while(evelist.Count != 0){
				DateTime Time = evelist [0].StartDate;
				itemToAdd = evelist [0];

				for (int i = 0; i < evelist.Count; i++) {
					if (evelist [i].StartDate < Time) {
						itemToAdd = evelist [i];
						Time = itemToAdd.StartDate;
					}
				}
				orderedList.Add (itemToAdd);
				evelist.Remove (itemToAdd);
			}

			List<EventForLists> eveFL = new List<EventForLists>();
			foreach (Event eve in orderedList)
			{
				eveFL.Add(new EventForLists(eve));
			}

			ObservableCollection<GroupedEvents> groupedEvents = new ObservableCollection<GroupedEvents>();
			if (eveFL.Count > 0)
			{
				GroupedEvents monthGroup = null;
				int month = eveFL[0].eve.StartDate.Month;

				for (int d = 0; d < eveFL.Count; d++)
				{
					if (d == 0)
					{
						monthGroup = new GroupedEvents() { Date = (eveFL[d].eve.StartDate.ToString("MMMMM")) };
					}
					if (month != eveFL[d].eve.StartDate.Month)
					{
						month = eveFL[d].eve.StartDate.Month;
						groupedEvents.Add(monthGroup);
						monthGroup = new GroupedEvents() { Date = (eveFL[d].eve.StartDate.ToString("MMMMM")) };
					}
					monthGroup.Add(eveFL[d]);
					if (d == eveFL.Count - 1)
					{
						groupedEvents.Add(monthGroup);
					}
				}
			}
			searchEventList.IsVisible = true;
			DataTemplate mt = null;
			if (listToUpdate == 0) { mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; }); }
			else { mt = new DataTemplate(() => { return new ViewCell { View = new ManageEventTemplate() }; }); }
			searchEventList.ItemTemplate = mt;
			searchEventList.ItemsSource = groupedEvents;
			searchEventList.IsRefreshing = false;
			loading.IsVisible = false;
		}

		public void UpdateList()
		{
			UpdateManageList(currentView);
		}
		*/
	}
}

