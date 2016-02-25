using System;
using HowlOut.iOS.Renderers;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.IO;
using HowlOut;
using Newtonsoft.Json;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
[assembly: Dependency(typeof(HowlOut.iOS.Renderers.LoginPageRenderer.SaveAndLoad))]

namespace HowlOut.iOS.Renderers
{
    public class LoginPageRenderer : PageRenderer
    {
        bool IsShown;

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
