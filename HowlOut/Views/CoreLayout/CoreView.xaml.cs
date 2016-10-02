using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		public List <ContentView> contentViews = new List<ContentView> ();
		public List <ScrollView> scrollViews = new List<ScrollView>();
		public List <string> contentViewTypes = new List<string> ();

		public UpperBar topBar;
		DataManager _dataManager;


		public CreateEvent createEventView;
		public EventView manageEventView;
		public EventView exploreEventView;
		public YourNotifications howlsEventView;
		public HomeView homeView;

		int lastCoreView = 2;

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
			createEventView = new CreateEvent(new Event(), true);
			manageEventView = new EventView(0);
			howlsEventView = new YourNotifications();
			homeView = new HomeView();
			exploreEventView = new EventView(1);
			contentViews.Add (exploreEventView);
			scrollViews.Add (null);
			contentViewTypes.Add ("Event");
			topBar.setNavigationLabel("Event", null);
			mainView.Content = exploreEventView;
			await _dataManager.update();
			loading.IsVisible = false;
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard(); 
			var first = DateTime.Now;
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			ContentView view = null;
			ScrollView scroll = null;

			//System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");
			if (type == 0)
			{
				view = createEventView;
				topBar.showCreateNewGroupButton(true);
				topBar.setNavigationLabel("Create Event", null);
			} else if (type == 1)
			{
				view = manageEventView;
				scroll = null;
				topBar.setNavigationLabel("Your Events", scroll);

			} else if (type == 2)
			{
				view = exploreEventView;
				scroll = null;
				topBar.showFilterSearchButton(true);
				topBar.setNavigationLabel("Find Events", scroll);

			} else if (type == 3)
			{
				view = howlsEventView;
				scroll = howlsEventView.getScrollView();
				topBar.setNavigationLabel("Howls", scroll);

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
			scrollViews.Clear();
			contentViews.Clear ();
			contentViewTypes.Clear ();
			contentViews.Add (view);
			scrollViews.Add(scroll);

			//await ViewExtensions.ScaleTo(view, 0, 0);
			mainView.Content = view;
			//await ViewExtensions.ScaleTo(view, 1, 200);
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");

		}

		public async void setContentViewWithQueue (ContentView view, string type, ScrollView s)
		{
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();

			System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");

			contentViews.Add (view);
			scrollViews.Add(s);
			contentViewTypes.Add (type);

			topBar.showBackButton (true);

			topBar.setNavigationLabel(type, s);

			//await ViewExtensions.ScaleTo(view, 0, 0);
			mainView.Content = view;
			//await ViewExtensions.ScaleTo(view, 1, 200);
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

		protected override bool OnBackButtonPressed()
		{
			returnToPreviousView ();

			return true;
		}


		public async void GoToSelectedEvent(string eveID)
		{
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			InspectController inspect = new InspectController(null, null, eve);
			setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
		}

		public async void GoToSelectedGroup(string groupID)
		{
			Group grp = await _dataManager.GroupApiManager.GetGroupById(groupID);
			InspectController inspect = new InspectController(null, grp, null);
			setContentViewWithQueue(inspect, "Group", inspect.getScrollView());
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

