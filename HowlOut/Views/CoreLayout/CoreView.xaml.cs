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
		public List <ViewModelInterface> contentViews = new List<ViewModelInterface> ();

		public StackLayout TopBarLayout { get { return topBarLayout; } }
		public Grid MainGrid { get { return mainGrid; } }

		bool hideNotiLayout = false;

		public ConversationView viewdConversation;

		ContentView newContentView;

		public UpperBar topBar;
		public DataManager _dataManager;
		public OtherFunctions otherFunctions = new OtherFunctions();

		public CarouselList createView;
		public CarouselList searchEventView;
		public CarouselList joinedEventView;
		public CarouselList conversatios;
		public CarouselList myProfile;
		public InspectController homeView;
		public YourConversations profileConversatios;
		public YourConversations otherConversatios;
		public YourNotifications notifications;

		public CreateEvent createEvent;
		public CreateGroup createGroup;
		//public CreateOrganization createOrganization;

		public ExploreEventCategories exploreEventCategories;

		public EventListView joinedEvents;
		public EventListView endedEvents;

		public EventListView trackedEvents;

		public EventListView exploreEvents;
		public EventListView friendsEvents;

		public bool topBarHidden;

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

 			notiScroll.Scrolled += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine(notiScroll.ScrollY);
				if (notiScroll.ScrollY > 5 && !hideNotiLayout)
				{
					notiScroll.ScrollToAsync(0, 1000, true);
					if (!hideNotiLayout) HideNotiLayout();
					hideNotiLayout = true;
				}
			};
		}

		public async void HideNotiLayout()
		{
			while (notiLayout.TranslationY > -100)
			{
				notiLayout.TranslationY -= 2;
				await Task.Delay(4);
			}
			notiLayout.IsVisible = false;
		}

		async void hideTopBar(int d)
		{
			/*
			if (d == -1) topBarHidden = true;
			else topBarHidden = false;
			await Task.Delay(500);
			for (int i = 0; i < 50; i++)
			{
				App.coreView.TopBarLayout.TranslationY += 2 * d;
				App.coreView.MainGrid.TranslationY += 1 * d;
				App.coreView.MainGrid.HeightRequest += -1 * d;
				await Task.Delay(5);
			}
			*/
		}

		public async void ShowNotification(Action a, string s)
		{
			notiContent.Children.Clear();
			Button b = new Button()
			{
				Text = s,
				BackgroundColor = Color.FromHex("#90000000"),
				HeightRequest = 51,
				TextColor = Color.White
			};


			b.Clicked += (sender, e) =>
			{
				HideNotiLayout();
				a.Invoke();
			};


			notiContent.Children.Add(b);

			notiLayout.IsVisible = true;



			notiScroll.ScrollToAsync(b, ScrollToPosition.Start, false);
			while (notiLayout.TranslationY < 0)
			{
				notiLayout.TranslationY += 2;
				await Task.Delay(4);
			}
			hideNotiLayout = false;
			int waiter = 0;
			while (waiter < 100)
			{
				await Task.Delay(100);
				waiter++;
				if (hideNotiLayout)
				{
					break;
				}
			}

			hideNotiLayout = true;
			HideNotiLayout();
		}

		public async void startCoreView()
		{
			notifications = new YourNotifications();
			homeView = new InspectController(App.userProfile);

			profileConversatios = new YourConversations(ConversationModelType.Profile, App.StoredUserFacebookId, 1);
			otherConversatios = new YourConversations(ConversationModelType.Profile, App.StoredUserFacebookId, 2);
			createEvent = new CreateEvent(new Event(), true);
			createGroup = new CreateGroup(new Group(), true);
			//createOrganization = new CreateOrganization(new Organization(), true);

			exploreEventCategories = new ExploreEventCategories();
			exploreEvents = new EventListView(0);
			joinedEvents = new EventListView(2);
			endedEvents = new EventListView(10);
			trackedEvents = new EventListView(3);
			friendsEvents = new EventListView(1);

			createView = new CarouselList(
				new List<VisualElement>() { createEvent, createGroup },
				new List<string>() { "Event", "Group" },
				CarouselList.ViewType.Create
			);

			conversatios = new CarouselList(
				new List<VisualElement>() { profileConversatios, otherConversatios },
				new List<string>() { "Yours", "Others" },
				CarouselList.ViewType.Conversations
			);

			searchEventView = new CarouselList(
				new List<VisualElement>() { exploreEventCategories, exploreEvents, friendsEvents, trackedEvents },
				new List<string>() { "Explore", "Explore", "Friends", "Followed" },
				CarouselList.ViewType.SearchEvents
			);

			joinedEventView = new CarouselList(
				new List<VisualElement>() { joinedEvents, endedEvents},
				new List<string>() { "Joined", "Old"},
				CarouselList.ViewType.JoinedEvents
			);

			myProfile = new CarouselList(
				new List<VisualElement>() { homeView, notifications },
				new List<string>() { "Me", "Notifications" },
				CarouselList.ViewType.Home
			);


			setContentView(2);
			await _dataManager.update();
			loading.IsVisible = false;

			if(token.Length > 15) displayAlertMessage("token", token, "ok");
		}

		public async Task updateHomeView()
		{
			App.userProfile = await GetLoggedInProfile();
			homeView = new InspectController(App.userProfile);
			myProfile = new CarouselList(
				new List<VisualElement>() { homeView, notifications },
				new List<string>() { "Me", "Notifications" },
				CarouselList.ViewType.Home
			);
		}

		public void updateCreateViews()
		{
			createView = new CarouselList(
				new List<VisualElement>() { createEvent, createGroup },
				new List<string>() { "Event", "Group" },
				CarouselList.ViewType.Create
			);
		}

		void changeView(ViewModelInterface view)
		{
			ContentView oldContentView = newContentView;
			newContentView = new ContentView() { Content = view.getContentView(), BackgroundColor = App.HowlOutBackground };
			mainGrid.Children.Add(newContentView);

			//newContentView.TranslationX = this.Width;
			//await newContentView.TranslateTo(0, 0, 400, null);
			//view.viewInFocus(topBar);
			if(oldContentView != null) mainGrid.Children.Remove(oldContentView);
			oldContentView = null;
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();
			if(contentViews.Count > 0) contentViews[contentViews.Count-1].viewExitFocus();
			//await Task.Delay(2);
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			ViewModelInterface view = null;
			viewdConversation = null;

			updateHomeView();

			if (type == 0) { view = createView; 
			} else if (type == 1) {  view = joinedEventView;
			} else if (type == 2) {  view = searchEventView; 
			} else if (type == 3) { view = conversatios; 
			} else if (type == 4) { 
				//await updateHomeView();
				view = myProfile; 
			}

			if ((type == 1 || type == 2))
			{
				if (!topBarHidden)
					hideTopBar(-1);
			}
			else {
				if (topBarHidden)
					hideTopBar(1);
			}

			lastCoreView = type;


			activeConversationViews.Clear();
			contentViews.Clear ();
			contentViews.Add (view);
			//changeView(view);
			mainView.Content = view.getContentView();
			view.viewInFocus(topBar);
		}

		public async void setContentViewWithQueue(ViewModelInterface view)
		{
			topBar.hideAll();
			if (topBarHidden)
				hideTopBar(1);
			if (contentViews.Count > 0) contentViews[contentViews.Count - 1].viewExitFocus();
			//await Task.Delay(2);
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			contentViews.Add (view);
			topBar.showBackButton (true);
			//changeView(view);
			mainView.Content = view.getContentView();
			view.viewInFocus(topBar);
		}

		public async void setContentViewReplaceCurrent(ViewModelInterface view, int amount)
		{
			topBar.hideAll();
			if (topBarHidden)
				hideTopBar(1);
			if (contentViews.Count > 0) contentViews[contentViews.Count - 1].viewExitFocus();
			//await Task.Delay(2);
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			contentViews.RemoveRange(contentViews.Count - (1 + amount), amount);
			contentViews.Add(view);
			if(contentViews.Count > 1) topBar.showBackButton(true);
			//changeView(view);
			mainView.Content = view.getContentView();
			view.viewInFocus(topBar);
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
			if (count != 1) 
			{
				try
				{
					contentViews.RemoveAt(contentViews.Count - 1);
					ViewModelInterface oldView = contentViews[contentViews.Count - 1];
					if (count > 2)
					{
						App.coreView.setContentViewWithQueue(oldView);
						contentViews.RemoveAt(contentViews.Count - 1);
					}
					else if (count == 2)
					{
						App.coreView.setContentView(lastCoreView);
					}
				}
				catch (Exception e) {}
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
			return await _dataManager.ProfileApiManager.GetLoggedInProfile();
		}

		public async Task GoToSelectedProfile(string id)
		{
			IsLoading(true);
			Profile pro = await _dataManager.ProfileApiManager.GetProfile(id);
			InspectController inspect = new InspectController(pro);
			setContentViewWithQueue(inspect);
			IsLoading(false);
		}

		public async Task GoToSelectedEvent(string eveID)
		{
			IsLoading(true);
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			InspectController inspect = new InspectController(eve);
			setContentViewWithQueue(inspect);
			IsLoading(false);
		}

		public async Task GoToSelectedGroup(string groupID)
		{
			IsLoading(true);
			Group grp = await _dataManager.GroupApiManager.GetGroup(groupID);
			InspectController inspect = new InspectController(grp);
			setContentViewWithQueue(inspect);
			IsLoading(false);
		}

		/*
		public async Task GoToSelectedOrganization(string id)
		{
			IsLoading(true);
			Organization org = await _dataManager.OrganizationApiManager.GetOrganization(id);
			InspectController inspect = new InspectController(org);
			setContentViewWithQueue(inspect);
			IsLoading(false);
		} */

		public async Task GoToSelectedConversation(string id)
		{
			IsLoading(true);
			Conversation con = await _dataManager.MessageApiManager.GetOneConversation(id);
			ConversationView conV = new ConversationView(con);
			setContentViewWithQueue(conV);
			IsLoading(false);
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

			closeWarning.Clicked += (sender, e) =>
			{
				answer = false;
				tcs.TrySetResult(true);
			};

			await tcs.Task;
			WarningLayout.IsVisible = false;
			return answer;
		}




		public async Task<bool> DisplayShare(Event eve)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			bool answer = false;
			ShareLayout.IsVisible = true;


			closeShare.Clicked += (sender, e) =>
			{
				tcs.TrySetResult(true);
			};

			CalenderBtn.Clicked += async (sender, e) =>
			{
				ShareLayout.IsVisible = false;
				bool success = await App.coreView.displayConfirmMessage("Add Event To Calendar", "Would you like to add this event to your calendar ?", "yes", "no");
				if (success)
				{
					success = await DependencyService.Get<SocialController>().addEventToCalendar(eve);
					if (!success) await App.coreView.displayAlertMessage("Error", "An error occured and event was not added to calendar", "ok");
				}
			};
			await tcs.Task;
			ShareLayout.IsVisible = false;
			return answer;
		}

		public async Task<bool> DisplayOptions(List<Action> actions, List<string> titles, List<string> imageSources)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			bool answer = false;
			optionLayout.IsVisible = true;
			List<Button> buttons = new List<Button>();
			optionLayoutList.HeightRequest = 0;
			for (int i = 0; i < titles.Count; i++)
			{
				Grid g = new Grid() {HeightRequest = 40 };
				StackLayout s = new StackLayout() {Orientation = StackOrientation.Horizontal };

				s.Children.Add(new IconView() { Source = imageSources[i], Foreground = App.HowlOut, HeightRequest=20, WidthRequest=20, VerticalOptions = LayoutOptions.Center });
				s.Children.Add(new Label() { Text = titles[i], TextColor = App.NormalTextColor, VerticalOptions = LayoutOptions.Center });
				g.Children.Add(s);
				Button b = new Button() { };
				buttons.Add(b);
				g.Children.Add(b);

				optionLayoutList.Children.Add(g);

				optionLayoutList.HeightRequest += 40;
			}


			foreach (Button b in buttons)
			{
				b.Clicked += (sender, e) =>
				{
					if (actions != null && actions[buttons.IndexOf(b)] != null) { actions[buttons.IndexOf(b)].Invoke(); }
					tcs.TrySetResult(true);
				};
			}



			/*
			for (int i = 0; i < actions.Count; i++)
			{
				Button b = new Button() {BackgroundColor=Color.White, Text = titles[i] };
				b.Clicked += (sender, e) =>
				{
					actions[i].Invoke();
				};
				optionLayoutList.Children.Add(b);
			} */

			closeOptions.Clicked += (sender, e) =>
			{
				tcs.TrySetResult(true);
			};

			await tcs.Task;
			optionLayout.IsVisible = false;
			optionLayoutList.Children.Clear();
			return answer;
		}

		/*
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
		*/

		public void setHowlsNoti(int i)
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
			else {
				bottomBar.conversationNoti.IsVisible = false;
				bottomBar.conversationNoti.Text = 0 + "";
			}
		}

		/*
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
		*/
	}
}

