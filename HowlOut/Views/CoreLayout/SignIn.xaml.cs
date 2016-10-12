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
			/*
			App.PostSuccessFacebookAction = async token =>
			{

				System.Diagnostics.Debug.WriteLine("Blytka");
				//you can use this token to authenticate to the server here
				//call your FacebookLoginService.LoginToServer(token)
				//I'll just navigate to a screen that displays the token:



				//await Navigation.PushAsync(new DiplayTokenPage(token));

			};
			*/
        }

        void fb_click(Object sender, EventArgs e)
        {
            //Launch Facebook Login Page upon Click
            

        }
    }
}
