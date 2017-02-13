using System;

using Xamarin.Forms;

namespace HowlOut
{
	public class NavigationPageTest : NavigationPage
	{
		public NavigationPageTest(int baseType)
		{
			ContentPageFrame c1 = null;
			if (baseType == 0) c1 = new ContentPageFrame(new CreateView(), false);
			if (baseType == 1) c1 = new ContentPageFrame(new EventListView(0), false);
			if (baseType == 2) c1 = new ContentPageFrame(new EventListView(0), false);
			if (baseType == 3) c1 = new ContentPageFrame(new YourConversations(ConversationModelType.Profile, App.userProfile.ProfileId, 0), false);
			if (baseType == 4) c1 = new ContentPageFrame(new YourNotifications(), false);
			BackgroundColor = App.HowlOut;
			PushAsync(c1);
		}

		public void pushView(ViewModelInterface cv)
		{
			//var c = new ContentPageFrame(cv);

			//var c = new ContentPage() { Content = cv };


			var c = new ContentPageFrame(cv, true);

			PushAsync(c);
		}

		public void pushView(Grid cv)
		{
			var c = new ContentPageFrame(cv);
			PushAsync(c);
		}
	}
}

