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
				pickerLayout.Children.Add(sl);
			}
		}
	}
}
