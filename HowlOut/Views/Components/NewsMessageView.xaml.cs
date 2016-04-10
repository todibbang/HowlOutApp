using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class NewsMessageView : ContentView
	{
		DataManager _dataManager = new DataManager();

		public NewsMessageView (Event eventObject, MessageType type)
		{
			InitializeComponent ();

			//headerLayout.IsVisible = false;

			HeaderContent.Content = new ProfileDesignView (eventObject.Owner, null, null, 50, ProfileDesignView.Design.Plain, true);

			MessageContent.Content = new SearchEventTemplate (eventObject);

			//MessageContent.Content = new SearchEventTemplate (eventObject);
			if (type == MessageType.Invite) {
				acceptButton.Clicked += (sender, e) => {
					_dataManager.joinEvent(eventObject);
				};
				declineButton.Clicked += (sender, e) => {
					_dataManager.declineEvent(eventObject);
				};
				followButton.Clicked += (sender, e) => {
					_dataManager.followEvent(eventObject);
				};
			}
		}

		public enum MessageType {
			Invite,
			Info,
		}
	}
}

