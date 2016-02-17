using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		Event newEvent = new Event();

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
				newEvent.StartTime = startTime.Time.ToString();
			};
			endTime.PropertyChanged += (sender, e) => {
				newEvent.EndTime = endTime.Time.ToString();
			};
			startDate.PropertyChanged += (sender, e) => {
				newEvent.StartDate = startDate.Date.ToString();
			};
			endDate.PropertyChanged += (sender, e) => {
				newEvent.EndDate = endDate.Date.ToString();
			};
			minAge.TextChanged += (sender, e) => {
				newEvent.MinAge = minAge.Text;
			};
			maxAge.TextChanged += (sender, e) => {
				newEvent.MaxAge = maxAge.Text;
			};
			minSize.Completed += (sender, e) => {
				newEvent.MinSize = minSize.Text;
			};
			maxSize.Completed += (sender, e) => {
				newEvent.MaxSize = maxSize.Text;
			};


			launchButton.Clicked += (sender, e) =>
			{
				System.Diagnostics.Debug.WriteLine("ClickedLaunche");
				App.coreView.setContentView(new InspectEvent(newEvent, 2), 0);
				LaunchEvent(newEvent);
			};

		}

		private async void LaunchEvent(Event eventToCreate)
		{
			DataManager dataManager = new DataManager();

			EventDBO newEventAsDBO = new EventDBO{OwnerId = newEvent.OwnerId, Title = newEvent.Title, 
				Description = newEvent.Description, StartTime = newEvent.StartTime, EndTime = newEvent.EndTime,
				StartDate = newEvent.StartDate, EndDate = newEvent.EndDate, MinAge = newEvent.MinAge,
				MaxAge = newEvent.MaxAge, MinSize = newEvent.MinSize, MaxSize = newEvent.MaxSize};

			//await dataManager.CreateEvent (eventToCreate);
			await dataManager.CreateEvent (newEventAsDBO);
		}

	}
}

