using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HowlOut
{ 
	public class OtherFunctions
	{
		

		public void selectButton(List<Button> buttons, Button selected)
		{
			foreach (Button b in buttons)
			{
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 16;
				b.TextColor = App.PlaceHolderColor;
			}
			selected.FontAttributes = FontAttributes.None;
			selected.FontSize = 18;
			selected.TextColor = App.HowlOut;

		}

		public void setOptionsGrid(Grid buttonGrid, List<String> buttonText, List<VisualElement> grids, List<Action> actions, CarouselView carousel)
		{
			List<Button> buttons = new List<Button>();
			List<Action> clickButtonAction = new List<Action>();
			foreach (String s in buttonText)
			{
				Button b = new Button { Text = s, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.PlaceHolderColor, FontSize = 16 };
				buttons.Add(b);
				clickButtonAction.Add(() => selectButton(buttons, b));
			}

			if (grids != null && grids[0] != null) grids[0].IsVisible = true;
			//if (actions != null && actions[0] != null) { actions[0].Invoke(); }
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
					buttonGrid.Children.Add(new Button() { BorderColor = App.LineColor, BorderWidth = 0.5, BorderRadius = 0, BackgroundColor = Color.White }, 0, i + 1, 0, 1);
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
					buttonGrid.Children.Add(new StackLayout() { WidthRequest = 1, BackgroundColor = App.LineColor }, i, 0);
				}
			}

			if (carousel != null)
			{
				carousel.PositionSelected += (sender, e) =>
				{
					if (clickButtonAction[carousel.Position] != null) { clickButtonAction[carousel.Position].Invoke(); }
				};
			}

			foreach (Button b in buttons)
			{
				b.Clicked += (sender, e) =>
				{
					//selectButton(buttons, b);
					if (grids != null)
					{
						foreach (VisualElement g in grids)
						{
							if (g != null) { g.IsVisible = false; }
						}
					}
					if (clickButtonAction[buttons.IndexOf(b)] != null) { clickButtonAction[buttons.IndexOf(b)].Invoke(); }
					if (grids != null && grids[buttons.IndexOf(b)] != null) { grids[buttons.IndexOf(b)].IsVisible = true; }
					if (actions != null && actions[buttons.IndexOf(b)] != null) { actions[buttons.IndexOf(b)].Invoke(); }
					scrollTo(b);
				};
			}

		}

		public async Task scrollTo(VisualElement a)
		{

			await Task.Delay(40);
			var y = a.Y;
			var parent = a.ParentView;
			while (parent != null)
			{
				y += parent.Y;
				parent = parent.ParentView;
			}
			if (App.coreView.scrollViews[App.coreView.scrollViews.Count - 1] != null)
			{
				App.coreView.scrollViews[App.coreView.scrollViews.Count - 1].ScrollToAsync(0, (y - 120), true);
			}
			//s.ScrollToAsync(s.X, (y - 100), true);
		}

		public async Task<bool> SenderOfEvent(StackLayout SelectEventSenderLayout, Event eve, Group grp)
		{
			bool continueCreating = true;

			if (App.userProfile.Organizations != null && App.userProfile.Organizations.Count > 0)
			{
				SelectEventSenderLayout.Children.Clear();
				SelectEventSenderLayout.Children.Add(new Label()
				{
					Text = "Who is the sender of this event?",
					TextColor = Color.White,
					FontSize = 16,
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				});

				List<Button> buttons = new List<Button>();
				organisationButton(App.userProfile.Name, buttons, SelectEventSenderLayout);

				foreach (Organization o in App.userProfile.Organizations)
				{
					organisationButton(o.Name, buttons, SelectEventSenderLayout);
				}
				organisationButton("Cancel", buttons, SelectEventSenderLayout);

				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				SelectEventSenderLayout.IsVisible = true;
				foreach (Button b in buttons)
				{
					b.Clicked += (sender, e) =>
					{


						if (b == buttons[0])
						{
							System.Diagnostics.Debug.WriteLine("You are the sender of the event");
						}
						else if (b == buttons[buttons.Count - 1])
						{
							System.Diagnostics.Debug.WriteLine("Cancel creating event");
							continueCreating = false;
						}
						else {
							System.Diagnostics.Debug.WriteLine("Sender of event is " + App.userProfile.Organizations[buttons.IndexOf(b) - 1].Name);
							if (eve != null) eve.OrganizationOwner = App.userProfile.Organizations[buttons.IndexOf(b) - 1];
							if (grp != null) grp.OrganizationOwner = App.userProfile.Organizations[buttons.IndexOf(b) - 1];
						}
						tcs.TrySetResult(true);
					};
				}

				await tcs.Task;
				SelectEventSenderLayout.IsVisible = false;
			}

			return continueCreating;
		}

		public async Task<bool> GroupEventIsFor(StackLayout SelectEventSenderLayout, Event newEvent)
		{
			bool continueCreating = false;

			// Dummy Data End

			if (App.userProfile.Groups != null && App.userProfile.Groups.Count > 0)
			{
				SelectEventSenderLayout.Children.Clear();
				SelectEventSenderLayout.Children.Add(new Label()
				{
					Text = "What Group is this event for?",
					TextColor = Color.White,
					FontSize = 16,
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				});

				List<Button> buttons = new List<Button>();

				foreach (Group o in App.userProfile.Groups)
				{
					organisationButton(o.Name, buttons, SelectEventSenderLayout);
				}
				organisationButton("Cancel", buttons, SelectEventSenderLayout);

				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				SelectEventSenderLayout.IsVisible = true;
				foreach (Button b in buttons)
				{
					b.Clicked += (sender, e) =>
					{

						if (b == buttons[buttons.Count - 1])
						{
							System.Diagnostics.Debug.WriteLine("Cancel creating event");
							continueCreating = false;
						}
						else {
							System.Diagnostics.Debug.WriteLine("Sender of event is " + App.userProfile.Groups[buttons.IndexOf(b)].Name);
							newEvent.GroupSpecific = App.userProfile.Groups[buttons.IndexOf(b)];
							continueCreating = true;
						}
						tcs.TrySetResult(true);
					};
				}

				await tcs.Task;
				SelectEventSenderLayout.IsVisible = false;
			}

			return continueCreating;
		}

		void organisationButton(String name, List<Button> buttons, StackLayout SelectEventSenderLayout)
		{
			Button oB = new Button()
			{
				Text = name,
				HeightRequest = 30,
				WidthRequest = 100,
				FontSize = 14,
				TextColor = App.HowlOut,
				BorderColor = App.HowlOut,
				BorderWidth = 1,
				BorderRadius = 10,
				BackgroundColor = Color.White,
			};
			buttons.Add(oB);
			SelectEventSenderLayout.Children.Add(oB);
		}

		public async Task scrollTo(double y)
		{
			await Task.Delay(40);
			if (App.coreView.scrollViews[App.coreView.scrollViews.Count - 1] != null)
			{
				App.coreView.scrollViews[App.coreView.scrollViews.Count - 1].ScrollToAsync(0, (y - 120), true);
			}
		}

		public void ViewImages(List<string> images)
		{
			List<VisualElement> ve = new List<VisualElement>();
			List<string> st = new List<string>();
			foreach (string s in images)
			{
				ve.Add(new ImageView(s));
				st.Add("Image");
			}


			CarouselList cl = new CarouselList( ve, st );

			App.coreView.setContentViewWithQueue(cl, "", null);
		}
	}
}
