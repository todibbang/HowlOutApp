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
		private DataManager _dataManager;

		public static Color HowlOut = Color.FromHex("#ff4bc6b4");
		public static Color HowlOutFade = Color.FromHex("#504bc6b4");

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
				CrossLocalNotifications.Current.Show ("Notifications works!!", "Nice",99,DateTime.Now.AddSeconds(30));
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

		public static void selectButton(List<Button> buttons, Button selected)
		{
			foreach (Button b in buttons)
			{
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 16;
				b.TextColor = App.HowlOutFade;
			}
			selected.FontAttributes = FontAttributes.Bold;
			selected.FontSize = 18;
			selected.TextColor = App.HowlOut;
		}

		public static void setOptionsGrid(Grid buttonGrid, List<Button> buttons, List<VisualElement> grids)
		{
			grids[0].IsVisible = true;
			selectButton(buttons, buttons[0]);

			int bNumber = 0;

			for (int i = 0; i < (buttons.Count * 2 - 1); i++)
			{
				if (i % 2 == 0)
				{
					buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				}
				else {
					buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
				}

				if (i == (buttons.Count * 2 - 1) - 1)
				{
					buttonGrid.Children.Add(new Button() { BorderColor = HowlOut, BorderWidth = 0.5, BorderRadius = 10 }, 0, i + 1, 0, 1);
				}
			}

			for (int i = 0; i < (buttons.Count * 2 - 1); i++)
			{
				if (i % 2 == 0)
				{
					buttonGrid.Children.Add(buttons[bNumber], i, 0);
					bNumber++;
				}
				else {
					buttonGrid.Children.Add(new StackLayout() { WidthRequest = 1, BackgroundColor = HowlOut }, i, 0);
				}
			}


			foreach (Button b in buttons)
			{
				b.Clicked += (sender, e) =>
				{
					selectButton(buttons, b);
					foreach (VisualElement g in grids)
					{
						g.IsVisible = false;
					}
					grids[buttons.IndexOf(b)].IsVisible = true;
					scrollTo(b);
				};
			}

		}

		public static async Task scrollTo(VisualElement a)
		{
			await Task.Delay(40);
			var y = a.Y;
			var parent = a.ParentView;
			while (parent != null)
			{
				y += parent.Y;
				parent = parent.ParentView;
			}

			coreView.scrollViews[coreView.scrollViews.Count - 1].ScrollToAsync(0, (y - 120), true);

			//s.ScrollToAsync(s.X, (y - 100), true);
		}

		public static async Task scrollTo(double y)
		{
			await Task.Delay(40);
			coreView.scrollViews[coreView.scrollViews.Count - 1].ScrollToAsync(0, (y - 120), true);
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

