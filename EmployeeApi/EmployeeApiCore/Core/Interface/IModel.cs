using System.Collections.Generic;

namespace EmployeeApiCore.Core.Interface
{
	public interface IModel
	{
		[System.Xml.Serialization.XmlIgnore]
		KeyValuePair<string, string> PrimaryKey { get; }
	}
}
