using System.Collections.Generic;
using System.Xml.Serialization;
using EmployeeApiCore.Core.BaseClass;
using Newtonsoft.Json;

namespace EmployeeApiCore.Core.Object
{
	public class ColumnModel : BaseModel
	{

		[XmlElementAttribute(ElementName = "COLUMN_NAME")]
		[JsonProperty("COLUMN_NAME")]
		public string ColumnName { get; set; }

		[XmlElementAttribute(ElementName = "DATA_TYPE")]
		[JsonProperty("DATA_TYPE")]
		public string DataType { get; set; }

		[XmlElementAttribute(ElementName = "IS_NULLABLE")]
		[JsonProperty("IS_NULLABLE")]
		public string IsNullable { get; set; }

		[XmlElementAttribute(ElementName = "CONSTRAINT_TYPE")]
		[JsonProperty("CONSTRAINT_TYPE")]
		public string ConstraintType { get; set; }

		[XmlElementAttribute(ElementName = "IS_IDENTITY")]
		[JsonProperty("IS_IDENTITY")]
		public int IsIdentity { get; set; }

		[XmlIgnore]
		[JsonIgnore]
		public override KeyValuePair<string, string> PrimaryKey
		{
			get { return new KeyValuePair<string, string>("Field", this.ColumnName); }
		}
	
		public class ColumnCollection : BaseModelCollection<ColumnModel>
		{
			public string GetPrimaryKey()
			{
				string primaryKey = string.Empty;

				foreach(var item in this)
				{
					if(item.ConstraintType.ToLower() == "primary key")
						primaryKey += item.ColumnName + "|";
				}

				if(!string.IsNullOrEmpty(primaryKey))
					primaryKey = primaryKey.Remove(primaryKey.Length - 1, 1);

				return primaryKey;
			}

			public bool IsMultiPrimaryKey()
			{
				int count = 0;

				foreach(var item in this)
				{
					if(item.ConstraintType.ToLower() == "primary key")
						count += 1;

				}

				return count > 1;
			}
		}
	}
}
