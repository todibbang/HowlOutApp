using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectProfile : ContentView
	{

		HttpClient httpClient;

		public InspectProfile ()
		{
			InitializeComponent ();

			Likes.Text = "24";
			Loyalty.Text = "82%";
			NameAndAge.Text = "Tobias Bjerge Bang, 23";

			//var profilePicUri = new Uri("http://xamarin.com/content/images/pages/forms/example-app.png");
			var profilePicUri = new Uri("https://graph.facebook.com/"+App.UserFacebookId+"/picture");

			ProfileImage.Source = ImageSource.FromUri(profilePicUri);
		}

		private void profileImageGetter(Image profile)
		{
			//var profilePicUri = new Uri("http://xamarin.com/content/images/pages/forms/example-app.png");

			//var webImage = new Image { Aspect = Aspect.AspectFit };
			//webImage.Source = ImageSource.FromUri(profilePicUri);
			//profile = webImage;
		}
	}
}

