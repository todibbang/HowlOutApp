using System;
using System.Collections.Generic;
using Xamarin.Forms;
namespace HowlOut
{
	public class VoteItem
	{
		public string VoteDescription { get; set; }
		public List<Profile> ProfilesVotedYes { get; set; }
		public int OptionVoteCount { get {
				if (ProfilesVotedYes == null) return 0;
				return ProfilesVotedYes.Count; } }
		public Color BackgroundColor { get {
				if (ProfilesVotedYes != null && ProfilesVotedYes.Exists(p => p.ProfileId == App.userProfile.ProfileId))
					return App.HowlOutFade;
				return Color.Transparent;
			} 
		}
	}
}
