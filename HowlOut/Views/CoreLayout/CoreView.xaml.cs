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
		public List<ViewModelInterface> contentViews = new List<ViewModelInterface>();
		public Grid TopBarLayout { get { return topBarLayout; } }
		public Grid MainGrid { get { return mainGrid; } }
		bool hideNotiLayout = false;
		public ConversationView viewdConversation;
		ContentView newContentView;
		public UpperBar topBar;
		public BottomBar btmBar { get { return bottomBar; } }
		public DataManager _dataManager;
		public OtherFunctions otherFunctions = new OtherFunctions();
		public YourNotifications notifications;
		public InspectController homeView;
		public YourConversations yourConversatios;
		public CreateView createView;
		//public ExploreEventCategories exploreEventCategories;
		public EventListView exploreEvents;
		public EventListView joinedEvents;
		public Button notiButton = new Button() { BackgroundColor = Color.FromHex("#ffc65539"), BorderRadius = 8, FontSize = 10, TextColor = Color.White, Text = "0" };
		public bool topBarHidden;
		public bool topBarHiddenGlobalCooldown = false;
		public double viewLastChanged = DateTime.Now.Ticks;
		public List<ConversationView> activeConversationViews = new List<ConversationView>();
		public int lastCoreView = 2;
		public ViewModelInterface lastCoreViewInterface;
		public string token = "";

		public CoreView()
		{
			InitializeComponent();
			_dataManager = new DataManager();
			topBar = new UpperBar();
			topBarLayout.Children.Add(topBar);
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

		public async void hideTopBar(bool hide)
		{



			if (topBarHiddenGlobalCooldown)
			{
				return;
			}


			if (hide && App.coreView.TopBarLayout.TranslationY < 0) return; 
			if (!hide && App.coreView.TopBarLayout.TranslationY  > -1) return;
			topBarHiddenGlobalCooldown = true;

			int d = 1;
			topBarHidden = false;
			if (hide)
			{
				d = -1;
				topBarHidden = true;
			}

			for (int i = 0; i < 60; i++)
			{
				App.coreView.TopBarLayout.TranslationY += 2 * d;
				//App.coreView.MainGrid.TranslationY += 1 * d;
				//App.coreView.MainGrid.HeightRequest += -1 * d;

				//App.coreView.extraGrid.TranslationY += 1 * d;
				//App.coreView.extraGrid.HeightRequest += -1 * d;
				await Task.Delay(5);

				if (App.coreView.TopBarLayout.TranslationY <= -120)
				{
					App.coreView.TopBarLayout.TranslationY = -120;
					break;
				} 
				if (App.coreView.TopBarLayout.TranslationY > 0)
				{
					App.coreView.TopBarLayout.TranslationY = 0;
					break;
				}
			}
			topBarHiddenGlobalCooldown = false;
		}

		public async void ShowNotification(Action a, string s)
		{
			notiContent.Children.Clear();
			Grid g = new Grid();
			StackLayout sl = new StackLayout() { Padding = new Thickness(10, 0, 10, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
			sl.Children.Add(new Label() { TextColor = Color.White, Text = s, VerticalOptions = LayoutOptions.CenterAndExpand });
			g.Children.Add(new Button() { HeightRequest = 51, BackgroundColor = Color.FromHex("#cc000000"), BorderRadius = 5, });
			g.Children.Add(sl);
			Button b = new Button();
			g.Children.Add(b);
			b.Clicked += (sender, e) =>
			{
				HideNotiLayout();
				a.Invoke();
			};
			notiContent.Children.Add(g);
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
			await _dataManager.UtilityManager.getGeoLocation();
			homeView = new InspectController(App.userProfile);
			yourConversatios = new YourConversations(ConversationModelType.Profile, App.StoredUserFacebookId, 1);
			//exploreEventCategories = new ExploreEventCategories();
			exploreEvents = new EventListView(0);
			joinedEvents = new EventListView(2);
			notifications = new YourNotifications();
			createView = new CreateView();
			mainView.Content = joinedEvents;
			joinedEvents.viewInFocus(topBar);
			IsLoading(false);
			await _dataManager.update();
		}

		public async Task updateMainViews(int i)
		{
			if (i == 0)
			{
				createView.reloadView();
			}
			else if (i == 1)
			{
				joinedEvents.UpdateList(true, "");
			}
			else if (i == 2)
			{

			}
			else if (i == 3)
			{
				yourConversatios.UpdateConversations(true);
			}
			else if (i == 4)
			{
				homeView.reloadView();
			}
		}

		void changeView(ViewModelInterface view)
		{
			ContentView oldContentView = newContentView;
			newContentView = new ContentView() { Content = view.getContentView(), BackgroundColor = App.HowlOutBackground };
			mainGrid.Children.Add(newContentView);
			if (oldContentView != null) mainGrid.Children.Remove(oldContentView);
			oldContentView = null;
		}

		public async void setContentView(int type)
		{
			int timeDif = (int)((DateTime.Now.Ticks - viewLastChanged) / 10000);
			if (timeDif < 1000)
			{
				await Task.Delay(1000 - timeDif);
			}
			viewLastChanged = DateTime.Now.Ticks;
			extraGrid.IsVisible = false;
			extraView.Content = null;
			topBar.hideAll();
			hideTopBar(false);
			if (contentViews.Count > 0) contentViews[contentViews.Count - 1].viewExitFocus();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			ViewModelInterface view = null;
			viewdConversation = null;

			if (type == 0)
			{
				view = createView;
			}
			else if (type == 1)
			{
				view = exploreEvents;
			}
			else if (type == 2)
			{
				view = joinedEvents;
			}
			else if (type == 3)
			{
				view = yourConversatios;
			}
			else if (type == 4) { view = homeView; }

			lastCoreView = type;
			bottomBar.selectButton(type);
			activeConversationViews.Clear();
			contentViews.Clear();
			lastCoreViewInterface = view;
			mainView.Content = view.getContentView();
			view.viewInFocus(topBar);
		}

		public async void setContentViewWithQueue(ViewModelInterface view)
		{
			if (contentViews.Count > 0) contentViews[contentViews.Count - 1].viewExitFocus();
			else {
				if (lastCoreView == 0) { createView.viewExitFocus(); }
				else if (lastCoreView == 1) { joinedEvents.viewExitFocus(); }
				else if (lastCoreView == 2) { exploreEvents.viewExitFocus(); }
				else if (lastCoreView == 3) { yourConversatios.viewExitFocus(); }
				else if (lastCoreView == 4) { homeView.viewExitFocus(); }
			}
			extraGrid.IsVisible = true;
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			contentViews.Add(view);
			topBar.showBackButton(true);
			extraView.Content = view.getContentView();
			view.viewInFocus(topBar);
		}

		public async void setContentViewReplaceCurrent(ViewModelInterface view, int amount)
		{
			extraGrid.IsVisible = true;
			topBar.hideAll();
			if (contentViews.Count > 0) contentViews[contentViews.Count - 1].viewExitFocus();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			try
			{
				contentViews.RemoveRange(contentViews.Count - (1 + amount), amount);
			}
			catch (Exception ecx)
			{
				try
				{
					amount--;
					contentViews.RemoveRange(contentViews.Count - (1 + amount), amount);
				}
				catch (Exception exc2)
				{

				}
			}
			contentViews.Add(view);
			if (contentViews.Count > 1) topBar.showBackButton(true);
			extraView.Content = view.getContentView();
			view.viewInFocus(topBar);
		}

		public void addConversationViewToActiveList(ConversationView convView)
		{
			activeConversationViews.Add(convView);
		}

		public async void reloadCurrentView()
		{
			IsLoading(true);
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			if (contentViews.Count == 0)
			{

			}
			else {
				contentViews[contentViews.Count - 1].reloadView();
			}
			await updateMainViews(4);
			IsLoading(false);
		}

		public void returnToPreviousView()
		{
			try
			{
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				System.Diagnostics.Debug.WriteLine("contentViews.Count: " + contentViews.Count);
				bool specialView = false;
				if (contentViews[contentViews.Count - 1] is CreateGroup)
				{
					var group = contentViews[contentViews.Count - 1] as CreateGroup;
					if (!group.IsCreate && group.otherViews.IsVisible)
					{
						group.otherViews.Content = null;
						group.otherViews.IsVisible = false;
						specialView = true;
					}
				}

				else if (contentViews[contentViews.Count - 1] is CreateEvent)
				{
					var eve = contentViews[contentViews.Count - 1] as CreateEvent;
					if (!eve.IsCreate && eve.otherViews.IsVisible)
					{
						eve.otherViews.Content = null;
						eve.otherViews.IsVisible = false;
						specialView = true;
					}
				}
				/*
				else if (contentViews[contentViews.Count - 1] is CreateView)
				{
					var cv = (contentViews[contentViews.Count - 1]) as CreateView;
					bool cvReturned = cv.returnCreateView();
					if (cvReturned)
					{
						specialView = true;
					}
				} */
				else if (contentViews[contentViews.Count - 1] is WeShareOverView)
				{
					var expenShareView = contentViews[contentViews.Count - 1] as WeShareOverView;
					if (expenShareView.SecondGrid.IsVisible)
					{
						expenShareView.returnToFirstFrid();
						specialView = true;
					}
				}



				if (!specialView)
				{
					int count = contentViews.Count;
					if (count != 0)
					{
						try
						{
							contentViews[contentViews.Count - 1].viewExitFocus();
							if (count == 1)
							{
								extraGrid.IsVisible = false;
								extraView.Content = null;
								topBar.hideAll();
								contentViews = new List<ViewModelInterface>();
								DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
								lastCoreViewInterface.viewInFocus(topBar);
							}
							else {
								contentViews.RemoveAt(contentViews.Count - 1);
								ViewModelInterface oldView = contentViews[contentViews.Count - 1];

								if (oldView is FilterSearch)
								{
									oldView.reloadView();
								}

								if (count > 1)
								{
									App.coreView.setContentViewWithQueue(oldView);
									contentViews.RemoveAt(contentViews.Count - 1);
								}
							}
						}
						catch (Exception e) { }
					}
				}
			}
			catch (Exception exc) {}
		}

		public void IsLoading(bool show)
		{
			loading.IsVisible = show;
		}

		protected override bool OnBackButtonPressed()
		{
			returnToPreviousView();
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

		public async Task GoToSelectedConversation(string id)
		{
			IsLoading(true);
			Conversation con = await _dataManager.MessageApiManager.GetOneConversation(id);
			ConversationView conV = new ConversationView(con);
			setContentViewWithQueue(conV);
			IsLoading(false);
		}

		public async Task displayAlertMessage(string title, string message, string buttonText)
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

		public async Task<bool> displayConfirmMessage(string title, string message, string confirm, string decline)
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
			closeWarning.Clicked += (sender, e) =>
			{
				answer = false;
				tcs.TrySetResult(true);
			};
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

		public async Task<bool> DisplayShare(Event eve)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			bool answer = false;
			ShareLayout.IsVisible = true;
			inviteBtn = new Button() { WidthRequest = 60, BorderRadius = 30, BorderWidth = 2, BorderColor = App.HowlOut };
			shareGrid.Children.Add(inviteBtn, 1, 0);
			MailBtn = new Button() { WidthRequest = 60, BorderRadius = 30, BorderWidth = 2, BorderColor = Color.Red };
			shareGrid.Children.Add(MailBtn, 3, 0);
			SmsBtn = new Button() { WidthRequest = 60, BorderRadius = 30, BorderWidth = 2, BorderColor = Color.FromHex("#ffd8d32f") };
			shareGrid.Children.Add(SmsBtn, 5, 0);
			messengerShareButton = new FacebookSendButton() { WidthRequest = 60, HorizontalOptions = LayoutOptions.CenterAndExpand, Link = "https://api.howlout.net/redirect?type=event" + eve.EventId };
			shareGrid.Children.Add(messengerShareButton, 1, 2);
			facebookShareButton = new FacebookShareButton() { WidthRequest = 60, HorizontalOptions = LayoutOptions.CenterAndExpand, Link = "https://api.howlout.net/redirect?type=event" + eve.EventId };
			shareGrid.Children.Add(facebookShareButton, 3, 2);
			CalenderBtn = new Button() { WidthRequest = 60, BorderRadius = 30, BorderWidth = 2, BorderColor = Color.FromHex("#ffef9d0e") };
			shareGrid.Children.Add(CalenderBtn, 5, 2);
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
			MailBtn.Clicked += (sender, e) =>
			{
				Device.OpenUri(new Uri(String.Format("mailto:Replace_this_mail@mail.com?subject=" + App.userProfile.Name + " has invited you to a HowlOut Event&body=\n\n https://api.howlout.net/redirect?type=event" + eve.EventId)));
			};
			SmsBtn.Clicked += (sender, e) =>
			{
				Device.OpenUri(new Uri(String.Format("sms:&body=" + App.userProfile.Name + " has invited you to a HowlOut Event\n\napi.howlout.net/redirect?type=event" + eve.EventId)));
			};
			inviteBtn.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new InviteListView(eve, false));
				ShareLayout.IsVisible = false;
				tcs.TrySetResult(true);
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
			optionLayoutList.TranslationX = 250;
			optionLayoutList.TranslateTo(0, 0, 250, null);
			List<Button> buttons = new List<Button>();
			optionLayoutList.HeightRequest = 0;
			for (int i = 0; i < titles.Count; i++)
			{
				Grid g = new Grid() { HeightRequest = 50 };
				StackLayout s = new StackLayout() { Orientation = StackOrientation.Horizontal };
				s.Children.Add(new IconView() { Source = imageSources[i], Foreground = App.HowlOut, HeightRequest = 20, WidthRequest = 20, VerticalOptions = LayoutOptions.Center });
				s.Children.Add(new Label() { Text = titles[i], TextColor = App.NormalTextColor, VerticalOptions = LayoutOptions.Center });
				g.Children.Add(s);
				Button b = new Button() { };
				buttons.Add(b);
				g.Children.Add(b);
				optionLayoutList.Children.Add(g);
				optionLayoutList.HeightRequest += 50;
			}
			foreach (Button b in buttons)
			{
				b.Clicked += (sender, e) =>
				{
					if (actions != null && actions[buttons.IndexOf(b)] != null) { actions[buttons.IndexOf(b)].Invoke(); }
					tcs.TrySetResult(true);
				};
			}
			closeOptions.Clicked += (sender, e) =>
			{
				tcs.TrySetResult(true);
			};
			await tcs.Task;
			await optionLayoutList.TranslateTo(250, 0, 250, null);
			optionLayout.IsVisible = false;
			optionLayoutList.Children.Clear();
			return answer;
		}

		public void setHowlsNoti(int i)
		{
			if (i > 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = i + "";

				notiButton.IsVisible = true;
				notiButton.Text = i + "";
			}
			else if (i < 0)
			{
				bottomBar.homeNoti.IsVisible = true;
				bottomBar.homeNoti.Text = int.Parse(bottomBar.homeNoti.Text) + 1 + "";

				notiButton.IsVisible = true;
				notiButton.Text = int.Parse(bottomBar.homeNoti.Text) + 1 + "";
			}
			else {
				bottomBar.homeNoti.IsVisible = false;
				bottomBar.homeNoti.Text = 0 + "";

				notiButton.IsVisible = false;
				notiButton.Text = 0 + "";
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

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); //must be called
			if (width > height)
			{
				hideTopBar(true);
			}
			else {
				hideTopBar(false);
			}
		}
	}
}

