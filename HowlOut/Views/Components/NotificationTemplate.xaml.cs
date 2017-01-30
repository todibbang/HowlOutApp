using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class NotificationTemplate : ContentView
	{
		public NotificationTemplate()
		{
			InitializeComponent();

			setup();

		}

		async void setup()
		{
			try
			{
				Notification noti = (Notification)this.BindingContext;

				if (
					noti.NotificationType == NotificationType.InvitedToEvent ||
					noti.NotificationType == NotificationType.InvitedToEventAsOwner ||
					noti.NotificationType == NotificationType.InvitedToGroup ||
					noti.NotificationType == NotificationType.InvitedToGroupAsOwner ||
					noti.NotificationType == NotificationType.RequestedToFriend)
				{
					BtnLayout.IsVisible = true;
				}

				TapGestureRecognizer tgr = new TapGestureRecognizer();
				tgr.Tapped += (sender, e) =>
				{
					System.Diagnostics.Debug.WriteLine("Accept");
				};
				AcceptBtn.GestureRecognizers.Add(tgr);

				tgr = new TapGestureRecognizer();
				tgr.Tapped += (sender, e) =>
				{
					System.Diagnostics.Debug.WriteLine("Decline");
				};
				DeclineBtn.GestureRecognizers.Add(tgr);
			}
			catch (Exception exc)
			{
				await Task.Delay(100);
				setup();
			}
		}
	}
}
