using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class HowlOutLogin : ContentPage
	{
		public HowlOutLogin()
		{
			InitializeComponent();

			returnBtn.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new LoginPage());
			};
		}
	}
}
