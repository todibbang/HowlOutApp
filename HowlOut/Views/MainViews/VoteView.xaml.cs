using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class VoteView : ContentView, ViewModelInterface
	{
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();

			if (!Edit)
			{
				ub.setRightButton("ic_more_vert_white.png").Clicked += async (sender, e) =>
				{
					List<Action> actions = new List<Action>();
					List<string> titles = new List<string>();
					List<string> images = new List<string>();

					actions.Add(() =>
					{
						//App.coreView.setContentViewWithQueue(new VoteView(VoteItems, true));
						Edit = true;
						reloadView();
					});
					titles.Add("Edit");
					images.Add("ic_settings.png");
					await App.coreView.DisplayOptions(actions, titles, images, optiongrid);
				};
			}
			return ub;
		}
		public void reloadView() { this.Content = new VoteView(VoteItems, Edit); }
		public ContentView getContentView() { return this; }
		public List<VoteItem> VoteItems;
		bool Edit;

		public VoteView(List<VoteItem> voteItems, bool edit)
		{
			InitializeComponent();
			Edit = edit;

			var tap = new TapGestureRecognizer();
			tap.Tapped += (sender, e) =>
			{
				VoteItems.Add(new VoteItem() { });
				Edit = true;
				reloadView();
				//App.coreView.setContentViewReplaceCurrent(new VoteView(VoteItems, true));
			};
			addNewIcon.GestureRecognizers.Add(tap);


			if (voteItems == null)
			{
				voteItems = new List<VoteItem>();
				voteItems.Add(new VoteItem() { VoteDescription = "Test mulighed nr 1.", });
				voteItems.Add(new VoteItem() { VoteDescription = "Test mulighed Test mulighed nr 2.", });
				voteItems.Add(new VoteItem() { VoteDescription = "Test mulighed Test mulighed Test mulighed Test mulighed Test mulighednr 3.", });
				voteItems.Add(new VoteItem() { VoteDescription = "Test mulighed Test mulighed Test mulighed Test mulighed Test mulighednr Test mulighed Test mulighed Test mulighed Test mulighed Test mulighednr 4.", });
			}
			VoteItems = voteItems;

			if (edit)
			{
				VoteEditScrollView.IsVisible = true;
				foreach (VoteItem vi in voteItems)
				{
					var editVoteItem = new VoteItemEditTemplate() { BindingContext = vi };
					VoteEditList.Children.Add(editVoteItem);
					editVoteItem.deleteTapped.Tapped += (sender, e) =>
					{
						VoteItems.Remove(vi);
						reloadView();
						//App.coreView.setContentViewReplaceCurrent(new VoteView(VoteItems, true));
					};
				}
				VoteEditList.Children.Add(addNewLayout);

				updateBtn.IsVisible = true;
				updateBtn.Clicked += (sender, e) =>
				{
					//App.coreView.setContentViewReplaceCurrent(new VoteView(VoteItems, false));
					Edit = false;
					reloadView();
				};
				scrollToBottom();
			}
			else {
				
				VoteList.ItemsSource = voteItems;
				footerLayout.Children.Add(addNewLayout);
			}

		}
		async void scrollToBottom()
		{
			await Task.Delay(100);
			VoteEditScrollView.ScrollToAsync(addNewLayout, ScrollToPosition.MakeVisible, true);
		}
	}
}
