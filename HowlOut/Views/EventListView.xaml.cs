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
		public void viewInFocus(UpperBar bar){ }
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		public List<Event> evelist;
		List<Event> oldEvelist;

		string newestEveId;
		EventForLists newestEFL;
		GroupedEvents newestEFLGroup;

		private DataManager _dataManager;
		private StandardButton standardButton = new StandardButton();
		bool beingRepositioned = false;
		int currentView = 0;
		Profile profile;
		Group group;
		string searchString = "";
		//Organization organization;
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
		/*
		public EventListView(Organization org)
		{
			InitializeComponent();
			organization = org;
			currentView = 6;
			setUp();
		} */

		public EventListView(int viewType)
		{
			InitializeComponent();
			currentView = viewType;
			setUp();
		}

		void setUp()
		{
			if (currentView == 0 || currentView == 6 || currentView == 1)
			{
				headerLayout.HeightRequest = 50;
			}

			_dataManager = new DataManager();
			if (currentView == 0)
			{
				exploreSettings.IsVisible = true;
				exploreBg.IsVisible = true;
				explorePng.IsVisible = true;
			}
			UpdateList( true, "");
			searchEventList.ItemSelected += OnItemSelected;
			searchEventList.IsPullToRefreshEnabled = true;
			searchEventList.Refreshing += (sender, e) => { UpdateList(true, searchString); };
			exploreSettings.Clicked += (sender, e) =>
			{
				if (App.userProfile != null && App.userProfile.SearchPreference != null)
					App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference));
			};
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

			if (currentView == 2)
			{
				_dataManager.setUpdateSeen(selectedEvent.eve.EventId, NotificationModelType.Event);
			}

			InspectController inspect = new InspectController(selectedEvent.eve);
			App.coreView.setContentViewWithQueue(inspect);
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
			App.coreView.setContentViewWithQueue(inspect);

		}

		public async void UpdateManageList(int listToUpdate, bool update, string searchText)
		{
			try {
				nothingToLoad.IsVisible = false;
				currentView = listToUpdate;
				var first = DateTime.Now;
				if (update)
				{
					evelist = new List<Event>();

					if (listToUpdate == 0)
					{
						evelist = await _dataManager.EventApiManager.SearchEvents(searchText);
					}
					else if (listToUpdate == 10)
					{
						evelist = await _dataManager.EventApiManager.GetEndedEvents();
					}
					else if (listToUpdate == 1)
					{
						evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, App.userProfile.Friends);

						var extraEvents = new List<Event>();
						extraEvents = await _dataManager.EventApiManager.GetEventsProfilesAttending(false, new List<Profile> { App.userProfile });
						if (extraEvents != null && extraEvents.Count > 0)
						{
							if (evelist != null)
							{
								foreach (Event exeve in extraEvents)
								{
									if (!evelist.Exists(xe => xe.EventId == exeve.EventId))
									{
										evelist.Add(exeve);
									}
								}
							}
							else {
								evelist = extraEvents;
							}
						}

					}
					else if (listToUpdate == 2)
					{
						evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { App.userProfile });
						oldEvelist = await _dataManager.EventApiManager.GetEndedEvents();
						footer.HeightRequest = App.coreView.Height;
					}
					else if (listToUpdate == 3)
					{
						evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(false, new List<Profile> { App.userProfile });

					}
					else if (listToUpdate == 4)
					{
						evelist = await _dataManager.EventApiManager.GetEventsProfilesAttending(true, new List<Profile> { profile });
						HeightRequest = evelist.Count * 160;
						HeightRequest += 60;
						footer.HeightRequest = 0;
					}
					else if (listToUpdate == 5)
					{
						evelist = await _dataManager.EventApiManager.GetEventsForGroups(new List<Group> { group });
						HeightRequest = evelist.Count * 160;
						HeightRequest += 60;
						footer.HeightRequest = 0;
						//if (HeightRequest > 200) HeightRequest = 200;
					}

					else if (listToUpdate == 6)
					{
						evelist = await _dataManager.EventApiManager.SearchEvents(searchText);
					} 
				}
				if (listToUpdate == 2 || listToUpdate == 10)
				{
					int n = 0;
					foreach (Event c in evelist)
					{
						if (_dataManager.checkIfUnseen(c.EventId, NotificationModelType.Event))
						{
							n++;
						}
					}
					//App.coreView.setEventsNoti(n);
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

				if (evelist == null || evelist.Count == 0)
				{
					if (oldEvelist == null || oldEvelist.Count == 0)
					{
						nothingToLoad.IsVisible = true;
						searchEventList.IsRefreshing = false;
						searchEventList.ItemsSource = null;
						return;
					}
				}

				searchEventList.ItemsSource = null;

				//var orderedList = new ObservableCollection<Event>();
				if (evelist != null && evelist.Count > 0)
				{
					evelist = evelist.OrderBy(c => c.StartDate).ToList();
					newestEveId = evelist[0].EventId;
				}


				if (oldEvelist != null && oldEvelist.Count > 0)
				{
					oldEvelist = oldEvelist.OrderByDescending(c => c.StartDate).ToList();
					oldEvelist.AddRange(evelist);
					if (evelist == null || evelist.Count == 0)
					{
						newestEveId = oldEvelist[0].EventId;
					}

					evelist = oldEvelist;
				}



				//Event itemToAdd = new Event();
				/*
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
				*/

				List<EventForLists> eveFL = new List<EventForLists>();
				foreach (Event eve in evelist)
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
						if (eveFL[d].eve.EventId == newestEveId)
						{
							newestEFL = eveFL[d];
							newestEFLGroup = monthGroup;
						}

						if (d == eveFL.Count - 1)
						{
							groupedEvents.Add(monthGroup);
						}
					}
				}

				searchEventList.IsVisible = true;
				DataTemplate mt = null;
				if (listToUpdate != 2 && listToUpdate != 4 && listToUpdate != 5 && listToUpdate != 10) { mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; }); }
				else { mt = new DataTemplate(() => { return new ViewCell { View = new ManageEventTemplate() }; }); }
				//mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; });
				searchEventList.ItemTemplate = mt;
				searchEventList.ItemsSource = groupedEvents;
				searchEventList.IsRefreshing = false;

				foreach(GroupedEvents efl in groupedEvents)
				{
					if (efl.ToList().Exists(newEve => newEve.eve.EventId == newestEveId))
					{
						newestEFLGroup = efl;
					}
				}
				/*
				if (listToUpdate == 2)
				{
					await Task.Delay(1000);
				} */
				searchEventList.ScrollTo(newestEFL, newestEFLGroup , ScrollToPosition.Start, true);
			}
			catch (Exception e) {}
		} 

		public void UpdateList(bool update, string searchText)
		{
			searchString = searchText;
			UpdateManageList(currentView, update, searchString);
		}
	}
}

