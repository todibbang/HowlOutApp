using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class IntroSlides : CarouselPage
	{
		public IntroSlides(App app)
		{
			InitializeComponent();

			next1.Clicked += (sender, e) =>
			{
				CurrentPage = page2;
			};

			next2.Clicked += (sender, e) =>
			{
				app.startProgram();
			};

		}
	}
}
