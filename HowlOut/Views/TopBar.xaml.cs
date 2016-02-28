using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class TopBar : ContentView
	{
		public TopBar ()
		{
			InitializeComponent ();

			var backImage = new TapGestureRecognizer();
			backImage.Tapped += (sender, e) => 
			{
				App.coreView.returnToPreviousView();
			};
			backBtn.GestureRecognizers.Add(backImage); 

			//back.Clicked += (sender, e) =>
			//{
				//App.coreView.returnToPreviousView();
			//};
			knapTwo.Clicked += (sender, e) =>
			{
			};
			//knapThree.Clicked += (sender, e) =>
			//{
			//};
		}
	}
}

