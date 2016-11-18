using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class NotificationView : ContentView
	{
		public NotificationView()
		{
			InitializeComponent();
		}


		public NotificationView(Notification notification)
		{
			InitializeComponent();

			time.Text = notification.SendTime.ToString("dddd HH:mm - dd MMMMM yyyy");
		}
	}
}
