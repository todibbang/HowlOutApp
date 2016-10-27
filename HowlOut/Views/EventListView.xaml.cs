using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class EventListView : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		private DataManager _dataManager;
		private StandardButton standardButton = new StandardButton();
		bool beingRepositioned = false;
		int currentView = 0;
		Profile profile;
		Group group;
		Organization organization;
		//string ID;

		public EventListView(Profile pro)
		{
			InitializeComponent();
			profile = pro;
			currentView = 4;
			setUp();
		}

		public EventListView(Group grp)
		{
			InitializeComponent();
			group = grp;
			currentView = 5;
			setUp();
		}

		public EventListView(Organization org)
		{
			InitializeComponent();
			organization = org;
			currentView = 6;
			setUp();
		}

		public EventListView(int viewType)
		{
			InitializeComponent();
			currentView = viewType;
			setUp();
		}

		void setUp()
		{
			_dataManager = new DataManager();
			UpdateList();
			searchEventList.ItemSelected += OnItemSelected;
			searchEventList.IsPullToRefreshEnabled = true;
			searchEventList.Refreshing += (sender, e) => { UpdateList(); };
		}

		public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			ListView list = null;
			if (searchEventList.SelectedItem != null)
			{
				list = searchEventList;
			}
			else return;

			var selectedEvent = list.SelectedItem as EventForLists;

			InspectController inspect = new InspectController(selectedEvent.eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
			list.SelectedItem = null;

			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}

		void OnPanUpdated(object sender, PanUpdatedEventArgs e) 
		{
			System.Diagnostics.Debug.WriteLine(e.TotalX + " " + e.TotalX);
		}

		private async void GoToSelectedEvent(string eveID)
		{
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);

			InspectController inspect = new InspectController(eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());

		}

		public async void UpdateManageList(int listToUpdate)
		{
			loading.IsVisible = true;
			ErrorLoading.IsVisible = false;
			List<Event> evelist = new List<Event>();
			currentView = listToUpdate;
			var first = DateTime.Now;
			if (listToUpdate == 0)
			{
				evelist = await _dataManager.EventApiManager.SearchEvents();
			}
			else if (listToUpdate == 1)
			{
				evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending( true, App.userProfile.Friends);

			}
			else if (listToUpdate == 2)
			{
				evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { App.userProfile });

			}
			else if (listToUpdate == 3)
			{
				evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(false, new List<Profile> { App.userProfile });

			}
			else if (listToUpdate == 4)
			{
				evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { profile });

			}
			else if (listToUpdate == 5)
			{
				evelist = await _dataManager.EventApiManager.GetEventsForGroups(new List<Group> { group });
				HeightRequest = evelist.Count * 90;
				HeightRequest += 20;
				if (HeightRequest > 200) HeightRequest = 200;
			}
			else if (listToUpdate == 6)
			{
				evelist = await _dataManager.EventApiManager.GetEventsForOrgs(new List<Organization> { organization });
				HeightRequest = evelist.Count * 90;
				HeightRequest += 20;
				if (HeightRequest > 200) HeightRequest = 200;
			}
			/*
			else if (listToUpdate == 2)
			{
				evelist = await _dataManager.ProfileApiManager.GetEventsInvitedTo();
				var evesAttended = await _dataManager.EventApiManager.GetEventsWithOwnerId(ID);
				for (int i = evelist.Count - 1; i > -1; i--)
				{
					for (int m = 0; m < evesAttended.Count; m++)
					{
						if (evelist[i].EventId == evesAttended[m].EventId)
						{
							evelist.RemoveAt(i);
							break;
						}
					}
				}
			}
			*/
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine("Time to load: " + (time.Milliseconds) + " ms");

			if (evelist == null)
			{
				loading.IsVisible = false;
				searchEventList.IsRefreshing = false;
				return;
			} else if (evelist.Count == 0) {
				loading.IsVisible = false;
				searchEventList.IsRefreshing = false;
				searchEventList.ItemsSource = null;
				return;
			}
						searchEventList.ItemsSource = null;

			var orderedList = new ObservableCollection<Event>();

			Event itemToAdd = new Event();
			while (evelist.Count != 0)
			{
				DateTime Time = evelist[0].StartDate;
				itemToAdd = evelist[0];

				for (int i = 0; i < evelist.Count; i++)
				{
					if (evelist[i].StartDate < Time)
					{
						itemToAdd = evelist[i];
						Time = itemToAdd.StartDate;
					}
				}
				orderedList.Add(itemToAdd);
				evelist.Remove(itemToAdd);
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
			if (listToUpdate == 0 || listToUpdate == 1) { mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; }); }
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
	}
}

