using System;
using System.ComponentModel;

using HowlOut;
using HowlOut.iOS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Drawing;
using CoreGraphics;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(CustomTabbed), typeof(CustomTabbedRenderer))]

namespace HowlOut.iOS
{
	public class CustomTabbedRenderer : TabbedRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			System.Diagnostics.Debug.WriteLine("HEeyye");

			TabBarItem.BadgeValue = "6";

			test();
		}

		public override UIViewController SelectedViewController
		{
			get
			{
				UITextAttributes selectedTextAttributes = new UITextAttributes();
				selectedTextAttributes.Font = UIFont.FromName("ChalkboardSE-Bold", 12.0F); // SELECTED
				if (base.SelectedViewController != null)
				{
					base.SelectedViewController.TabBarItem.SetTitleTextAttributes(selectedTextAttributes, UIControlState.Normal);
					base.SelectedViewController.TabBarItem.BadgeValue = base.SelectedViewController.TabBarItem.BadgeValue;


				}
				return base.SelectedViewController;
			}
			set
			{
				base.SelectedViewController = value;

				foreach (UIViewController viewController in base.ViewControllers)
				{
					UITextAttributes normalTextAttributes = new UITextAttributes();
					normalTextAttributes.Font = UIFont.FromName("ChalkboardSE-Light", 9.0F); // unselected

					viewController.TabBarItem.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
				}
			}

		}

		public async void test()
		{
			try
			{
				if (App.notificationController.unseenCommunications > 0)
				{
					base.ViewControllers[1].TabBarItem.BadgeValue = App.notificationController.unseenCommunications + "";
				}
				else {
					base.ViewControllers[1].TabBarItem.BadgeValue = null;
				}

				if (App.notificationController.unseenNotifications > 0)
				{
					base.ViewControllers[2].TabBarItem.BadgeValue = App.notificationController.unseenNotifications + "";
				}
				else {
					base.ViewControllers[2].TabBarItem.BadgeValue = null;
				}
			}
			catch (Exception exc) {}

			await Task.Delay(1000);
			//
			test();
		}
	}
}
