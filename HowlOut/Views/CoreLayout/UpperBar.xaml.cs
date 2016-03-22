﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class UpperBar : ContentView
	{
		public UpperBar ()
		{
			InitializeComponent ();

			var backImage = new TapGestureRecognizer();
			backImage.Tapped += async (sender, e) => 
			{
				await backBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await backBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.returnToPreviousView();
			};				
			backBtn.GestureRecognizers.Add(backImage); 

			var updateImage = new TapGestureRecognizer();
			updateImage.Tapped += async (sender, e) =>
			{
				await updateBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await updateBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentView (new EventView (), "Event");
			};				
			updateBtn.GestureRecognizers.Add(updateImage);

			howlOut.Clicked += (sender, e) => 
			{
				
			};
		}

		public void setNavigationLabel(string label)
		{
			//navigationLabel.Text = label;
		}
	}
}
