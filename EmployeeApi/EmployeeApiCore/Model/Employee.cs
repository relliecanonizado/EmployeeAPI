using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EmployeeApiCore.Core.BaseClass;
using Newtonsoft.Json;

namespace EmployeeApiCore.Model
{
	namespace DataObject
	{
		public class EmployeeDataObject : BaseModel
		{

			#region Fields
			
			[XmlElementAttribute(ElementName = "FirstName")]
			[JsonProperty("FirstName")]
			public string FirstName
			{
				get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
				set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
			}

			[XmlElementAttribute(ElementName = "MiddleName")]
			[JsonProperty("MiddleName")]
			public string MiddleName
			{
				get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
				set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
			}

			[XmlElementAttribute(ElementName = "LastName")]
			[JsonProperty("LastName")]
			public string LastName
			{
				get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
				set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
			}
			
			#endregion

			#region PropertySpecified
			
			[XmlIgnore]
			[JsonIgnore]
			public bool FirstNameSpecified
			{
				get { return this.FirstName != null && this.FirstName != string.Empty; }
			}

			[XmlIgnore]
			[JsonIgnore]
			public bool MiddleNameSpecified
			{
				get { return this.FirstName != null && this.FirstName != string.Empty; }
			}

			[XmlIgnore]
			[JsonIgnore]
			public bool LastNameSpecified
			{
				get { return this.LastName != null && this.LastName != string.Empty; }
			}
			
			#endregion

			public override KeyValuePair<string, string> PrimaryKey
			{
				get { return new KeyValuePair<string, string>("Id", this.Id); }
			}
		}
	}

	namespace Model
	{
		public class Employee : DataObject.EmployeeDataObject
		{
			
		}
	}

	namespace Collection
	{
		public class EmployeeCollection : BaseModelCollection<Model.Employee>
		{

		}
	}
}
