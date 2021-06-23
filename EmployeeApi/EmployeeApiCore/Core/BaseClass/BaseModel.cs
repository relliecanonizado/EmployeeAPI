using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using EmployeeApiCore.Core.Interface;
using EmployeeApiCore.Core.Object;
using Newtonsoft.Json;

namespace EmployeeApiCore.Core.BaseClass
{	
	public abstract class BaseModel : BindableObject, IModel
	{

		[XmlElementAttribute(ElementName = "Id")]
		[JsonProperty("Id")]
		public string Id
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		[XmlElementAttribute(ElementName = "AddedBy")]
		[JsonProperty("AddedBy")]
		public string AddedBy
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		[XmlElementAttribute(ElementName = "ModifiedBy")]
		[JsonProperty("ModifiedBy")]
		public string ModifiedBy
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		[XmlElementAttribute(ElementName = "DateAdded")]
		[JsonProperty("DateAdded")]
		public DateTime? DateAdded
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		[XmlElementAttribute(ElementName = "DateModified")]
		[JsonProperty("DateModified")]
		public DateTime? DateModified
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}
		
		[XmlIgnore]
		[JsonIgnore]
		public dynamic Tag { get; set; }

		[XmlIgnore]
		[JsonIgnore]
		public abstract KeyValuePair<string, string> PrimaryKey { get; }

		#region PropertySpecified

		[XmlIgnore]
		[JsonIgnore]
		public bool IdSpecified
		{
			get { return this.Id != null && this.Id != string.Empty; }
		}

		[XmlIgnore]
		[JsonIgnore]
		public bool AddedBySpecified
		{
			get { return this.AddedBy != null && this.AddedBy != string.Empty; }
		}

		[XmlIgnore]
		[JsonIgnore]
		public bool ModifiedBySpecified
		{
			get { return this.ModifiedBy != null && this.ModifiedBy != string.Empty; }
		}

		[XmlIgnore]
		[JsonIgnore]
		public bool DateAddedSpecified
		{
			get { return this.DateAdded != null; }
		}

		[XmlIgnore]
		[JsonIgnore]
		public bool DateModifiedSpecified
		{
			get { return this.DateModified != null; }
		}

		#endregion
	}
	
}
