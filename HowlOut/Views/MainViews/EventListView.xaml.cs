using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class EventListView : ContentView, ViewModelInterface
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}
		public void reloadView() { }
		public void viewInFocus(UpperBar bar)
		{
			App.coreView.hideTopBar(false);
			if (currentView == 0)
			{
				searchBar = bar.getSearchBar();
				searchBar.TextChanged += searchBarTextChanged;
				bar.showLeftButton();
			}
			if (currentView == 1) bar.setNavigationlabel("Friends & Followed");
			if (currentView == 2)
			{
				var btn = bar.setNavigationlabel("  View Old Events  ");
				btn.BorderWidth = 1;
				btn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new EventListView(3));
				};
			}
			if (currentView == 3) bar.setNavigationlabel("Old Events");

			searchEventList.ItemAppearing += itemAppearingEvent;
			searchEventList.ItemDisappearing += itemDisappearingEvent;
		}

		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			App.coreView.hideTopBar(false);
			if (currentView == 0)
			{
				searchBar = ub.getSearchBar();
				searchBar.TextChanged += searchBarTextChanged;
				ub.showLeftButton();
			}
			if (currentView == 1) ub.setNavigationlabel("Friends & Followed");
			if (currentView == 2)
			{
				var btn = ub.setNavigationlabel("  View Old Events  ");
				btn.BorderWidth = 1;
				btn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new EventListView(3));
				};
			}
			if (currentView == 3) ub.setNavigationlabel("Old Events");

			searchEventList.ItemAppearing += itemAppearingEvent;
			searchEventList.ItemDisappearing += itemDisappearingEvent;

			return ub;
		}

		public void viewExitFocus() {
			searchEventList.ItemAppearing -= itemAppearingEvent;
			searchEventList.ItemDisappearing -= itemDisappearingEvent;
			if (currentView == 0)
			{
				try
				{
					searchBar.TextChanged -= searchBarTextChanged;
				}
				catch (Exception exc) {}
			}
		}
		public ContentView getContentView() { return this; }
		//public StackLayout HeaderLayout { get { return headerLayout; } }
		public List<Event> evelist;
		List<EventForLists> eveFL;

		int lastItemToAppear = 0;
		int lastItemToDisappear = 0;
		bool isZeroVisible = true;

		DataManager _dataManager;
		int currentView = 0;
		Profile profile;
		Group group;
		string searchString = "";
		Event upcomingEvent;

		EventHandler<ItemVisibilityEventArgs> itemAppearingEvent;
		EventHandler<ItemVisibilityEventArgs> itemDisappearingEvent;
		EventHandler<TextChangedEventArgs> searchBarTextChanged;
		Entry searchBar = null;

		ListsAndButtons groupsList;

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

		public EventListView(int viewType)
		{
			InitializeComponent();
			currentView = viewType;
			setUp();
		}

		async void setUp()
		{
			if (currentView == 0)
			{
				headerGrid.IsVisible = true;
				groupsList = new ListsAndButtons(null, null, true, false);
				manageGroupsLayout.Children.Add(groupsList);
				searchBarTextChanged = new EventHandler<TextChangedEventArgs>(async (sender, e) =>
				{
					if (string.IsNullOrWhiteSpace(searchBar.Text))
					{
						await Task.Delay(10);
						DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
					}
					UpdateList(true, searchBar.Text);
				});
				viewMyEventsButton.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new EventListView(2));
				};
				upcomingEventButton.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new InspectController(upcomingEvent));
				};
			}

			_dataManager = new DataManager();
			UpdateList(true, "");
			searchEventList.ItemSelected += OnItemSelected;
			searchEventList.IsPullToRefreshEnabled = true;
			searchEventList.Refreshing += (sender, e) => { UpdateList(true, searchString); };
			itemAppearingEvent = new EventHandler<ItemVisibilityEventArgs>(async (sender, e) =>
			{
				var item = e.Item as EventForLists;
				if (eveFL != null && item != null)
				{
					try
					{
						int lita = eveFL.FindIndex(efl => efl.eve.EventId == item.eve.EventId);
						System.Diagnostics.Debug.WriteLine(lita);

						if (lita < 2)
						{
							isZeroVisible = true;
							await Task.Delay(250);
							App.coreView.hideTopBar(false);
						} 
						else if (lita > lastItemToAppear && !isZeroVisible)
						{
							App.coreView.hideTopBar(true);
						}
						lastItemToAppear = lita;
					}
					catch (Exception exc) { }
				}
			});

			itemDisappearingEvent = new EventHandler<ItemVisibilityEventArgs>((sender, e) =>
			{
				var item = e.Item as EventForLists;
				if (eveFL != null && item != null)
				{
					try
					{
						int litd = eveFL.FindIndex(efl => efl.eve.EventId == item.eve.EventId);
						System.Diagnostics.Debug.WriteLine("lastItemToDisappear" + litd);

						if (litd == 0 || litd == 1) isZeroVisible = false;

						if (litd < lastItemToDisappear)
						{
							App.coreView.hideTopBar(false);
						}
						lastItemToDisappear = litd;
					}
					catch (Exception exc) { }
				}
			});					
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
			App.coreView.setContentViewWithQueue(new InspectController(selectedEvent.eve));
			list.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			await App.notificationController.setUpdateSeen(selectedEvent.eve.EventId, NotificationModelType.Event);
		}

		public async void UpdateEventList(int listToUpdate, bool update, string searchText)
		{
			try
			{
				closeAsync();
				nothingToLoad.IsVisible = false;
				currentView = listToUpdate;
				if (update)
				{
					evelist = await loadEvents(listToUpdate, searchText);
				}

				if (listToUpdate == 4 || listToUpdate == 5)
				{
					HeightRequest = evelist.Count * 180;
					HeightRequest += 60;
					footer.HeightRequest = 0;
				}

				if (evelist == null || evelist.Count == 0)
				{
					nothingToLoad.IsVisible = true;
					searchEventList.IsRefreshing = false;
					searchEventList.ItemsSource = null;
					return;
				}

				if (listToUpdate == 3)
					evelist = evelist.OrderByDescending(c => c.StartDate).ToList();
				else 
					evelist = evelist.OrderBy(c => c.StartDate).ToList();

				eveFL = new List<EventForLists>();
				foreach (Event eve in evelist)
				{
					eveFL.Add(new EventForLists(eve));
				}

				if (upcomingEventsList.Children.Count > 0) upcomingEventsList.Children.RemoveAt(0);
				upcomingEventsList.Children.Add(new SearchEventTemplate() { BindingContext = eveFL[0] });
				upcomingEvent = eveFL[0].eve;
				var groupedEvents = new ObservableCollection<GroupedEvents>();
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
				searchEventList.ItemsSource = groupedEvents;
				searchEventList.IsRefreshing = false;
			}
			catch (Exception e) { System.Diagnostics.Debug.WriteLine(e.Message); }
		}

		async void closeAsync()
		{
			await Task.Delay(2000);
			if (searchEventList.IsRefreshing) searchEventList.IsRefreshing = false;
			if (searchEventList.ItemsSource == null) UpdateList(true, searchString);
		}

		public async Task<List<Event>> loadEvents(int listToUpdate, string searchText)
		{
			var eventList = new List<Event>();
			if (listToUpdate == 0)
			{
				eventList = await _dataManager.EventApiManager.SearchEvents(searchText);
				showHideUpcomingEvent(searchText);
			}
			else if (listToUpdate == 1)
			{
				eventList = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, App.userProfile.Friends);

				var extraEvents = new List<Event>();
				extraEvents = await _dataManager.EventApiManager.GetEventsProfilesAttending(false, new List<Profile> { App.userProfile });
				if (extraEvents != null && extraEvents.Count > 0)
				{
					if (eventList != null)
					{
						eventList.AddRange(extraEvents.FindAll(xe => !eventList.Exists(e => e.EventId == xe.EventId)));
					}
					else {
						eventList = extraEvents;
					}
				}
			}
			else if (listToUpdate == 2)
			{
				eventList = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { App.userProfile });
			}
			else if (listToUpdate == 3)
			{
				eventList = await _dataManager.EventApiManager.GetEndedEvents();
			}
			else if (listToUpdate == 4)
			{
				eventList = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { profile });
			}
			else if (listToUpdate == 5)
			{
				eventList = await _dataManager.EventApiManager.GetEventsForGroups(new List<Group> { group });
			}
			return eventList;
		}

		public async Task UpdateList(bool update, string searchText)
		{
			try
			{
				await App.coreView._dataManager.ProfileApiManager.GetLoggedInProfile();
				if (currentView == 2 || currentView == 0)
				{
					var grps = new List<Group>();

					if (currentView == 2 || string.IsNullOrEmpty(searchText))
					{
						grps.AddRange(App.userProfile.GroupsOwned);
						grps.AddRange(App.userProfile.Groups);
					}
					else {
						grps = await _dataManager.GroupApiManager.GetGroupsFromName(searchText);
					}

					if (grps.Count > 0)
					{
						groupsList.createList(null, grps, null, null, true, false, false);
					}
				}
			}
			catch (Exception exc) {}
			searchString = searchText;
			UpdateEventList(currentView, update, searchString);
		}

		void showHideUpcomingEvent(string txt)
		{
			if (string.IsNullOrWhiteSpace(txt))
			{
				if (eventsLabel.Text == "  Event results")
				{
					eventsLabel.Text = "  Suggested Events";
					groupsLabel.Text = "  My Groups";
					AnimationController.ShowAnimation(upcomingEventsGrid, 200);
				}
			}
			else {
				if (eventsLabel.Text != "  Event results")
				{
					eventsLabel.Text = "  Event results";
					groupsLabel.Text = "  Group results";
					AnimationController.HideAnimation(upcomingEventsGrid, 200);
				}
			}
		}
	}
}

