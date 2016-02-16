using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		Event newEvent = new Event();

		string Title;
		string OwnerId;
		string Description;
		string StartTime;
		string EndTime;
		string StartDate;
		string EndDate;
		string MinAge;
		string MaxAge;
		string MinSize;
		string MaxSize;


		public CreateEvent ()
		{
			InitializeComponent ();

			newEvent.OwnerId = App.StoredUserFacebookId;

			title.TextChanged += (sender, e) => {
				newEvent.Title = title.Text;
				System.Diagnostics.Debug.WriteLine(newEvent.Title);
			};
			description.TextChanged += (sender, e) => {
				newEvent.Description = description.Text;
			};

			startTime.PropertyChanged += (sender, e) => {
				StartTime = startTime.Time.ToString();
			};
			endTime.PropertyChanged += (sender, e) => {
				EndTime = endTime.Time.ToString();
			};
			startDate.PropertyChanged += (sender, e) => {
				StartDate = startDate.Date.ToString();
			};
			endDate.PropertyChanged += (sender, e) => {
				EndDate = endDate.Date.ToString();
			};
			minAge.TextChanged += (sender, e) => {
				MinAge = minAge.Text;
			};
			maxAge.TextChanged += (sender, e) => {
				MaxAge = maxAge.Text;
			};
			minSize.Completed += (sender, e) => {
				MinSize = minSize.Text;
			};
			maxSize.Completed += (sender, e) => {
				MaxSize = maxSize.Text;
			};


			launchButton.Clicked += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine("Clicked");
				App.coreView.setContentView(new InspectEvent(newEvent, 2), 0);
				LaunchEvent(newEvent);
			};

			inviteButton.Clicked += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine("ClickedInvite");
				LaunchEvent(newEvent);
			};

		}

		private async void LaunchEvent(Event eventToCreate)
		{
			DataManager dataManager = new DataManager();
			await dataManager.CreateEvent (eventToCreate);
		}

	}
}

