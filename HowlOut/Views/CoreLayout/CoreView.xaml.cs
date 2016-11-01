using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		public List <ContentView> contentViews = new List<ContentView> ();
		public List <ScrollView> scrollViews = new List<ScrollView>();
		public List <string> contentViewTypes = new List<string> ();

		public UpperBar topBar;
		public DataManager _dataManager;
		public OtherFunctions otherFunctions = new OtherFunctions();

		//public CreateEvent createEventView;

		public CarouselList createView;
		public CarouselList manageEventView;
		public CarouselList searchEventView;
		public CarouselList howlsView;

		//public CreateView createView;
		//public EventView manageEventView;
		//public EventView exploreEventView;
		//public HowlsView howlsView;
		public HomeView homeView;

		public CreateEvent createEvent;
		public CreateGroup createGroup;
		public CreateOrganization createOrganization;

		public EventListView joinedEvents;
		public EventListView trackedEvents;

		public EventListView exploreEvents;
		public EventListView friendsEvents;

		public YourNotifications conversatios;
		public YourNotifications notifications;

		public List<ConversationView> activeConversationViews = new List<ConversationView>();

		int lastCoreView = 2;

		public string token = "";

		public CoreView ()
		{
			InitializeComponent ();
			_dataManager = new DataManager ();
			topBar = new UpperBar();
			topBarLayout.Children.Add (topBar);
			topBar.hideAll();
		}

		public async void startCoreView()
		{
			createEvent = new CreateEvent(new Event(), true);
			createGroup = new CreateGroup(new Group(), true);
			createOrganization = new CreateOrganization(new Organization(), true);

			joinedEvents = new EventListView(2);
			trackedEvents = new EventListView(3);

			exploreEvents = new EventListView(0);
			friendsEvents = new EventListView(1);

			conversatios = new YourNotifications(1);
			notifications = new YourNotifications(0);


			createView = new CarouselList(
				new List<VisualElement>() {createEvent, createGroup, createOrganization },
				new List<string>() { "Event", "Group", "Organization"}
			);
			manageEventView = new CarouselList(
				new List<VisualElement>() { joinedEvents, trackedEvents },
				new List<string>() { "Manage", "Follow" }
			);
			howlsView = new CarouselList(
				new List<VisualElement>() { conversatios, notifications },
				new List<string>() { "Conversations", "Notifications" }
			);

			homeView = new HomeView();

			searchEventView = new CarouselList(
				new List<VisualElement>() { exploreEvents, friendsEvents },
				new List<string>() { "Explore", "Friends" }
			);
			contentViews.Add (searchEventView);
			scrollViews.Add (null);
			contentViewTypes.Add ("Event");
			topBar.setNavigationLabel("Event", null);
			topBar.showFilterSearchButton(true);
			mainView.Content = searchEventView;
			await _dataManager.update();
			loading.IsVisible = false;

			if(token.Length > 15) displayAlertMessage("token", token, "ok");
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard(); 			GetLoggedInProfile();
			var first = DateTime.Now;
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			ContentView view = null;
			ScrollView scroll = null;



			//System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");
			if (type == 0)
			{
				view = createView;
				//topBar.showCreateNewGroupButton(true);
				topBar.setNavigationLabel("Create", null);
				createView.setLastCarousel();
			} else if (type == 1)
			{
				view = manageEventView;
				scroll = null;
				topBar.setNavigationLabel("Your Events", scroll);
				manageEventView.setLastCarousel();

			} else if (type == 2)
			{
				view = searchEventView;
				scroll = null;
				topBar.showFilterSearchButton(true);
				topBar.setNavigationLabel("Find Events", scroll);
				searchEventView.setLastCarousel();

			} else if (type == 3)
			{
				view = howlsView;
				scroll = null;
				topBar.setNavigationLabel("Howls", scroll);
				howlsView.setLastCarousel();
				topBar.showNewConversationButton(true);

			} else if (type == 4)
			{
				view = homeView;
				scroll = homeView.getScrollView();
				topBar.setNavigationLabel("Me", scroll);
			}

			lastCoreView = type;

			/*
			if (type == 1) {
				view = new EventView();
			} else if (type == 2) {
				view = new HomeView();
			}
			*/
			activeConversationViews.Clear();
			scrollViews.Clear();
			contentViews.Clear ();
			contentViewTypes.Clear ();
			contentViews.Add (view);
			scrollViews.Add(scroll);

			mainView.Content = view;
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");

		}

		public async void setContentViewWithQueue (ContentView view, string type, ScrollView s)
		{
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			topBar.hideAll();
			GetLoggedInProfile();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();

			System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");

			contentViews.Add (view);
			scrollViews.Add(s);
			contentViewTypes.Add (type);

			topBar.showBackButton (true);

			topBar.setNavigationLabel(type, s);
			mainView.Content = view;
			//await ViewExtensions.ScaleTo(view, 1, 200);
		}

		public async void setContentViewReplaceCurrent(ContentView view, string type, ScrollView s, int amount)
		{
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			topBar.hideAll();
			GetLoggedInProfile();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();

			System.Diagnostics.Debug.WriteLine(view.ToString() + " , the new view");

			for (int i = 0; i < amount; i++)
			{
				contentViews.Remove(contentViews[contentViews.Count - (1+i)]);
				scrollViews.Remove(scrollViews[scrollViews.Count - (1-i)]);
				contentViewTypes.Remove(contentViewTypes[contentViewTypes.Count - (1-i)]);
			}


			contentViews.Add(view);
			scrollViews.Add(s);
			contentViewTypes.Add(type);

			topBar.showBackButton(true);

			topBar.setNavigationLabel(type, s);
			mainView.Content = view;
			//await ViewExtensions.ScaleTo(view, 1, 200);
		}

		public void addConversationViewToActiveList(ConversationView convView)
		{
			activeConversationViews.Add(convView);
		}

		public void returnToPreviousView()
		{
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard(); 			System.Diagnostics.Debug.WriteLine ("contentViews.Count: " + contentViews.Count);
			int count = contentViews.Count;
			if (count != 1) {

				contentViews.RemoveAt (contentViews.Count - 1);
				contentViewTypes.RemoveAt (contentViewTypes.Count - 1);
				scrollViews.RemoveAt(scrollViews.Count - 1);

				ContentView oldView = contentViews [contentViews.Count - 1];
				ScrollView oldScroll = scrollViews[scrollViews.Count - 1];

				if (count > 2) {
				
					App.coreView.setContentViewWithQueue (oldView, "", oldScroll);

					contentViews.RemoveAt (contentViews.Count - 1);
					contentViewTypes.RemoveAt (contentViewTypes.Count - 1);
					scrollViews.RemoveAt(scrollViews.Count - 1);

				} else if (count == 2) {
					App.coreView.setContentView(lastCoreView);
					//TODO
					/*
					if (oldView.ToString () == new EventView ().ToString ()) {
						App.coreView.setContentView (1);
					} else if (oldView.ToString () == new HomeView ().ToString ()) {
						App.coreView.setContentView (2);
					} 
					*/

				}
			}
		}

		public void IsLoading(bool show)
		{
			loading.IsVisible = show;
		}

		protected override bool OnBackButtonPressed()
		{
			returnToPreviousView ();

			return true;
		}


		public async Task<Profile> GetLoggedInProfile()
		{
			return App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile();
		}

		public async Task GoToSelectedEvent(string eveID)
		{
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			InspectController inspect = new InspectController(eve);
			setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
		}

		public async Task GoToSelectedGroup(string groupID)
		{
			Group grp = await _dataManager.GroupApiManager.GetGroup(groupID);
			InspectController inspect = new InspectController(grp);
			setContentViewWithQueue(inspect, "Group", inspect.getScrollView());
		}

		public async Task GoToSelectedOrganization(string id)
		{
			Organization org = await _dataManager.OrganizationApiManager.GetOrganization(id);
			InspectController inspect = new InspectController(org);
			setContentViewWithQueue(inspect, "Organization", inspect.getScrollView());
		}

		public async Task displayAlertMessage (string title, string message, string buttonText)
		{
			optionOne.IsVisible = false;
			optionTwo.IsVisible = false;
			optionsBorder.IsVisible = false;
			optionOK.IsVisible = true;

			WarningTitle.Text = title;
			WarningDescription.Text = message;
			optionOK.Text = buttonText;



			await DisplayAlert();
		}

		public async Task<bool> displayConfirmMessage (string title, string message, string confirm, string decline)
		{
			optionOne.IsVisible = true;
			optionTwo.IsVisible = true;
			optionsBorder.IsVisible = true;
			optionOK.IsVisible = false;

			WarningTitle.Text = title;
			WarningDescription.Text = message;
			optionOne.Text = decline;
			optionTwo.Text = confirm;

			var answer = await DisplayAlert();
			return answer;
		}

		async Task<bool> DisplayAlert()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			bool answer = false;
			WarningLayout.IsVisible = true;



			optionOne.Clicked += (sender, e) =>
			{
				answer = false;
				tcs.TrySetResult(true);
			};
			optionTwo.Clicked += (sender, e) =>
			{
				answer = true;
				tcs.TrySetResult(true);
			};
			optionOK.Clicked += (sender, e) =>
			{
				answer = true;
				tcs.TrySetResult(true);
			};


			await tcs.Task;
			WarningLayout.IsVisible = false;
			return answer;
		}



	}
}

