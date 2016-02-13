using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CoreView : ContentPage
	{


		public CoreView (ContentView view)
		{
			InitializeComponent ();

			mainView.Content = view;


		}

		public void setContentView (ContentView view)
		{
			mainView.Content = view;
		}



	}
}

