using System;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class App : Application
	{
		public static CoreView coreView;

		public App ()
		{
			coreView = new CoreView(new SearchEvent());
			InitializeComponent();
			// The root page of your application
			//MainPage = new MyPeople();
			MainPage = coreView;


		}



		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

