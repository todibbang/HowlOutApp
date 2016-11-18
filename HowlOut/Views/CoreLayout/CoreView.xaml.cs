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

		public ConversationView viewdConversation;

		public UpperBar topBar;
		public DataManager _dataManager;
		public OtherFunctions otherFunctions = new OtherFunctions();

		public CarouselList createView;
		public CarouselList searchEventView;
		public InspectController homeView;
		public YourConversations conversatios;
		public YourNotifications notifications;

		public CreateEvent createEvent;
		public CreateGroup createGroup;
		public CreateOrganization createOrganization;

		public EventListView joinedEvents;
		public EventListView trackedEvents;

		public EventListView exploreEvents;
		public EventListView friendsEvents;



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
			notifications = new YourNotifications();
			conversatios = new YourConversations(ConversationModelType.Profile, App.StoredUserFacebookId);
			createEvent = new CreateEvent(new Event(), true);
			createGroup = new CreateGroup(new Group(), true);
			createOrganization = new CreateOrganization(new Organization(), true);
			exploreEvents = new EventListView(0);
			joinedEvents = new EventListView(2);
			trackedEvents = new EventListView(3);

			setContentView(2);
			await _dataManager.update();
			loading.IsVisible = false;

			if(token.Length > 15) displayAlertMessage("token", token, "ok");
		}

		public async void updateHomeView()
		{
			await App.coreView.GetLoggedInProfile();
			homeView = new InspectController(App.userProfile);
		}

		public async void updateCreateViews()
		{
			createView = new CarouselList(
				new List<VisualElement>() { createEvent, createGroup, createOrganization },
				new List<string>() { "Event", "Group", "Organization" }
			);
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			GetLoggedInProfile();
			var first = DateTime.Now;
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			ContentView view = null;
			ScrollView scroll = null;
			viewdConversation = null;


			if (type == 0)
			{
				if (createView == null)
				{
					createView = new CarouselList(
						new List<VisualElement>() { createEvent, createGroup, createOrganization },
						new List<string>() { "Event", "Group", "Organization" }
					);
				}
				view = createView;
				topBar.setNavigationLabel("Create", null);
				createView.setLastCarousel();
			} else if (type == 1)
			{
				if (conversatios == null)
				{
					conversatios = new YourConversations(ConversationModelType.Profile, App.StoredUserFacebookId);
				}
				view = conversatios;
				scroll = null;
				topBar.setNavigationLabel("Conversations", scroll);
				App.coreView.topBar.showNewConversationButton(true, conversatios);

			} else if (type == 2)
			{
				if (searchEventView == null)
				{
					searchEventView = new CarouselList(
						new List<VisualElement>() { joinedEvents, exploreEvents, trackedEvents },
						new List<string>() { "Joined", "Explore", "Followed" }
					);
				}
				view = searchEventView;
				scroll = null;
				topBar.showFilterSearchButton(true);
				topBar.setNavigationLabel("Find Events", scroll);
				searchEventView.setLastCarousel();

			} else if (type == 3)
			{
				if (notifications == null)
				{
					notifications = new YourNotifications();
				}
				view = notifications;
				scroll = null;
				topBar.setNavigationLabel("Howls", scroll);


			} else if (type == 4)
			{
				if (homeView == null)
				{
					homeView = new InspectController(App.userProfile);
				}
				view = homeView;
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

			topBar.showBackButton (true);

			//topBar.setNavigationLabel(type, s);
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
				contentViews.Remove(contentViews[contentViews.Count - (1)]);
				scrollViews.Remove(scrollViews[scrollViews.Count - (1)]);
			}


			contentViews.Add(view);
			scrollViews.Add(s);

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
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			System.Diagnostics.Debug.WriteLine ("contentViews.Count: " + contentViews.Count);
			int count = contentViews.Count;
			if (count != 1) {

				contentViews.RemoveAt (contentViews.Count - 1);
				scrollViews.RemoveAt(scrollViews.Count - 1);

				ContentView oldView = contentViews [contentViews.Count - 1];
				ScrollView oldScroll = scrollViews[scrollViews.Count - 1];

				if (count > 2) {
				
					App.coreView.setContentViewWithQueue (oldView, "", oldScroll);

					contentViews.RemoveAt (contentViews.Count - 1);
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

		public async Task GoToSelectedProfile(string id)
		{
			Profile pro = await _dataManager.ProfileApiManager.GetProfile(id);
			InspectController inspect = new InspectController(pro);
			setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
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

		public async Task GoToSelectedConversation(string id)
		{
			Conversation con = await _dataManager.MessageApiManager.GetOneConversation(id);
			ConversationView conV = new ConversationView(con);
			setContentViewWithQueue(conV, "Conversation", null);
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

		public void setConversationsNoti(int i)
		{
			if (i > 0)
			{
				bottomBar.conversationNoti.IsVisible = true;
				bottomBar.conversationNoti.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.conversationNoti.IsVisible = true;
				bottomBar.conversationNoti.Text = int.Parse(bottomBar.conversationNoti.Text) + 1 + "";
			}
			else{
				bottomBar.conversationNoti.IsVisible = false;
				bottomBar.conversationNoti.Text = 0 + "";
			}
		}

		public void setEventsNoti(int i)
		{
			if (i > 0)
			{
				bottomBar.eventNoti.IsVisible = true;
				bottomBar.eventNoti.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.eventNoti.IsVisible = true;
				bottomBar.eventNoti.Text = int.Parse(bottomBar.eventNoti.Text) + 1 + "";
			}
			else {
				bottomBar.eventNoti.IsVisible = false;
				bottomBar.eventNoti.Text = 0 + "";
			}
		}

		public void setHowlsNoti(int i)
		{
			if (i > 0)
			{
				bottomBar.howlsNoti.IsVisible = true;
				bottomBar.howlsNoti.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.howlsNoti.IsVisible = true;
				bottomBar.howlsNoti.Text = int.Parse(bottomBar.howlsNoti.Text) + 1 + "";
			}
			else {
				bottomBar.howlsNoti.IsVisible = false;
				bottomBar.howlsNoti.Text = 0 + "";
			}
		}

		public void setHomeNoti(int i)
		{
			if (i > 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = int.Parse(bottomBar.homeNoti.Text) + 1 + "";
			}
			else {
				bottomBar.homeNoti.IsVisible = false;
				bottomBar.homeNoti.Text = 0 + "";
			}
		}
	}
}

