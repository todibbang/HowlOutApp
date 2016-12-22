using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using ModernHttpClient;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using Plugin.LocalNotifications;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace HowlOut
{
	public partial class App : Application
	{
		public static CoreView coreView;
		public static NotificationController notificationController = new NotificationController();
		public static Profile userProfile;
		public static Position lastKnownPosition = new Position(55.5, 12.6);
		DataManager _dataManager;

		public static Color HowlOut = Color.FromHex("#ff4bc6b4");
		public static Color HowlOutFade = Color.FromHex("#ffa9e4db");
		public static Color HowlOutBackground = Color.FromHex("#fff2f2f2");
		public static Color HowlOutRed = Color.FromHex("#ffe85151");
		public static Color LineColor = Color.FromHex("#ffb8b8b8");
		public static Color PlaceHolderColor = Color.FromHex("#ffd6d6d6");
		public static Color NormalTextColor = Color.FromHex("#ff707070");

		public static Action<string> PostSuccessFacebookAction { get; set; }

		public static string serverUri = "https://api.howlout.net/";

		public interface ISaveAndLoad
        {
            //Needed to pull and save tokens
            void SaveText(string filename, string text);
            string LoadText(string filename);

        }

        public static string StoredToken;
		public static string StoredUserFacebookId;
		public static string StoredApiKey;
        static string StoredUserFacebookName;

		public static NotificationToken StoredNotificationToken = new NotificationToken();

        public App ()
		{
            InitializeComponent();


			//Eventsfired from the LoginPage to trigger actions here
			LoginPage.FacebookLoginSucceeded += LoginPage_LoginSucceeded;
            LoginPage.LoginCancelled += LoginPage_LoginCancelled;
			LoginPage.HowlOutLoginAttempted += SetHowlOutLogin;

            //This loads a user token if existent, or else it will load "null" 
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
			StoredUserFacebookId = DependencyService.Get<HowlOut.App.ISaveAndLoad> ().LoadText ("userFacebookId");
			StoredApiKey = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("StoredApiKey");

			System.Diagnostics.Debug.WriteLine ("STORED FACEBOOK ID");
			System.Diagnostics.Debug.WriteLine (StoredUserFacebookId);
            
			_dataManager = new DataManager();
			_dataManager.UtilityManager.updateLastKnownPosition ();

            if (!App.IsLoggedIn)
            {
				MainPage = new SignIn();
            }
            else
            {
				//CrossLocalNotifications.Current.Show ("Notifications works!!", "Nice",99,DateTime.Now.AddSeconds(30));
				coreView = new CoreView();
				MainPage = coreView;
				startProgram(coreView);
            }
		}



		public static async Task storeToken(string token, string id, string name)
        {
            //Writes a New Token upon authentication in the directory
            DependencyService.Get<ISaveAndLoad>().SaveText("token", token);
			DependencyService.Get<ISaveAndLoad> ().SaveText ("userFacebookId", id);
            StoredToken = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("token");
			StoredUserFacebookId = DependencyService.Get<HowlOut.App.ISaveAndLoad> ().LoadText ("userFacebookId");
			StoredUserFacebookName = name;
        }

		public static async Task storeApiKey(string key)
		{
			DependencyService.Get<ISaveAndLoad>().SaveText("StoredApiKey", key);
			StoredApiKey = DependencyService.Get<HowlOut.App.ISaveAndLoad>().LoadText("StoredApiKey");
		}
			
		public static bool IsLoggedIn {
            get 
			{
				if (!string.IsNullOrWhiteSpace (StoredToken) && !string.IsNullOrWhiteSpace (StoredUserFacebookId)) {
					return true;
				} 
				else 
				{
					return false;
				}
            }
        }

		public static void SetUserFacebookId(string userFacebookId)
		{
			//gets Actual Token, fired from the LoginPageRenderer
			StoredUserFacebookId = userFacebookId;

		}

		private void SetHowlOutLogin(object sender, EventArgs e)
		{
			MainPage = new HowlOutLogin();
		}

        private void LoginPage_LoginCancelled(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Blytka 5");
            MainPage = new SignIn();
			System.Diagnostics.Debug.WriteLine("Blytka 6");
        }

		private async void LoginPage_LoginSucceeded(object sender, EventArgs e)
        {

           // await storeToken();

			var success = false;
			int tries = 0;

			while (!success)
			{

				Profile profile = new Profile() { ProfileId = StoredUserFacebookId, Name = StoredUserFacebookName, Age = 0, 
					SmallImageSource = "https://graph.facebook.com/v2.5/" + StoredUserFacebookId + "/picture?height=500&width=50",
					ImageSource = "https://graph.facebook.com/v2.5/" + StoredUserFacebookId + "/picture?height=100&width=100",
					LargeImageSource = "https://graph.facebook.com/v2.5/" + StoredUserFacebookId + "/picture?height=300&width=300"
				};
				success = await _dataManager.ProfileApiManager.CreateUpdateProfile(profile, true);

			}
			_dataManager = new DataManager();
			success = false;
			while (!success)
			{
				success = await _dataManager.ProfileApiManager.RegisterForNotifications(StoredNotificationToken);
				tries++;
				if (tries > 5) break;
			}


			coreView = new CoreView();
			coreView.token = StoredNotificationToken.DeviceToken + ", " + success;
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
			userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile ();
			coreView.startCoreView ();
			//coreView.setContentView (new EventView(), "Event");

		}

		public static async void UpdateLiveConversations()
		{
			if(coreView.viewdConversation != null)coreView.viewdConversation.conversation = await coreView._dataManager.MessageApiManager.GetOneConversation(coreView.viewdConversation.ConversationId);
			await coreView.profileConversatios.UpdateConversations(true);
			await coreView.otherConversatios.UpdateConversations(true);
			foreach (ConversationView cv in coreView.activeConversationViews)
			{
				cv.UpdateList(true);
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

