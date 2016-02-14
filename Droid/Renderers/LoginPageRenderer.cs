using System;

using Android.App;
using HowlOut.Droid.Renderers;
using HowlOut;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.IO;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
[assembly: Dependency(typeof(HowlOut.Droid.Renderers.LoginPageRenderer.SaveAndLoad))]

namespace HowlOut.Droid.Renderers
{
	public class LoginPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            var activity = this.Context as Activity;

            OAuth2Authenticator auth = new OAuth2Authenticator(
                clientId: "651141215029165", // OAuth2 client Id (App-ID in facebook)
                scope: "email", // Scopes for information needed to access
                authorizeUrl: new Uri("https://www.facebook.com/dialog/oauth"), // Authentication URL for service (i.e FB, Twitter, Google etc.)
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html.")); // Redirect URL for the service

            auth.Completed += (sender, eventArgs) => {
                if (eventArgs.IsAuthenticated)
                {
                    //Saves Token, and Calls LoginSuccess() to change Screen
                    var access = eventArgs.Account.Properties["access_token"];
					//var properties = eventArgs.Account.Properties;
                    App.SetToken(access);

                    HowlOut.LoginPage.LoginSuccess();
                }
                else {
                    HowlOut.LoginPage.LoginCancel();
                }
            };
            activity.StartActivity(auth.GetUI(activity));
        }

        //Class implemented to save token in a local file in the App's Directory
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
                    return System.IO.File.ReadAllText(filePath);
                }
                catch (Exception e)
                {
                    // This allows application to redirect to "Sign-In" when there is no value stored for the Token
                    Console.WriteLine(e);
                    return null;

                }
            }
        }
    }
}