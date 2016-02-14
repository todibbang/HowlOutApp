using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		string Title;
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


			title.TextChanged += (sender, e) => {
				Title = title.Text;
			};
			description.TextChanged += (sender, e) => {
				Description = description.Text;
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

		}

	}
}

