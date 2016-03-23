using System;
using System.Collections.Generic;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InspectGroup : ContentView
	{
		DataManager dataManager = new DataManager ();
		Profile inspectedProfile;

		public InspectGroup (Group group)
		{
			InitializeComponent ();

			//Likes.Text = group.Likes + "";
			//Loyalty.Text = group.LoyaltyRating + "";
			NameAndAge.Text = group.Name;
			groupImage.Text = group.Members.Count + "";

			invite.Clicked += (sender, e) => 
			{
				App.coreView.setContentView (new InviteView (group, null, InviteView.WhatToShow.PeopleToInviteToGroup), "InviteView");
			};
		}
	}
}

