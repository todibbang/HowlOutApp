using System;

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

		public double latitude {get; set;} 
		public double longitude {get; set;} 

		public string [] koordinater {get; set;} 

		public Data ()
		{
			koordinater = new string[2];
		}
	}
}

