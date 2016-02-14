using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{
		public CoreView (ContentView view, bool createEventBool)
		{
			InitializeComponent ();

			CreateButton.IsVisible = false;
			if(createEventBool) CreateButton.IsVisible = true;

			mainView.Content = view;

			CreateButton.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new CreateEvent(), false);
			};
		}

		public void setContentView (ContentView view, bool createEventBool)
		{
			mainView.Content = view;

			if (createEventBool)
				CreateButton.IsVisible = true;
			else CreateButton.IsVisible = false;
		}




	}
}

