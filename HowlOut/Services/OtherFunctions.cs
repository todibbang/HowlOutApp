using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HowlOut
{ 
	public class OtherFunctions
	{
		public static EventHandler testEvent;

		public void selectButton(List<Button> buttons, Button selected)
		{
			foreach (Button b in buttons)
			{
				b.FontAttributes = FontAttributes.None;
				b.FontSize = 12;
				b.TextColor = Color.White;
			}
			selected.FontAttributes = FontAttributes.Bold;
			selected.FontSize = 12;
			selected.TextColor = App.HowlOut;

		}

		public void setOptionsGrid(Grid buttonGrid, List<string> buttonText, List<VisualElement> grids, List<Action> actions, CarouselView carousel)
		{
			buttonGrid.BackgroundColor = Color.FromHex("#cc000000");



			List<Button> buttons = new List<Button>();
			List<Action> clickButtonAction = new List<Action>();
			if (buttonText != null)
			{
				foreach (String s in buttonText)
				{
					Button b = new Button { Text = s, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, FontSize = 16 };
					buttons.Add(b);
					clickButtonAction.Add(() => selectButton(buttons, b));
				}
			}

			if (grids != null && grids[0] != null) grids[0].IsVisible = true;
			//if (actions != null && actions[0] != null) { actions[0].Invoke(); }
			selectButton(buttons, buttons[0]);

			int bNumber = 0;
			buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Absolute) });
			buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
			buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Absolute) });
			buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(62, GridUnitType.Absolute) });
			buttonGrid.RowSpacing = 0;

			StackLayout selectedBar = new StackLayout() { BackgroundColor = App.HowlOut };
			buttonGrid.Children.Add(selectedBar, 0, 0);

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
					buttonGrid.Children.Add(new Button() { BorderColor = App.LineColor, BorderWidth = 0.0, BorderRadius = 0, BackgroundColor = Color.Transparent }, 0, i + 1, 1, 2);
				}
			}

			for (int i = 0; i < (buttons.Count * 2 - 1); i++)
			{
				if (i % 2 == 0)
				{
					buttonGrid.Children.Add(buttons[bNumber], i, 1);
					bNumber++;
				}
				else {
					//buttonGrid.Children.Add(new StackLayout() { WidthRequest = 1, BackgroundColor = App.LineColor }, i, 0);
				}
			}

			if (carousel != null)
			{
				carousel.PositionSelected += (sender, e) =>
				{
					if (clickButtonAction[carousel.Position] != null) { 
						clickButtonAction[carousel.Position].Invoke(); 
					}
					buttonGrid.Children.Add(selectedBar, carousel.Position * 2, 0);
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
					//scrollTo(b);
				};
			}

		}
		/*
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
		*/


		public async Task<bool> SenderOfEvent(StackLayout SelectEventSenderLayout, Event eve)
		{
			bool continueCreating = true;

			if (App.userProfile.GroupsOwned != null && App.userProfile.GroupsOwned.Count > 0)
			{
				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				SelectEventSenderLayout.Children.Clear();
				SelectEventSenderLayout.IsVisible = true;

				SelectEventSenderLayout.Children.Add(new Label()
				{
					Text = "Who is the owner of this event?",
					FontSize = 16,
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				});

				Grid uGrid = new Grid() { HeightRequest = 120, WidthRequest = 120, Padding = new Thickness(0,0,0,10) };
				uGrid.Children.Add(new ProfileDesignView(App.userProfile,120,false, GenericDesignView.Design.OnlyImage),0,0);
				Button youBtn = new Button()
				{
					HeightRequest = 150,
					WidthRequest = 150,
				};
				uGrid.Children.Add(youBtn, 0, 0);
				SelectEventSenderLayout.Children.Add(uGrid);
				youBtn.Clicked += async (sender, e) =>
				{
					bool success = await App.coreView.displayConfirmMessage("Continue ?", "You have selected yourself to be the owner of this event. Continue creating the event ?", "Confirm", "Cancel");
					if(success)tcs.TrySetResult(true);
				};
				StackLayout labLayout = new StackLayout();
				ListsAndButtons lab = new ListsAndButtons();
				labLayout.Children.Add(lab);
				SelectEventSenderLayout.Children.Add(labLayout);

				Button cancleBtn = new Button()
				{
					Text = "Cancel",
					HeightRequest = 30,
					WidthRequest = 100,
					FontSize = 14,
					TextColor = Color.White,
					BorderRadius = 10,
					BackgroundColor = App.HowlOutRed,
					VerticalOptions = LayoutOptions.EndAndExpand
				};
				SelectEventSenderLayout.Children.Add(cancleBtn);
				getGroupClicked(lab, eve);

				cancleBtn.Clicked += (sender, e) =>
				{
					continueCreating = false;
					tcs.TrySetResult(true);
				};
				testEvent += async (sender, e) =>
				{
					bool success = await App.coreView.displayConfirmMessage("Continue ?", "Continue creating the event with the selected group as owner ?", "Confirm", "Cancel");
					if (success) tcs.TrySetResult(true);
					else {
						labLayout.Children.Clear();
						labLayout.Children.Add(lab = new ListsAndButtons());
						getGroupClicked(lab, eve);
					}
				};
				await tcs.Task;
				lab = null;
				SelectEventSenderLayout.IsVisible = false;
			}
			return continueCreating;
		}

		private async Task getGroupClicked(ListsAndButtons lab, Event eve)
		{
			eve.GroupOwner = await lab.createList(null, App.userProfile.GroupsOwned, null, null, false, false, true);
			EventArgs e = new EventArgs();
			testEvent(null, e);
		}

		/*
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
		} */

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
			/*
			await Task.Delay(40);
			if (App.coreView.scrollViews[App.coreView.scrollViews.Count - 1] != null)
			{
				App.coreView.scrollViews[App.coreView.scrollViews.Count - 1].ScrollToAsync(0, (y - 120), true);
			}
			*/
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


			CarouselList cl = new CarouselList(ve, st, CarouselList.ViewType.Other );

			App.coreView.setContentViewWithQueue(cl);
		}
	}
}
