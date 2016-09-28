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

		public void startCoreView()
		{
			createEventView = new CreateEvent(new Event(), true);
			manageEventView = new EventView(0);
			exploreEventView = new EventView(1);
			howlsEventView = new YourNotifications();
			homeView = new HomeView();

			_dataManager.update ();

			contentViews.Add (manageEventView);
			contentViewTypes.Add ("Event");
			topBar.setNavigationLabel("Event");
			mainView.Content = manageEventView;
			loading.IsVisible = false;
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard(); 
			var first = DateTime.Now;
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			ContentView view = null;

			//System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");
			if (type == 0)
			{
				view = createEventView;
				topBar.showCreateNewGroupButton(true);
				topBar.setNavigationLabel("Create Event");
			} else if (type == 1)
			{
				view = manageEventView;
				topBar.setNavigationLabel("Your Events");
			} else if (type == 2)
			{
				view = exploreEventView;
				topBar.showFilterSearchButton(true);
				topBar.setNavigationLabel("Find Events");
			} else if (type == 3)
			{
				view = howlsEventView;
				topBar.setNavigationLabel("Howls");
			} else if (type == 4)
			{
				view = homeView;

				topBar.setNavigationLabel("Me");
			}

			lastCoreView = type;

			/*
			if (type == 1) {
				view = new EventView();
			} else if (type == 2) {
				view = new HomeView();
			}
			*/

			contentViews.Clear ();
			contentViewTypes.Clear ();
			contentViews.Add (view);


			//await ViewExtensions.ScaleTo(view, 0, 0);
			mainView.Content = view;
			//await ViewExtensions.ScaleTo(view, 1, 200);
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");

		}

		public async void setContentViewWithQueue (ContentView view, string type)
		{
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			topBar.hideAll();
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();

			System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");

			contentViews.Add (view);
			contentViewTypes.Add (type);

			topBar.showBackButton (true);

			topBar.setNavigationLabel(type);

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

				ContentView oldView = contentViews [contentViews.Count - 1];


				if (count > 2) {
				
					App.coreView.setContentViewWithQueue (oldView, "");

					contentViews.RemoveAt (contentViews.Count - 1);
					contentViewTypes.RemoveAt (contentViewTypes.Count - 1);

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
			setContentViewWithQueue(new InspectController(null, null, eve), "UserProfile");
		}

		public async void GoToSelectedGroup(string groupID)
		{
			Group grp = await _dataManager.GroupApiManager.GetGroupById(groupID);
			setContentViewWithQueue(new InspectController(null, grp, null), "Group");
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
			System.Diagnostics.Debug.WriteLine("1");
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			System.Diagnostics.Debug.WriteLine("2");
			bool answer = false;
			WarningLayout.IsVisible = true;



			optionOne.Clicked += (sender, e) =>
			{
				answer = false;
				tcs.TrySetResult(true);
				System.Diagnostics.Debug.WriteLine("Press1");
			};
			optionTwo.Clicked += (sender, e) =>
			{
				answer = true;
				System.Diagnostics.Debug.WriteLine("Press2.1");
				tcs.TrySetResult(true);
				System.Diagnostics.Debug.WriteLine("Press2.2");
			};
			optionOK.Clicked += (sender, e) =>
			{
				answer = true;
				tcs.TrySetResult(true);
				System.Diagnostics.Debug.WriteLine("Press3");
			};


			await tcs.Task;
			//tcs.SetCanceled();
			System.Diagnostics.Debug.WriteLine("3");
			WarningLayout.IsVisible = false;
			return answer;
		}
	}
}

