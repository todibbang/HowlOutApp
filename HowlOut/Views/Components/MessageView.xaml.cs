using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class MessageView : ContentView
	{
		DataManager _dataManager = new DataManager();

		public MessageView (Event eventObject, MessageType type)
		{
			InitializeComponent ();

			headerLayout.IsVisible = false;

			HeaderContent.Content = new ProfileDesignView (eventObject.Owner, null, null, 50, ProfileDesignView.ProfileDesign.Plain);

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
			Plain,
			Buttons,
			Label,
			LabelAndButtons,
			Invite
		}
	}
}

