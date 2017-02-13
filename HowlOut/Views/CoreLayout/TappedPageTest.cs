using System;

using Xamarin.Forms;

namespace HowlOut
{
	public class TappedPageTest : TabbedPage
	{
		public TappedPageTest()
		{
			
			var image = new Image();
			image.Source = "ic_me.png";
			NavigationPage.SetHasNavigationBar(this, false);

			//byte[] data = File.ReadAll("image.png");

			var file = new FileImageSource() { File = "ic_me.png"  };


			//var img = DependencyService.Get<ImageResizer>().ResizeImage( "" , 50)




			var page1 = new NavigationPageTest(0) { Title = "Create", Icon = "ic_add_circle_white.png"};
			//var page2 = new NavigationPageTest(1) { Title = "null"};
			var page3 = new NavigationPageTest(2) { Title = "Home", Icon = "ic_howlout.png"};
			var page4 = new NavigationPageTest(3) { Title = "Communication", Icon = "ic_message_white"};
			var page5 = new NavigationPageTest(4) { Title = "Notifications", Icon = "ic_public_white"};
			this.Children.Add(page1);
			//this.Children.Add(page2);
			this.Children.Add(page3);
			this.Children.Add(page4);
			this.Children.Add(page5);



			BarBackgroundColor = Color.FromHex("#cc000000");
			BarTextColor = App.HowlOut;
		}

		public void pushView(ViewModelInterface cv)
		{
			var navPage = CurrentPage as NavigationPageTest;

			navPage.pushView(cv);
		}

		public void pushView(Grid ve)
		{
			var navPage = CurrentPage as NavigationPageTest;
			navPage.pushView(ve);
		}

		public void popView()
		{
			var navPage = CurrentPage as NavigationPageTest;

			navPage.PopAsync();
		}
	}
}

