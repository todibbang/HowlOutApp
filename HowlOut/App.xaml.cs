using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class App : Application
	{
		public static CoreView coreView;
        private HttpClient httpClient;

        public interface ISaveAndLoad
        {
            //Needed to pull and save tokens
            void SaveText(string filename, string text);
            string LoadText(string filename);

        }

        public static string StoredToken;
        static string _Token;

        public App ()
		{
            coreView = new CoreView(new SearchEvent(), false);

            InitializeComponent();


            //Eventsfired from the LoginPage to trigger actions here
            LoginPage.LoginSucceeded += LoginPage_LoginSucceeded;
            LoginPage.LoginCancelled += LoginPage_LoginCancelled;

            //This loads a user token if existent, or else it will load "null" 
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");

            //Sets the UI to Welcome(), since it is a BaseContentPage it will first check if authorized
            if (!App.IsLoggedIn)
            {
				//MainPage = coreView;
				MainPage = new SignIn();
            }
            else
            {
                MainPage = coreView;
            }

		}

        public async Task storeToken()
        {
            //Writes a New Token upon authentication in the directory
            DependencyService.Get<ISaveAndLoad>().SaveText("token", Token);
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
        }
			

        public static string Token
        {
            get { return _Token; }
        }

        public static bool IsLoggedIn
        {
            get
            {
                //returns Boolean for Login
                return !string.IsNullOrWhiteSpace(StoredToken);

            }
        }

        public static void SetToken(string token)
        {
            //gets Actual Token, fired from the LoginPageRenderer
            _Token = token;

        }

        private void LoginPage_LoginCancelled(object sender, EventArgs e)
        {
            //if login cancelled, user will be redirected back to the sign-in page
            MainPage = new SignIn();
        }

        private async void LoginPage_LoginSucceeded(object sender, EventArgs e)
        {
            await storeToken();
            MainPage = coreView;
            httpClient = new HttpClient(new NativeMessageHandler());

            var facebookUri = new Uri("https://graph.facebook.com/v2.3/me?access_token="+StoredToken);
            //Test token
            //CAAJQNaDSB60BAG72FzPUHzMmPwMtFcodg14U6rBsySwVKpLykYQuAdqSgXbCCUTX4ZAiFOVfaild5C9G7hHv0vnBHDOHw95HdY5yIMoLelXYKLXqomvn7oEwnNhJJkvuPGW87n9bW5ZCkJCsJd0pbps4lhZBC6lqgZC0VZAsU30SDdGmiZBLdqG0V1KiAf7K4jsZBMm5z3clCB8i9QWoCiI

            var response = await httpClient.GetAsync(facebookUri);

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(content);
                
            }
            
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

