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
		public static Profile userProfile;
		public static Position lastKnownPosition = new Position(55.5, 12.6);
		DataManager _dataManager;

		public static Color HowlOut = Color.FromHex("#ff4bc6b4");
		public static Color HowlOutFade = Color.FromHex("#504bc6b4");
		public static Color HowlOutBackground = Color.FromHex("#fff8f8f8");
		public static Color LineColor = Color.FromHex("#ffb8b8b8");
		public static Color PlaceHolderColor = Color.FromHex("#ffe6e6e6");
		public static Color NormalTextColor = Color.FromHex("#ff808080");

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
        static string StoredUserFacebookName;

		public static NotificationToken StoredNotificationToken = new NotificationToken();

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

			System.Diagnostics.Debug.WriteLine ("STORED FACEBOOK ID");
			System.Diagnostics.Debug.WriteLine (StoredUserFacebookId);
            
			_dataManager.UtilityManager.updateLastKnownPosition ();

            if (!App.IsLoggedIn)
            {
				MainPage = new SignIn();
            }
            else
            {
				CrossLocalNotifications.Current.Show ("Notifications works!!", "Nice",99,DateTime.Now.AddSeconds(30));
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

				Profile profile = new Profile() { ProfileId = StoredUserFacebookId, Name = StoredUserFacebookName, Age = 0, ImageSource = "https://graph.facebook.com/v2.5/" + StoredUserFacebookId + "/picture?height=200&width=200" };
				success = await _dataManager.ProfileApiManager.CreateUpdateProfile(profile, true);

			}
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
			List<Conversation> conversations = new List<Conversation>();
			coreView.notifications.UpdateNotifications(true);
			await coreView.conversatios.UpdateConversations();
			conversations = coreView.conversatios.conversations;
			foreach (ConversationView cv in coreView.activeConversationViews)
			{
				if (cv.type == MessageApiManager.CommentType.Converzation)
				{
					cv.conversation = conversations.Find(c => c.ConversationID == cv.ConversationId);
				}
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

