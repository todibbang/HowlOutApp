using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		public ObservableCollection<Event> searchEventList;
		public ObservableCollection<Event> manageEventList;

		public List <ContentView> contentViews = new List<ContentView> ();
		public List <string> contentViewTypes = new List<string> ();

		public SearchEvent searchEvent;
		public ManageEvent manageEvent;

		public CoreView ()
		{
			InitializeComponent ();

			System.Diagnostics.Debug.WriteLine ("Test Run: 1");

			searchEventList= new ObservableCollection<Event>();
			manageEventList= new ObservableCollection<Event>();

			System.Diagnostics.Debug.WriteLine ("Test Run: 2");

			DataManager dataManager = new DataManager ();
			dataManager.update ();
			System.Diagnostics.Debug.WriteLine ("Test Run: 3");

			searchEvent = new SearchEvent();
			manageEvent = new ManageEvent();
			System.Diagnostics.Debug.WriteLine ("Test Run: 4");
			contentViews.Add (searchEvent);
			contentViewTypes.Add ("SearchEvent");

			CreateButton.IsVisible = true;
			CreateButton.Text = "0";

			mainView.Content = searchEvent;

			CreateButton.Clicked += (sender, e) =>
			{
				if(contentViewTypes[contentViewTypes.Count-1] == "SearchEvent") {
					App.coreView.setContentView(new FilterSearch(), "FilterSearch");
				}
				if(contentViewTypes[contentViewTypes.Count-1] == "ManageEvent") {
					App.coreView.setContentView(new CreateEvent(new Event(), true), "CreateEvent"); 
				}
			};
		}

		public async void setContentView (ContentView view, string type)
		{
			await ViewExtensions.ScaleTo (mainView.Content, 0, 200);

			CreateButton.IsVisible = false;
			CreateImage.IsVisible = false;

			if (type == "SearchEvent") {
				view = searchEvent;
				CreateButton.IsVisible = true;
				CreateButton.Text = "";
				CreateImage.IsVisible = true;
				CreateImage.Source = "ic_search.png";
			} else if (type == "ManageEvent") {
				view = manageEvent;
				CreateButton.IsVisible = true;
				CreateButton.Text = "";
				CreateImage.IsVisible = true;
				CreateImage.Source = "ic_add.png";
			}

			contentViews.Add (view);
			contentViewTypes.Add (type);

			await ViewExtensions.ScaleTo(view, 0, 0);
			mainView.Content = view;
			await ViewExtensions.ScaleTo(view, 1, 400);
		}

		public void returnToPreviousView()
		{
			if (contentViews.Count != 1) {
				
				contentViews.RemoveAt (contentViews.Count - 1);
				contentViewTypes.RemoveAt (contentViewTypes.Count - 1);

				ContentView oldView = contentViews [contentViews.Count - 1];
				string oldType = contentViewTypes [contentViewTypes.Count - 1];

				App.coreView.setContentView (oldView, oldType);

				contentViews.RemoveAt (contentViews.Count - 1);
				contentViewTypes.RemoveAt (contentViewTypes.Count - 1);

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

