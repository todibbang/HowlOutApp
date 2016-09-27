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
		public EventView searchEventView;
		public EventView manageEventView;
		public EventView friendsEventView;
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
			searchEventView = new EventView(0);
			manageEventView = new EventView(1);
			friendsEventView = new EventView(2);
			homeView = new HomeView();

			_dataManager.update ();

			contentViews.Add (manageEventView);
			contentViewTypes.Add ("Event");
			topBar.setNavigationLabel("Event");
			mainView.Content = manageEventView;
			topBar.showOptionGrid(true, "Attending", "Following");
			loading.IsVisible = false;
		}

		public async void setContentView (int type)
		{
			topBar.hideAll();

			var first = DateTime.Now;
			//await ViewExtensions.ScaleTo (mainView.Content, 0, 200);
			ContentView view = null;

			//System.Diagnostics.Debug.WriteLine (view.ToString() + " , the new view");
			if (type == 0)
			{
				view = createEventView;
			} else if (type == 1)
			{
				view = searchEventView;
				topBar.showFilterSearchButton(true);
				topBar.showSearchBar(true);
			} else if (type == 2)
			{
				view = manageEventView;
				topBar.showOptionGrid(true, "Attending", "Following");
			} else if (type == 3)
			{
				view = friendsEventView;
				topBar.showOptionGrid(true, "Friends", "Invites");
			} else if (type == 4)
			{
				view = homeView;
				topBar.showSearchBar(true);
				topBar.showCreateNewGroupButton(true);
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

			System.Diagnostics.Debug.WriteLine ("contentViews.Count: " + contentViews.Count);
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

		public async Task displayAlertMessage (string title, string message, string buttonText)
		{
			await DisplayAlert (title, message, buttonText);
		}

		public async Task<bool> displayConfirmMessage (string title, string message, string confirm, string decline)
		{
			var answer = await DisplayAlert (title, message, confirm, decline);
			return answer;
		}

	}
}

