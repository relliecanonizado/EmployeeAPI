using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EmployeeApiCore.Core.Object;

namespace EmployeeApiCore.Core.Object
{
	[Serializable]
	public class ConfigObject : BindableObject
	{
		[XmlElement(ElementName = "SocketServerIp")]
		public string SocketServerIp { get; set; }

		[XmlElement(ElementName = "SocketServerPort")]
		public string SocketServerPort { get; set; }

		[XmlElement(ElementName = "SocketListenMilliseconds")]
		public int SocketListenMilliseconds { get; set; }

	}
}
