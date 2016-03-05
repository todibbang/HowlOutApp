using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using ModernHttpClient;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace HowlOut
{
	public partial class App : Application
	{
		public static CoreView coreView;
		public static Profile userProfile;

		private ProfileApiManager profileApiManager;
		private HttpClient httpClient;

        public interface ISaveAndLoad
        {
            //Needed to pull and save tokens
            void SaveText(string filename, string text);
            string LoadText(string filename);

        }

        public static string StoredToken;
        static string _Token;
		public static string StoredUserFacebookId;
		static string _UserFacebookId;
        static string _userFacebookName;

        public App ()
		{
           // coreView = new CoreView(new SearchEvent(), false);

            InitializeComponent();

			httpClient = new HttpClient(new NativeMessageHandler());
			profileApiManager = new ProfileApiManager (httpClient);

            //Eventsfired from the LoginPage to trigger actions here
            LoginPage.LoginSucceeded += LoginPage_LoginSucceeded;
            LoginPage.LoginCancelled += LoginPage_LoginCancelled;

            //This loads a user token if existent, or else it will load "null" 
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
			StoredUserFacebookId = DependencyService.Get<HowlOut.App.ISaveAndLoad> ().LoadText ("userFacebookId");

			System.Diagnostics.Debug.WriteLine ("STORED USER FACEBOOK ID");
			System.Diagnostics.Debug.WriteLine (StoredUserFacebookId);
            
			UtilityManager util = new UtilityManager ();
			util.updateLastKnownPosition ();

            if (!App.IsLoggedIn)
            {
				//MainPage = coreView;
				MainPage = new SignIn();
            }
            else
            {
				startProgram();
				coreView = new CoreView();
				MainPage = coreView;
				coreView.setContentView (null, "SearchEvent");
            }

		}

        public async Task storeToken()
        {
            //Writes a New Token upon authentication in the directory
            DependencyService.Get<ISaveAndLoad>().SaveText("token", Token);
			DependencyService.Get<ISaveAndLoad> ().SaveText ("userFacebookId", UserFacebookId);
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
			StoredUserFacebookId = DependencyService.Get<HowlOut.App.ISaveAndLoad> ().LoadText ("userFacebookId");
        }
			

        public static string Token
        {
            get { return _Token; }
        }

		public static string UserFacebookId
		{
			get { return _UserFacebookId; }
		}

        public static bool IsLoggedIn
        {
            get
            {
				//returns Boolean for Login
				if (!string.IsNullOrWhiteSpace (StoredToken) && !string.IsNullOrWhiteSpace (StoredUserFacebookId)) {
					return true;
				} 
				else 
				{
					return false;
				}
            }
        }

        public static void SetToken(string token)
        {
            //gets Actual Token, fired from the LoginPageRenderer
            _Token = token;

        }

		public static void SetUserFacebookId(string userFacebookId)
		{
			//gets Actual Token, fired from the LoginPageRenderer
			_UserFacebookId = userFacebookId;

		}

        public static void SetUserFacebookName(string userFacebookName)
        {
            //gets Actual Token, fired from the LoginPageRenderer
            _userFacebookName = userFacebookName;

        }

        private void LoginPage_LoginCancelled(object sender, EventArgs e)
        {
            //if login cancelled, user will be redirected back to the sign-in page
            MainPage = new SignIn();
        }

        private async void LoginPage_LoginSucceeded(object sender, EventArgs e)
        {

            await storeToken();
            
			Profile profile = new Profile { ProfileId = UserFacebookId, Name = _userFacebookName, Age = 0 };
			await profileApiManager.CreateProfile(profile);

			startProgram ();
			coreView = new CoreView();
			MainPage = coreView;
			coreView.setContentView (null, "SearchEvent");
        }

		private async void startProgram()
		{
			userProfile = await profileApiManager.GetProfileId (StoredUserFacebookId);
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

