using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ToDoItemTemplate : ContentView
	{
		List<CustomPicker> profilePicker = new List<CustomPicker>();

		public ToDoItemTemplate()
		{
			InitializeComponent();
			//var thisItem = (ToDoItem)this.BindingContext;
			//System.Diagnostics.Debug.WriteLine(thisItem.OptionDescription);
			lateSetup();
		}

		async void lateSetup()
		{
			await Task.Delay(100);
			var thisItem = (ToDoItem)this.BindingContext;

			try
			{
				if (thisItem.Profiles.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					backgroundBtn.BackgroundColor = App.HowlOutFade;

					if (thisItem.Completed[thisItem.Profiles.IndexOf(thisItem.Profiles.Find(p => p.ProfileId == App.userProfile.ProfileId))]) statusIcon.Foreground = App.HowlOut;
				}
			}
			catch (Exception exc) {} 



			var taps = new TapGestureRecognizer();
			taps.Tapped += (sender, e) =>
			{
				if (thisItem.Profiles != null && thisItem.Profiles.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					if (statusIcon.Foreground == Color.FromHex("#10000000"))
					{
						statusIcon.Foreground = App.HowlOut;
						thisItem.Completed[thisItem.Profiles.IndexOf(thisItem.Profiles.Find(p => p.ProfileId == App.userProfile.ProfileId))] = true;
					}
					else {
						statusIcon.Foreground = Color.FromHex("#10000000");
						thisItem.Completed[thisItem.Profiles.IndexOf(thisItem.Profiles.Find(p => p.ProfileId == App.userProfile.ProfileId))] = false;
					}

					completedLabel.Text = thisItem.NumberCompleted;
				}
			};
			completedBtn.GestureRecognizers.Add(taps);

			taps = new TapGestureRecognizer();
			taps.Tapped += (sender, e) =>
			{
				if (backgroundBtn.BackgroundColor == Color.White)
				{
					backgroundBtn.BackgroundColor = App.HowlOutFade;
					thisItem.Profiles.Add(App.userProfile);
					thisItem.Completed.Add(false);
				}
				else {
					backgroundBtn.BackgroundColor = Color.White;
					thisItem.Completed.RemoveAt(thisItem.Profiles.IndexOf(thisItem.Profiles.Find(p => p.ProfileId == App.userProfile.ProfileId)));
					thisItem.Profiles.Remove(thisItem.Profiles.Find(p => p.ProfileId == App.userProfile.ProfileId));
					statusIcon.Foreground = Color.FromHex("#10000000");
				}
				completedLabel.Text = thisItem.NumberCompleted;
				assignedLabel.Text = thisItem.AssignedAndNeeded;
			};
			joinBtn.GestureRecognizers.Add(taps);

			taps = new TapGestureRecognizer();
			taps.Tapped += (sender, e) =>
			{
				App.tappedPageTest.pushView(new ListsAndButtons(thisItem.Profiles, null, false, false ));
			};
			viewProfilesBtn.GestureRecognizers.Add(taps);



			System.Diagnostics.Debug.WriteLine(thisItem.OptionDescription);


			for (int i = 0; i < thisItem.ProfilesNeeded; i++)
			{
				profilePicker.Add(new CustomPicker() { Title = "None", HorizontalOptions = LayoutOptions.FillAndExpand });
				var picker = profilePicker[i];
				profilePicker[i].Items.Add("None");
				foreach (Profile p in thisItem.ToDoListView.profiles)
				{
					profilePicker[i].Items.Add(p.Name);
				}
				if (thisItem.Profiles != null && thisItem.Profiles.Count > i && thisItem.Profiles[i] != null)
				{
					profilePicker[i].SelectedIndex = thisItem.ToDoListView.profiles.FindIndex(p => p.ProfileId == thisItem.Profiles[i].ProfileId)+1;
					profilePicker[i].Title = thisItem.Profiles[i].Name;
				}

				var sl = new StackLayout() { Orientation = StackOrientation.Horizontal};
				sl.Children.Add(profilePicker[i]);

				var statusButton = new IconView() { Source = "ic_done_white.png", WidthRequest = 40, HeightRequest = 40, Foreground = Color.Gray };
				var tap = new TapGestureRecognizer();


				tap.Tapped += (sender, e) =>
				{
					if (picker.SelectedIndex > 0 && thisItem.ToDoListView.profiles[picker.SelectedIndex-1].ProfileId == App.userProfile.ProfileId)
					{
						if (statusButton.Foreground == Color.Gray) statusButton.Foreground = App.HowlOut;
						else statusButton.Foreground = Color.Gray;
					}
				};
				statusButton.GestureRecognizers.Add(tap);

				sl.Children.Add(statusButton);
				//pickerLayout.Children.Add(sl);
			}
		}
	}
}
