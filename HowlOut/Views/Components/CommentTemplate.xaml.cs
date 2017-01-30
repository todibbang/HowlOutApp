using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CommentTemplate : ContentView
	{
		public CommentTemplate()
		{
			InitializeComponent();
		}
		public CommentTemplate(Comment comment)
		{
			InitializeComponent();
			BindingContext = comment;
		}
	}
}

