﻿using System;
using Xamarin.Forms;
namespace HowlOut
{
	public class ConversationEditor : Editor
	{
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create<CustomEditor, string>(view => view.Placeholder, String.Empty);
		
		public ConversationEditor()
		{
		}

		public string Placeholder
		{
			get
			{
				return (string)GetValue(PlaceholderProperty);
			}

			set
			{
				SetValue(PlaceholderProperty, value);
			}
		}
	}
}
