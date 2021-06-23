using System.Xml.Serialization;
using EmployeeApiCore.Core.BaseClass;

namespace EmployeeApiCore.Core.Interface
{
	public interface IModelCollection<T>
	{
		#region Properties

		[XmlIgnore]
		System.Guid Guid { get; set; }

		#endregion

		#region Function

		dynamic Clone();

		bool Exists(T value, string propertyName, bool skipSelf);

		#endregion

	}
}
