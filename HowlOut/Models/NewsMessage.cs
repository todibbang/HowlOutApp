using System;

namespace HowlOut
{
	public class NewsMessage
	{
		public string SenderID { get; set; }

		public string ReceiverID { get; set; }

		public string Message { get; set; }

		public Event ContentEvent { get; set; }
		public string ContentMessage { get; set; }




		public NewsMessage ()
		{
		}

		public enum MessageType {
			EventInvite,

		}
	}
}

