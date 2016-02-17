using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		private int viewStyle = 0;


		public List <ContentView> contentViews = new List<ContentView> ();

		public CoreView (ContentView view)
		{
			InitializeComponent ();

			contentViews.Add (view);

			CreateButton.IsVisible = true;
			CreateButton.Text = "0";

			mainView.Content = view;

			CreateButton.Clicked += (sender, e) =>
			{
				if(viewStyle == 1) App.coreView.setContentView(new FilterSearch(), 0);
				if(viewStyle == 2) App.coreView.setContentView(new CreateEvent(), 0);
			};
		}

		public async void setContentView (ContentView view, int floatingButton)
		{

			await ViewExtensions.ScaleTo (mainView.Content, 0, 200);

			contentViews.Add (view);
			await ViewExtensions.ScaleTo(view, 0, 0);
			mainView.Content = view;
			await ViewExtensions.ScaleTo(view, 1, 400);

			viewStyle = floatingButton;

			if (floatingButton == 0)
				CreateButton.IsVisible = false;
			else if (floatingButton == 1) {
				CreateButton.IsVisible = true;
				CreateButton.Text = "0";
			} else {
				CreateButton.IsVisible = true;
				CreateButton.Text = "+";
			}
		}

		public void returnToPreviousView()
		{
			if (contentViews.Count != 1) {
				contentViews.RemoveAt (contentViews.Count - 1);
				App.coreView.setContentView ((contentViews [contentViews.Count - 1]), 0);
				contentViews.RemoveAt (contentViews.Count - 1);
			}
		}



	}
}

