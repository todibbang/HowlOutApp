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
			if (currentView == 3) bar.setNavigationlabel("Old Events");
			if (currentView == 1) bar.setNavigationlabel("Friends & Followed");


			if (currentView == 2)
			{
				var btn = bar.setNavigationlabel("  View Old Events  ");
				btn.BorderWidth = 1;
				btn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new EventListView(3));
				};
				//bar.displayNotiLayout();

				SetMainViewTopBar();
			}

			if (currentView == 0)
			{
				searchBar = bar.getSearchBar();
				searchBar.TextChanged += searchBarTextChanged;
				bar.setRightButton("ic_settings.png");
			}


			//bar.hideTopbarOnScroll();

			searchEventList.ItemAppearing += itemAppearingEvent;
			searchEventList.ItemDisappearing += itemDisappearingEvent;
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

		public StackLayout HeaderLayout { get { return headerLayout; } }

		public List<Event> evelist;
		List<Event> oldEvelist;
		List<EventForLists> eveFL;

		int lastItemToAppear = 0;
		int lastItemToDisappear = 0;
		bool isZeroVisible = true;

		string newestEveId;
		EventForLists newestEFL;
		GroupedEvents newestEFLGroup;

		private DataManager _dataManager;
		//private StandardButton standardButton = new StandardButton();
		bool beingRepositioned = false;
		int currentView = 0;
		Profile profile;
		Group group;
		string searchString = "";

		EventHandler<ItemVisibilityEventArgs> itemAppearingEvent;
		EventHandler<ItemVisibilityEventArgs> itemDisappearingEvent;
		EventHandler<TextChangedEventArgs> searchBarTextChanged;
		Entry searchBar = null;
		//Organization organization;
		//string ID;

		ListsAndButtons groups;

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

		async void setUp()
		{

			if (currentView == 0 || currentView == 2)
			{
				manageGroupsGrid.IsVisible = true;
				headerLayout.IsVisible = false;
				groups = new ListsAndButtons(null, null, false, false);
				manageGroupsLayout.Children.Add(groups);
				createBtn.IsVisible = true;
				if (currentView == 0)
				{
					exploreMoreGroups.IsVisible = false;
					exploreMoreEvents.IsVisible = false;
					eventsLabel.Text = "  Event results";
					groupsLabel.Text = "  Group results";

					searchBarTextChanged = new EventHandler<TextChangedEventArgs>(async (sender, e) =>
					{
						if (string.IsNullOrWhiteSpace(searchBar.Text))
						{
							await Task.Delay(10);
							DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
						}
						UpdateList(true, searchBar.Text);
					});
				}
				else {
					exploreMoreGroups.Clicked += (sender, e) =>
					{
						App.coreView.setContentView(1);
					};
					exploreMoreEvents.Clicked += (sender, e) =>
					{
						App.coreView.setContentView(1);
					};
				}
			}


			if (currentView == 0 || currentView == 6 || currentView == 1 || currentView == 2 || currentView == 3)
			{
				footer.HeightRequest = 60;
			}
			else if (currentView == 4 || currentView == 5)
			{
				//topBarHeight.Height = 0;
			}

			if (currentView == 2)
			{
				
				//joinedEventsGrid.IsVisible = true;
				/*viewOldEvents.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new EventListView(3));
				}; */
				/*
				if (App.userProfile.GroupsOwned.Count > 0)
				{
					headerLayout.Padding = new Thickness(0, 60, 0, 0);
					headerLayout.Children.Add(new ListsAndButtons(null, App.userProfile.GroupsOwned, true, false));
				}*/
			}

			_dataManager = new DataManager();
			/*
			if (currentView == 0)
			{
				exploreSettings.IsVisible = true;
				exploreBg.IsVisible = true;
				explorePng.IsVisible = true;
			} */
			UpdateList(true, "");
			searchEventList.ItemSelected += OnItemSelected;
			searchEventList.IsPullToRefreshEnabled = true;
			searchEventList.Refreshing += (sender, e) => { UpdateList(true, searchString); };
			exploreSettings.Clicked += (sender, e) =>
			{
				if (App.userProfile != null && App.userProfile.SearchPreference != null)
					App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference));
			};

			createBtn.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(0);
				//App.coreView.setContentViewWithQueue(new CreateView());
			};

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
			/*
			searchEventList.ItemAppearing += (sender, e) =>
			{
				var item = e.Item as EventForLists;
				if (eveFL != null && item != null)
				{
					try
					{
						int litd = eveFL.FindIndex(efl => efl.eve.EventId == item.eve.EventId);
						System.Diagnostics.Debug.WriteLine(litd);

						if (litd > lastItemToDissapear)
						{
							App.coreView.hideTopBar(true);
						}
						lastItemToDissapear = litd;
					}
					catch (Exception exc) { }
				}
			};	 */						
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
				App.notificationController.setUpdateSeen(selectedEvent.eve.EventId, NotificationModelType.Event);
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
			try
			{
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
					}
					else if (listToUpdate == 3)
					{
						evelist = await _dataManager.EventApiManager.GetEndedEvents();

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
						if (App.notificationController.checkIfUnseen(c.EventId, NotificationModelType.Event))
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

				if (listToUpdate == 3)
				{
					evelist = evelist.OrderByDescending(c => c.StartDate).ToList();
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

				eveFL = new List<EventForLists>();
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
				if (listToUpdate != 2 && listToUpdate != 3 && listToUpdate != 4 && listToUpdate != 5 && listToUpdate != 10) { mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; }); }
				else { mt = new DataTemplate(() => { return new ViewCell { View = new ManageEventTemplate() }; }); }
				//mt = new DataTemplate(() => { return new ViewCell { View = new SearchEventTemplate() }; });
				searchEventList.ItemTemplate = mt;
				searchEventList.ItemsSource = groupedEvents;
				searchEventList.IsRefreshing = false;

				foreach (GroupedEvents efl in groupedEvents)
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
				//searchEventList.ScrollTo(newestEFL, newestEFLGroup , ScrollToPosition.Start, true);
			}
			catch (Exception e) { }
		}

		public async void UpdateList(bool update, string searchText)
		{
			try
			{
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
						groups.createList(null, grps, null, null, true, false, false);
						/*
						manageGroupsGrid.IsVisible = true;
						headerLayout.IsVisible = false;
						if (manageGroupsLayout.Children.Count > 0) manageGroupsLayout.Children.RemoveAt(0);
						manageGroupsLayout.Children.Add(new ListsAndButtons(null, App.userProfile.GroupsOwned, true, false));*/
					}
				}
			}
			catch (Exception exc) {}
			searchString = searchText;
			UpdateManageList(currentView, update, searchString);
		}

		async void SetMainViewTopBar()
		{
			App.coreView.topBar.setRightButton("ic_menu.png").Clicked += async (sender, e) =>
			{
				List<Action> actions = new List<Action>();
				List<string> titles = new List<string>();
				List<string> images = new List<string>();

				actions.Add(() => { App.coreView.GoToSelectedProfile(App.userProfile.ProfileId); });
				titles.Add("View My Profile");
				images.Add("ic_me.png");

				actions.Add(async () => { 
					await App.storeToken("", "", "");
					await Navigation.PushModalAsync(new LoginPage()); });
				titles.Add("Log Out");
				images.Add("ic_settings.png");

				await App.coreView.DisplayOptions(actions, titles, images);
			};
		}
	}
}

