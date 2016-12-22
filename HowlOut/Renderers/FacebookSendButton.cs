using System;
using Xamarin.Forms;

namespace HowlOut
{
	public class FacebookSendButton : View
	{
		public static readonly BindableProperty LinkProperty =
			BindableProperty.Create("Link", typeof(string), typeof(FacebookSendButton), "");

		public string Link
		{
			get { return (string)GetValue(LinkProperty); }
			set { SetValue(LinkProperty, value); }
		}
	}
}
