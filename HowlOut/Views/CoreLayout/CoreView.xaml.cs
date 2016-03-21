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

		public CoreView ()
		{
			InitializeComponent ();
			_dataManager = new DataManager ();
			topBar = new UpperBar();
			topBarLayout.Children.Add (topBar);
		}

		public void startCoreView()
		{
			EventView eventView = new EventView();

			_dataManager.update ();

			contentViews.Add (eventView);
			contentViewTypes.Add ("Event");
			topBar.setNavigationLabel("Event");
			mainView.Content = eventView;
		}

		public async void setContentView (ContentView view, string type)
		{
			await ViewExtensions.ScaleTo (mainView.Content, 0, 200);

			contentViews.Add (view);
			contentViewTypes.Add (type);

			topBar.setNavigationLabel(type);

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

