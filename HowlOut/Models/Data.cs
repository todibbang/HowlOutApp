using System;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public class Data
	{
		public string id {get; set;} 
		public string vejnavn {get; set;} 
		public string husnr {get; set;} 
		public string postnr {get; set;} 
		public string postnrnavn {get; set;} 

		public string href {get; set;}

		public Position position {get; set;} 

		public Data ()
		{
		}
	}
}

