using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApiCore.Core.Object
{

	#region Poll Request

	public class PollRequest
	{
		public string PollType { get; set; }
	}

	public class InitialisationPollRequest : PollRequest
	{
		public string MacAddress { get; set; }
		public string ProgramVersion { get; set; }
		public string BackgroundId { get; set; }
		public string AdvertisementId { get; set; }

	}

	public class NormalPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
	}

	public class ProgramPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
	}

	public class ConfigPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
	}

	public class BackgroundPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
		public string Index { get; set; }
	}

	public class AdvertisementPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
		public string Index { get; set; }
	}

	public class TransactionPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
		public dynamic EventData { get; set; }
	}

	public class EventPollRequest : PollRequest
	{
		public string ReaderId { get; set; }
		public dynamic EventData { get; set; }
	}

	#endregion

	#region Poll Message

	public class PollResponse
	{
		public int Response { get; set; }
	}

	public class InitialisationPollResponse : PollResponse
	{
		public string ReaderId { get; set; }
	}

	public class NormalPollResponse : PollResponse
	{
		public ulong Time { get; set; }
		public short UtcOffset { get; set; }
		public string NewData { get; set; }
	}
	
	public class ProgramPollResponse : PollResponse
	{
		public string ReaderId { get; set; }
		public string Program { get; set; }
	}

	public class ConfigPollResponse : PollResponse
	{
		public  long? DeviceTag { get; set; }
		public double? PlayPrice { get; set; }
		public int? DeviceInterface { get; set; }
		public int? PulseWidth { get; set; }
		public int? PulseToActuate { get; set; }
		public string EclipseFeature { get; set; }
		public int Rotation { get; set; }
		public int? PollTime { get; set; }
		public int? ZipRequired { get; set; }
	}

	public class BackgroundPollResponse : PollResponse
	{
		public string ReaderId { get; set; }
		public byte[] Background { get; set; }
	}
	
	public class AdvertisementPollResponse : PollResponse
	{
		public string ReaderId { get; set; }
		public byte[] Background { get; set; }
	}

	public class TransactionPollResponse : PollResponse
	{
		
	}

	public class EventPollResponse : PollResponse
	{

	}

	#endregion

}
