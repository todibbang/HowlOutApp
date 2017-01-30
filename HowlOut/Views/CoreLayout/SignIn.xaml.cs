using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class SignIn : ContentPage
	{
		public SignIn()
		{
			InitializeComponent();
			Navigation.PushModalAsync(new LoginPage());
		}

		void fb_click(Object sender, EventArgs e)
		{
			
		}
	}
}
