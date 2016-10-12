using System;
using HowlOut.iOS.Renderers;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.IO;
using HowlOut;
using Newtonsoft.Json;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using HowlOut.iOS;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;
using Foundation;
using System.Net.Http;
using ModernHttpClient;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
[assembly: Dependency(typeof(HowlOut.iOS.Renderers.LoginPageRenderer.SaveAndLoad))]

namespace HowlOut.iOS.Renderers
{
    public class LoginPageRenderer : PageRenderer
    {
        bool IsShown;

		List<string> readPermissions = new List<string> { "public_profile" };

		LoginButton loginView;
		ProfilePictureView pictureView;
		UILabel nameLabel;

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
			// this notification will be called after the user is logged in and
			// after the AccessToken is gotten
			Facebook.CoreKit.Profile.Notifications.ObserveDidChange((sender, e) =>
			{

				if (e.NewProfile == null)
					return;

				nameLabel.Text = e.NewProfile.Name;
			});

			// Set the Read and Publish permissions you want to get
			loginView = new LoginButton(new CGRect(51, 0, 218, 46))
			{
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = readPermissions.ToArray()
			};

			// Handle actions once the user is logged in
			loginView.Completed += (sender, e) =>
			{
				if (e.Error != null)
				{
					// Handle if there was an error
					HowlOut.LoginPage.LoginCancel();
				}

				if (e.Result.IsCancelled)
				{
					// Handle if the user cancelled the login request
					HowlOut.LoginPage.LoginCancel();
				}

				App.SetToken(e.Result.Token.TokenString);
				App.SetUserFacebookId(e.Result.Token.UserID);



				// Handle your successful login
				HowlOut.LoginPage.LoginSuccess();
			};

			// Handle actions once the user is logged out
			loginView.LoggedOut += (sender, e) =>
			{
				// Handle your logout
			};

			// The user image profile is set automatically once is logged in
			pictureView = new ProfilePictureView(new CGRect(50, 50, 220, 220));

			// Create the label that will hold user's facebook name
			nameLabel = new UILabel(new RectangleF(20, 319, 280, 21))
			{
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			// Add views to main view
			View.AddSubview(loginView);
			View.AddSubview(pictureView);
			View.AddSubview(nameLabel);
		}

		/*
		public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!IsShown)
            {
                IsShown = true;

                var auth = new OAuth2Authenticator(
                clientId: "651141215029165", // OAuth2 client Id (App-ID in facebook)
                scope: "email", // Scopes for information needed to access
                authorizeUrl: new Uri("https://www.facebook.com/dialog/oauth"), // Authentication URL for service (i.e FB, Twitter, Google etc.)
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html.")); // Redirect URL for the service               

                auth.Completed += (sender, eventArgs) =>
                {
                    if (eventArgs.IsAuthenticated)
                    {

                        //Saves Token, and Calls LoginSuccess() to change Screen
                        var access = eventArgs.Account.Properties["access_token"];
						Console.WriteLine(access);
						App.SetToken(access);

                        var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, eventArgs.Account);

                        var obj = request.GetResponseAsync().Result.GetResponseText();
                        var facebookUserObj = JsonConvert.DeserializeObject<FacebookUserObject>(obj);
                        App.SetUserFacebookId(facebookUserObj.id);
                        App.SetUserFacebookName(facebookUserObj.name);

                        HowlOut.LoginPage.LoginSuccess();
                    }
                    else
                    {
                        HowlOut.LoginPage.LoginCancel();
                    }
                };
                PresentViewController(auth.GetUI(), true, null);
            }
        }
		*/

        public class SaveAndLoad : HowlOut.App.ISaveAndLoad
        {
            public void SaveText(string filename, string text)
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                System.IO.File.WriteAllText(filePath, text);
            }
            public string LoadText(string filename)
            {
                try
                {
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var filePath = Path.Combine(documentsPath, filename);
					var idAsString = System.IO.File.ReadAllText(filePath);
					return idAsString; 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // This allows application to redirect to "Sign-In" when there is no value stored for the Token
                    return null;

                }
            }
        }
    }
}
