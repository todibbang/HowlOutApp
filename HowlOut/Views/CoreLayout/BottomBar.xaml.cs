using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class BottomBar : ContentView
	{
		DataManager _dataManager;
		//private StandardButton standardButton = new StandardButton();

		//public Button createNoti { get { return noti_0; } }
		public Button conversationNoti { get { return noti_3; } }
		//public Button eventNoti { get { return noti_2; } }
		//public Button howlsNoti { get { return noti_3; } }
		public Button homeNoti { get { return noti_2; } }

		public BottomBar()
		{
			InitializeComponent();
			_dataManager = new DataManager();

			//MeButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Transparent, "", 0));
			//EventButtonLayout.Children.Add(standardButton.StandardButtonGrid(StandardButton.StandardButtonType.Transparent, "", 0));


			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(0);
				await create.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await create.ScaleTo(1, 50, Easing.Linear);
				unselectAll();
				create.Foreground = App.HowlOut;
			};
			create.GestureRecognizers.Add(createImage);

			var manageImage = new TapGestureRecognizer();
			manageImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(1);
				await manage.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await manage.ScaleTo(1, 50, Easing.Linear);
				unselectAll();
				manage.Foreground = App.HowlOut;
			};
			manage.GestureRecognizers.Add(manageImage);

			explore.Clicked += async (sender, e) =>
			{
				App.coreView.setContentView(2);
				await exploreImg.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await exploreImg.ScaleTo(1, 50, Easing.Linear);
				unselectAll();
				exploreImg.Foreground = App.HowlOut;
			};

			var friendsImage = new TapGestureRecognizer();
			friendsImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(3);
				await howls.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await howls.ScaleTo(1, 50, Easing.Linear);
				unselectAll();
				howls.Foreground = App.HowlOut;
			};
			howls.GestureRecognizers.Add(friendsImage);

			var socialImage = new TapGestureRecognizer();
			socialImage.Tapped += async (sender, e) =>
			{
				App.coreView.setContentView(4);
				//App.coreView.homeView.updateLists();
				await me.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await me.ScaleTo(1, 50, Easing.Linear);
				unselectAll();
				me.Foreground = App.HowlOut;
			};
			me.GestureRecognizers.Add(socialImage);
		}

		public void selectButton(int b)
		{
			unselectAll();
			if (b == 0)
			{
				create.Foreground = App.HowlOut;
			}
			else if (b == 1)
			{
				manage.Foreground = App.HowlOut;
			}
			else if (b == 2)
			{
				exploreImg.Foreground = App.HowlOut;
			}
			else if (b == 3)
			{
				howls.Foreground = App.HowlOut;
			}
			else if (b == 4)
			{
				me.Foreground = App.HowlOut;
			}

		}

		void unselectAll()
		{
			create.Foreground = Color.White;
			manage.Foreground = Color.White;
			exploreImg.Foreground = Color.White;
			howls.Foreground = Color.White;
			me.Foreground = Color.White;
		}
	}
}

