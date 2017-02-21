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

			TapGestureRecognizer tgr = new TapGestureRecognizer();
			tgr.Tapped += async (sender, e) =>
			{
				Notification noti = (Notification)this.BindingContext;
				System.Diagnostics.Debug.WriteLine(noti.Header + ", " + noti.NotificationType);
				if (noti.NotificationType == NotificationType.InvitedToEvent)
				{
					await App.coreView._dataManager.AttendTrackEvent(noti.ModelId, true, true);
				}
				if (noti.NotificationType == NotificationType.InvitedToEventAsOwner)
				{
					await App.coreView._dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(noti.ModelId, OwnerHandlingType.Accept);
				}
				if (noti.NotificationType == NotificationType.InvitedToGroup)
				{
					await App.coreView._dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(noti.ModelId, GroupApiManager.GroupHandlingType.Request);
				}
				if (noti.NotificationType == NotificationType.InvitedToGroupAsOwner)
				{
					await App.coreView._dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroupAsOwner(noti.ModelId, OwnerHandlingType.Accept);
				}
				if (noti.NotificationType == NotificationType.RequestedToFriend)
				{
					await App.coreView._dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(noti.ModelId, true);
				}
			};
			AcceptImg.GestureRecognizers.Add(tgr);

			tgr = new TapGestureRecognizer();
			tgr.Tapped += async (sender, e) =>
			{
				Notification noti = (Notification)this.BindingContext;
				System.Diagnostics.Debug.WriteLine("Decline");
				if (noti.NotificationType == NotificationType.InvitedToEvent)
				{
					await App.coreView._dataManager.AttendTrackEvent(noti.ModelId, false, true);
				}
				if (noti.NotificationType == NotificationType.InvitedToEventAsOwner)
				{
					await App.coreView._dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(noti.ModelId, OwnerHandlingType.Decline);
				}
				if (noti.NotificationType == NotificationType.InvitedToGroup)
				{
					await App.coreView._dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(noti.ModelId, GroupApiManager.GroupHandlingType.Decline);
				}
				if (noti.NotificationType == NotificationType.InvitedToGroupAsOwner)
				{
					await App.coreView._dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroupAsOwner(noti.ModelId, OwnerHandlingType.Decline);
				}
				if (noti.NotificationType == NotificationType.RequestedToFriend)
				{
					await App.coreView._dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(noti.ModelId, true);
				}
			};
			DeclineImg.GestureRecognizers.Add(tgr);
		}
	}
}
