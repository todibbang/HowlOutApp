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

		List<string> readPermissions = new List<string> { "public_profile", "user_events" };

		LoginButton loginView;
		ProfilePictureView pictureView;
		UILabel nameLabel;
		UIButton b;

		string name = "";
		string token = "";
		string id = "";

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			int height = AppDelegate.height;
			int width = AppDelegate.width;

			// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
			// this notification will be called after the user is logged in and
			// after the AccessToken is gotten
			Facebook.CoreKit.Profile.Notifications.ObserveDidChange((sender, e) =>
			{

				if (e.NewProfile == null)
					return;
				
				nameLabel.Text = e.NewProfile.Name;
				name = e.NewProfile.Name;
				logIn();
			});

			b = UIButton.FromType(UIButtonType.System);
			b.SetTitle("Button!", UIControlState.Normal);

			b.TouchUpInside += (sender, e) => {
				System.Diagnostics.Debug.WriteLine("Lort");
			};
			// Set the Read and Publish permissions you want to get


			loginView = new LoginButton(new CGRect(width / 2 - (110), height / 1.5, 220, 46))
			{
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = readPermissions.ToArray()
			};

			// Handle actions once the user is logged in
			loginView.Completed += (sender, e) =>
			{
				if (e.Error != null)
				{
					HowlOut.LoginPage.LoginCancel();
				}

				if (e.Result.IsCancelled)
				{
					HowlOut.LoginPage.LoginCancel();
				}
				token = e.Result.Token.TokenString;
				id = e.Result.Token.UserID;
			};

			// Handle actions once the user is logged out
			loginView.LoggedOut += (sender, e) =>
			{
				// Handle your logout
			};

			// The user image profile is set automatically once is logged in
			pictureView = new ProfilePictureView(new CGRect(width / 2 - 110, 50, 220, 220));

			// Create the label that will hold user's facebook name
			nameLabel = new UILabel(new RectangleF(width / 2 - 140, 319, 280, 21))
			{
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};
			// Add views to main view
			View.AddSubview(new UILabel(new RectangleF(20, 319, 280, 21)));
			View.AddSubview(loginView);
			View.AddSubview(b);
			//View.AddSubview(pictureView);
			View.AddSubview(nameLabel);
		}

		private async void logIn()
		{
			await HowlOut.App.storeToken(token, id, name);
			HowlOut.LoginPage.LoginSuccess();
		}

		/*
		async void getEvents(string id)
		{
			//var fields = "?fields=events,id,name,email,about,hometown,groups";
			var request = new GraphRequest("/me/events?fields=name,id,place,description&rsvp_status=attending", null, AccessToken.CurrentAccessToken.TokenString, null, "GET");
			var requestConnection = new GraphRequestConnection();

			var fbEvents = new List<FaceBookEvent>();

			requestConnection.AddRequest(request, (connection, result, error) =>
			{
				// Handle if something went wrong with the reques
				if (error != null)
				{
					System.Diagnostics.Debug.WriteLine("Hnnn2");
					new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
					return;
				}

				//fbEvents = JsonConvert.DeserializeObject<List<FaceBookEvent>>(result);

				NSDictionary userInfo = (result as NSDictionary);
				if (error != null)
				{

				}



			});
			requestConnection.Start();

			await System.Threading.Tasks.Task.Delay(500);



		}
		*/


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
