﻿using System;  using Xamarin.Forms.Platform.Android; using Xamarin.Forms; using HowlOut; using HowlOut.Droid; using Android.Graphics.Drawables;  [assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))] namespace HowlOut.Droid { 	public class CustomEntryRenderer : EntryRenderer 	{ 		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e) 		{ 			base.OnElementChanged(e);  			if (Control != null)
			{ 				GradientDrawable gd = new GradientDrawable(); 				gd.SetStroke(5, Color.White.ToAndroid()); 				this.Control.SetBackgroundDrawable(gd); 				this.Control.Gravity = Android.Views.GravityFlags.CenterHorizontal; 				this.Control.SetHintTextColor(Color.White.ToAndroid()); 			} 		} 	} }