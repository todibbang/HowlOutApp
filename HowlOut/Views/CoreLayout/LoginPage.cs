using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace HowlOut
{
	public class LoginPage : ContentPage
	{
		public LoginPage()
		{
			System.Diagnostics.Debug.WriteLine("Height" + Height);
			// Page is Rendered in CustomRenderer for IOS and Android
		}

		public static event EventHandler FacebookLoginSucceeded;
		public static event EventHandler HowlOutLoginAttempted;
		public static event EventHandler LoginCancelled;
		public static Object sender;
		public static void LoginSuccess()
		{


			//Invoked and then sent to the App.cs
			FacebookLoginSucceeded(sender, EventArgs.Empty);

		}

		public static void HowlOutLogin()
		{
			HowlOutLoginAttempted(sender, EventArgs.Empty);
		}

		public static void LoginCancel()
		{
			//Invoked and then sent to the App.cs
			LoginCancelled(sender, EventArgs.Empty);

		}
	}
}
