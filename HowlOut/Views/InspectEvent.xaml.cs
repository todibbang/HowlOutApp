using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();

		public InspectEvent (Event eve, int inspectType)
		{
			InitializeComponent ();

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

			eventTitle.Text = eve.Title;
			// eventTime.Text = eve.Time;
			// eventDistance.Text = eve.Position;
			eventDescription.Text = eve.Description;
			eventAttending.Text = "22";
			eventHolderLikes.Text = "22";
			eventLoyaltyRaiting.Text = "22";

			if (inspectType == 1) {
				searchSpecific.IsVisible = true;
				manageSpecific.IsVisible = false;
			} else if (inspectType == 2) {
				searchSpecific.IsVisible = false;
				manageSpecific.IsVisible = true;

				CommentList.ItemsSource = comments;

				comments.Add (new Comment {
					Title = "Rob Finnerty",
					Content = "Test1 asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf asdf xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb xcvb"
				});
			}
			detailsButton.Clicked += (sender, e) =>
			{
				if(detailedInfo.IsVisible == false)
				{
					detailedInfo.IsVisible = true;
					quickInfo.IsVisible = false;
				}
				else 
				{
					detailedInfo.IsVisible = false;
					quickInfo.IsVisible = true;
				}
			};


			eventHolderButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView(new InspectProfile(), 0);
			};

			eventGroupButton.Clicked += (sender, e) => 
			{
				App.coreView.setContentView(new InspectGroup(), 0);
			};
		}
	}
}