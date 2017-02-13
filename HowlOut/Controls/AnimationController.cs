using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public class AnimationController
	{
		public AnimationController()
		{
		}

		public static async Task HideAnimation(VisualElement ve, double heightRequest)
		{
			for (int i = 0; i < 100; i++)
			{
				ve.HeightRequest -= (heightRequest / 100);
				ve.TranslationY -= (heightRequest / 100);
				await Task.Delay(2);
			}
			ve.HeightRequest = 0;
			ve.TranslationY = -heightRequest;

			//await ve.ScaleTo(0, 500, null);
			ve.IsVisible = false;
		}
		public static async Task ShowAnimation(VisualElement ve, double heightRequest)
		{
			ve.IsVisible = true;
			for (int i = 0; i < 100; i++)
			{
				ve.HeightRequest += (heightRequest / 100);
				ve.TranslationY += (heightRequest / 100);
				await Task.Delay(2);
			}
			ve.HeightRequest = heightRequest;
			ve.TranslationY = 0;
			//await ve.ScaleTo(1, 500, null);

		}

		public static async Task SlideInAnimation(VisualElement ve)
		{
			ve.IsVisible = true;

			//await Task.Delay(100);
			//ve.Animate("", (s) => Layout(new Rectangle(((1 - s) * App.coreView.Width), App.coreView.Y, App.coreView.Width, App.coreView.Height)), 16, 600, Easing.Linear, null, null);
			//await ve.FadeTo(0, 1, null);
			//await ve.FadeTo( 1, 500, null);



			//await ve.TranslateTo(0, 0, 500, Easing.Linear);
			/*
			ve.TranslationX = App.coreView.Width;
			for (int i = 0; i < 100; i++)
			{
				ve.TranslationX -= (App.coreView.Width / 100);
				await Task.Delay(4);
			}
			ve.TranslationX = 0;*/
		}
		public static async Task SlideOutAnimation(VisualElement ve)
		{
			//ve.TranslationX = App.coreView.Width;
			//await ve.TranslateTo(App.coreView.Width, 0, 500, Easing.Linear);


			/*
			for (int i = 0; i < 100; i++)
			{
				ve.TranslationX += (App.coreView.Width / 100);
				await Task.Delay(4);
			}*/
			ve.IsVisible = false;
			//ve.TranslationX = 0;


		}
	}
}
