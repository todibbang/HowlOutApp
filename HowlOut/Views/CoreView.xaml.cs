using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		private int viewStyle = 0;
		public CoreView (ContentView view)
		{
			InitializeComponent ();

			CreateButton.IsVisible = true;
			CreateButton.Text = "0";

			mainView.Content = view;

			CreateButton.Clicked += (sender, e) =>
			{
				if(viewStyle == 1) App.coreView.setContentView(new FilterSearch(), 0);
				if(viewStyle == 2) App.coreView.setContentView(new CreateEvent(), 0);
			};
		}

		public void setContentView (ContentView view, int floatingButton)
		{
			mainView.Content = view;
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




	}
}

