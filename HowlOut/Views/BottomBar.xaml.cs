﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class BottomBar : ContentView
	{
		public BottomBar ()
		{
			InitializeComponent ();

			var exploreImage = new TapGestureRecognizer();
			exploreImage.Tapped += async (sender, e) => 
			{
				await exploreBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await exploreBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentView(new SearchEvent(), 1);
			};
			exploreBtn.GestureRecognizers.Add(exploreImage);  

			var manageImage = new TapGestureRecognizer();
			manageImage.Tapped += async (sender, e) => 
			{
				await manageBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await manageBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentView(new ManageEvent(), 2);
			};
			manageBtn.GestureRecognizers.Add(manageImage); 

			/*
			knapOne.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new SearchEvent(), 1);
			};
			knapTwo.Clicked += (sender, e) =>
			{
				App.coreView.setContentView(new ManageEvent(), 2);
			};
			*/
		}
	}
}

