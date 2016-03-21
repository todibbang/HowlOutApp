using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using ModernHttpClient;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using Plugin.LocalNotifications.Abstractions;
using Plugin.LocalNotifications;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace HowlOut
{
	public partial class App : Application
	{
		public static CoreView coreView;
		public static Profile userProfile;
		public static Position lastKnownPosition = new Position(55.5, 12.6);
		private DataManager _dataManager;

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
			_dataManager = new DataManager ();

			//Eventsfired from the LoginPage to trigger actions here
            LoginPage.LoginSucceeded += LoginPage_LoginSucceeded;
            LoginPage.LoginCancelled += LoginPage_LoginCancelled;

            //This loads a user token if existent, or else it will load "null" 
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
			StoredUserFacebookId = DependencyService.Get<HowlOut.App.ISaveAndLoad> ().LoadText ("userFacebookId");

			System.Diagnostics.Debug.WriteLine ("STORED USER FACEBOOK ID");
			System.Diagnostics.Debug.WriteLine (StoredUserFacebookId);
            
			_dataManager.UtilityManager.updateLastKnownPosition ();

            if (!App.IsLoggedIn)
            {
				MainPage = new SignIn();
            }
            else
            {
				CrossLocalNotifications.Current.Show ("Hej med dig", "Dette er cool", 1, DateTime.Now.AddMinutes(1));
				coreView = new CoreView();
				MainPage = coreView;
				startProgram(coreView);
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
            
			Profile profile = new Profile (){ ProfileId = UserFacebookId, Name = _userFacebookName, Age = 0 };
			await _dataManager.ProfileApiManager.CreateProfile(profile);

			coreView = new CoreView();
			MainPage = coreView;
			startProgram(coreView);
			/*
			coreView = new CoreView();
			MainPage = coreView;
			coreView.setContentView (null, "SearchEvent");
			*/
        }

		private async Task startProgram(CoreView coreView)
		{
			userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile (StoredUserFacebookId);
			coreView.startCoreView ();
			//coreView.setContentView (new EventView(), "Event");

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

