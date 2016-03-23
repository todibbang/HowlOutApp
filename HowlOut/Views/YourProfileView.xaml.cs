using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class YourProfileView : ContentView
	{
		public YourProfileView ()
		{
			InitializeComponent ();

			profileContent.Content = new InspectController (App.userProfile, null, null);
			findContent.Content = new InviteView (null, null, InviteView.WhatToShow.NewPeople);
			createGroupContent.Content = new CreateGroup ();

			profileButton.Clicked += (sender, e) => {
				setViewDesign(0);
			};
			findButton.Clicked += (sender, e) => {
				setViewDesign(1);
			};
			createGroupButton.Clicked += (sender, e) => {
				setViewDesign(2);
			};
		}

		private void setViewDesign(int number){
			profileContent.IsVisible = false;
			findContent.IsVisible = false;
			createGroupContent.IsVisible = false;
			profileButton.FontAttributes = FontAttributes.None;
			findButton.FontAttributes = FontAttributes.None;
			createGroupButton.FontAttributes = FontAttributes.None;
			profileLine.IsVisible = true;
			findLine.IsVisible = true;
			createGroupLine.IsVisible = true;

			if (number == 0) {
				profileContent.IsVisible = true;
				profileButton.FontAttributes = FontAttributes.Bold;
				profileLine.IsVisible = false;
			} else if (number == 1) {
				findContent.IsVisible = true;
				findButton.FontAttributes = FontAttributes.Bold;
				findLine.IsVisible = false;
			} else if (number == 2) {
				createGroupContent.IsVisible = true;
				createGroupButton.FontAttributes = FontAttributes.Bold;
				createGroupLine.IsVisible = false;
			}
		}
	}
}

