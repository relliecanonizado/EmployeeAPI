using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using EmployeeApiCore.Core.BaseClass;
using EmployeeApiCore.Core.Object;

namespace EmployeeApiCore.Core.Class
{
	public static class Extension
	{
		public static T DeepCopy<T>(this T item)
		{

			var formatter = new BinaryFormatter();
			var stream = new System.IO.MemoryStream();
			T result;

			formatter.Serialize(stream, item);
			stream.Seek(0, SeekOrigin.Begin);

			result = (T)(formatter.Deserialize(stream));
			stream.Close();
			return result;

		}

		public static string EscapeString(this string data)
		{

			if(data == null)
				return string.Empty;

			//data = data.Replace(@"\\", "\\\\");
			data = data.Replace(@"'", "\\'");
			//data = data.Replace(@"\", string.Empty);

			return data.Trim();

		}

		public static string FormatSQLDate(this System.DateTime? date)
		{

			if(date == null)
				return "NULL";

			return "'" + date.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";

		}

		public static string FormatSQLDate(this string date)
		{

			if(string.IsNullOrEmpty(date))
				return "NULL";

			return "'" + string.Format("yyyy-M-dd HH:mm:ss", date) + "'";

		}

		public static string ToSQLString(this string value)
		{

			if(string.IsNullOrEmpty(value))
				return "NULL";

			if(value == string.Empty)
				return "'" + string.Empty + "'";

			//value = value.Replace("\\", "\\\\");
			value = value.Replace("'", "''");

			//value = value.Replace(@"\\", "\\\\");
			//value = value.Replace(@"'", "\\'");
			//value = value.Replace(@"\", string.Empty);

			return "N'" + value.Trim() + "'";

		}

		public static string ToSQLString(this int value)
		{
			return value.ToString();
		}

		public static string ToSQLString(this double value)
		{
			return value.ToString();
		}

		public static string ToSQLString(this long value)
		{
			return value.ToString();
		}

		public static string ToSQLString(this decimal value)
		{
			return value.ToString();
		}

		public static string ToSQLString(this float value)
		{
			return value.ToString();
		}

		public static string ToSQLString(this System.DateTime? value)
		{

			if(value == null)
				return "NULL";

			return "'" + value.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";

		}

		public static string ToSQLString(this System.DateTime value)
		{

			if(value == null)
				return "NULL";

			return "'" + value.ToString("yyyy-MM-dd HH:mm:ss") + "'";

		}

		public static string ToSQLField(this QueryArg queryArgs, Type type)
		{
			string fields = string.Empty;
			PropertyInfo propertyInfo;
			Attribute[] attributes;

			foreach(var item in queryArgs.Fields)
			{
				propertyInfo = type.GetProperty(item, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				if(propertyInfo != null && propertyInfo.CanWrite)
				{
					attributes = Attribute.GetCustomAttributes(propertyInfo);

					if(attributes != null && attributes.Length > 0)
					{
						if(attributes[0] is System.Xml.Serialization.XmlElementAttribute)
						{
							fields += (attributes[0] as System.Xml.Serialization.XmlElementAttribute).ElementName + ",";
						}
						else
						{
							fields += (attributes[1] as System.Xml.Serialization.XmlElementAttribute).ElementName + ",";
						}
					}
				}
			}

			if(fields == string.Empty)
				return "*";
			else
				return fields.TrimEnd(',');

		}

		public static string ToSQLCondition(this FilterCollection filterCollection, Type type)
		{
			string condition = filterCollection.SearchOperator == SearchOperator.OR ? " WHERE 1=0 " : " WHERE 1=1 ";
			PropertyInfo propertyInfo;
			Attribute[] attributes;
			string field = string.Empty;


			if(filterCollection != null && filterCollection.Count > 0)
			{
				foreach(var item in filterCollection)
				{
					propertyInfo = type.GetProperty(((FilterModel)item).Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

					if(propertyInfo != null && propertyInfo.CanWrite)
					{

						attributes = Attribute.GetCustomAttributes(propertyInfo);

						if(attributes != null && attributes.Length > 0)
						{
							if(attributes[0] is XmlElementAttribute)
							{
								field = (attributes[0] as XmlElementAttribute).ElementName;
							}
							else
							{
								field = (attributes[1] as XmlElementAttribute).ElementName;
							}

							if(field != string.Empty)
							{
								if(filterCollection.SearchOperator == SearchOperator.OR)
								{
									condition += " OR " + field + item.ToSQLFilterFormat(propertyInfo);
								}
								else
								{
									condition += " AND " + field + item.ToSQLFilterFormat(propertyInfo);
								}
							}
						}
					}
				}
			}

			return condition;
		}

		public static string ToSQLFilterFormat(this FilterModel filterModel, PropertyInfo propertyInfo)
		{

			switch(Type.GetTypeCode(propertyInfo.PropertyType))
			{
				case TypeCode.String:

					switch(filterModel.Condition)
					{
						case SearchCondition.Equals		: return " = "		+ filterModel.Value.ToSQLString();
						case SearchCondition.NotEqual	: return " <> "		+ filterModel.Value.ToSQLString();
						case SearchCondition.StartsWith	: return " LIKE '"	+ filterModel.Value.EscapeString() + "%'";
						case SearchCondition.EndsWith	: return " LIKE '%" + filterModel.Value.EscapeString() + "'";
						case SearchCondition.Contains	: return " LIKE '%" + filterModel.Value.EscapeString() + "%'";
						default:
							return " = " + filterModel.Value.ToSQLString();
					}

				case TypeCode.Int32:

					int value = -1;

					if(filterModel.Value.IsNumeric())
						value = int.Parse(filterModel.Value);
					else
						return " > 0";


					switch(filterModel.Condition)
					{
						case SearchCondition.Equals				: return " = "	+ value.ToSQLString();
						case SearchCondition.NotEqual			: return " <> " + value.ToSQLString();
						case SearchCondition.GreaterThan		: return " > "	+ value.ToSQLString();
						case SearchCondition.LessThan			: return " < "	+ value.ToSQLString();
						case SearchCondition.GreaterThanOrEqual	: return " >= " + value.ToSQLString();
						case SearchCondition.LesserThanOrEqual	: return " <= " + value.ToSQLString();
						default:
							return " = " + value.ToSQLString();
					}

				case TypeCode.DateTime:
				
					switch(filterModel.Condition)
					{
						case SearchCondition.Equals				: return " = "	+ filterModel.Value.ToSQLString();
						case SearchCondition.NotEqual			: return " <> " + filterModel.Value.ToSQLString();
						case SearchCondition.GreaterThan		: return " > "	+ filterModel.Value.ToSQLString();
						case SearchCondition.LessThan			: return " < "	+ filterModel.Value.ToSQLString();
						case SearchCondition.GreaterThanOrEqual	: return " >= " + filterModel.Value.ToSQLString();
						case SearchCondition.LesserThanOrEqual	: return " <= " + filterModel.Value.ToSQLString();
						default:
							return " = " + filterModel.Value.ToSQLString();
					}

				case TypeCode.Object:

					switch(filterModel.Condition)
					{
						case SearchCondition.Equals		: return " = "		+ filterModel.Value.ToSQLString();
						case SearchCondition.NotEqual	: return " <> "		+ filterModel.Value.ToSQLString();
						case SearchCondition.StartsWith	: return " LIKE '"	+ filterModel.Value.EscapeString() + "%'";
						case SearchCondition.EndsWith	: return " LIKE '%" + filterModel.Value.EscapeString() + "'";
						case SearchCondition.Contains	: return " LIKE '%" + filterModel.Value.EscapeString() + "%'";
						default:
							return " = " + filterModel.Value.ToSQLString();
					}


				default:
					return string.Empty;
			}

		}

		public static SearchCondition ToSQLFilterFormat(this string condition)
		{

			switch(condition.ToLower())
			{
				case "equals"		: return SearchCondition.Equals;
				case "startswith"	: return SearchCondition.StartsWith;
				case "endswith"		: return SearchCondition.EndsWith;
				case "contains"		: return SearchCondition.Contains;
				case "notequals"	: return SearchCondition.NotEqual;
				case "gt"			: return SearchCondition.GreaterThan;
				case "lt"			: return SearchCondition.LessThan;
				case "gte"			: return SearchCondition.GreaterThanOrEqual;
				case "lte"			: return SearchCondition.LesserThanOrEqual;
				default:
					return SearchCondition.Equals;
			}
		
		}

		public static string ToSQLOrderBy(this QueryArg queryArgs, Type type)
		{

			StringBuilder results = new StringBuilder();
			PropertyInfo propertyInfo;
			System.Attribute[] attributes;
			List<string> sort = queryArgs.Sort;
			string field;
			string sortItem;

			results.Append(" ORDER BY ");

			foreach(var item in sort)
			{

				sortItem = item.StartsWith("-") ? item.Replace("-", "") : item;

				propertyInfo = type.GetProperty(sortItem, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				if(propertyInfo != null && propertyInfo.CanWrite)
				{
					attributes = Attribute.GetCustomAttributes(propertyInfo);

					if(attributes != null && attributes.Length > 0)
					{
						if(attributes[0] is System.Xml.Serialization.XmlElementAttribute)
						{
							field = (attributes[0] as System.Xml.Serialization.XmlElementAttribute).ElementName;
						}
						else
						{
							field = (attributes[1] as System.Xml.Serialization.XmlElementAttribute).ElementName;
						}

						if(field != string.Empty)
						{
							results.Append(" " + field);
							results.Append(item.StartsWith("-") ? " DESC," : ",");
						}
					}
				}
			}

			results.Length--;

			if(results.ToString().Trim() == "ORDER BY")
			{
				return " ORDER BY 1 ";
			}

			return results.ToString();

		}

		public static string ToSQLString(this object value, System.Type type)
		{

			if(value == null || type == typeof(string) && string.IsNullOrEmpty(value.ToString()))
				return "NULL";

			try
			{
				switch(System.Type.GetTypeCode(type))
				{
					case TypeCode.DateTime:
						return ToSQLString(System.DateTime.Parse(value.ToString()));
					case TypeCode.Double:
						return ToSQLString(double.Parse(value.ToString()));
					case TypeCode.Int32:
						return ToSQLString(int.Parse(value.ToString()));
					case TypeCode.Int64:
						return ToSQLString(long.Parse(value.ToString()));
					case TypeCode.Single:
						return ToSQLString(float.Parse(value.ToString()));
					case TypeCode.String:
						return value.ToSQLString();
					case TypeCode.Object:

						if(type == typeof(System.DateTime?))
							return ((System.DateTime?)value).ToSQLString();

						if(type == typeof(bool?))
							return ((bool?)value).ToSQLString();

						if(type == typeof(int?))
							return ((int?)value).ToSQLString();

						if(type == typeof(double?))
							return ((double?)value).ToSQLString();

						return "NULL";

					default:
						return "NULL";

				}
			}
			catch
			{
				return "NULL";
			}

		}

		public static string ToSQLString(this object value)
		{

			if(value == null || value.ToString() == string.Empty)
				return "NULL";

			try
			{
				switch(System.Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.DateTime:
						return ToSQLString((System.DateTime)value);
					case TypeCode.Double:
						return ToSQLString((double)value);
					case TypeCode.Int32:
						return ToSQLString((int)value);
					case TypeCode.Int64:
						return ToSQLString((long)value);
					case TypeCode.Single:
						return ToSQLString((float)value);
					case TypeCode.String:
						return ToSQLString(value.ToString());
					default:
						return "NULL";
				}

			}
			catch
			{
				return "NULL";
			}

		}

		/// <summary>
		/// This function gets the element name attribute of the property
		/// Returned results will depend on the System.Xml.Serialization.XmlElementAttribute attribute that is declared on the model's property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToElementName(this string propertyName, System.Type type)
		{

			PropertyInfo propertyInfo;
			System.Attribute[] attributes;
			string elementName = string.Empty;

			propertyInfo = type.GetProperty(propertyName);

			if(propertyInfo != null && propertyInfo.CanWrite)
			{
				attributes = System.Attribute.GetCustomAttributes(propertyInfo);

				if(attributes != null && attributes.Length > 0)
				{
					elementName = (attributes[0] as System.Xml.Serialization.XmlElementAttribute).ElementName;
				}
			}

			return elementName;

		}

		public static dynamic ToCollection(this IEnumerable collection, System.Type type)
		{

			dynamic result = System.Activator.CreateInstance(type);

			if(result != null && collection != null)
			{
				foreach(var item in collection)
				{
					dynamic convertedItem = System.Activator.CreateInstance(collection.GetType().GetGenericArguments()[0]);

					convertedItem = item;

					result.Add(convertedItem);
				}
			}

			return result;

		}

		public static dynamic ToCollection<T>(this T[] array, System.Type type)
		{

			dynamic result = System.Activator.CreateInstance(type);

			if(result != null)
			{
				if(array != null)
				{
					foreach(var item in array)
					{
						dynamic convertedItem = System.Activator.CreateInstance(array.GetType().GetGenericArguments()[0]);

						convertedItem = item;

						result.add(convertedItem);
					}
				}
			}

			return result;

		}

		public static dynamic ToModel(this BaseModel model, System.Type type)
		{

			dynamic result = System.Activator.CreateInstance(type);

			foreach(PropertyInfo propertyInfo in type.GetProperties())
			{
				if(propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(result, propertyInfo.GetValue(model, null), null);
				}
			}

			return result;

		}

		public static void Add<T>(ref T[] array, T item)
		{

			Array.Resize(ref array, array.Length + 1);

			array[array.Length - 1] = item;

		}

		public static string RemoveWhitespace(this string value)
		{
			return new string(value.Where(x => !char.IsWhiteSpace(x)).ToArray());
		}

		public static bool IsNumeric(this string value)
		{

			try
			{
				double number;
				return double.TryParse(value, out number);
			}
			catch
			{
				return false;
			}

		}

		public static bool IsNumeric(this object value)
		{

			try
			{
				double number;
				return double.TryParse(value.ToString(), out number);
			}
			catch(Exception)
			{
				return false;
			}

		}

		public static string ToRoleId(this Global.Role role)
		{
			switch(role)
			{									
				case Global.Role.CCSAdmin			: return "R001";
				case Global.Role.CCSBackOffice		: return "R002";
				case Global.Role.ClientAdmin		: return "R003";					
				case Global.Role.ClientManager		: return "R004";					
				case Global.Role.ClientTechnician	: return "R005";					
				case Global.Role.ClientBackOffice	: return "R006";					
				default								: return string.Empty;					
			}
		}

		public static Global.Role ToRole(this string roleId)
		{
			switch(roleId.ToLower())
			{
				case "r001"	: return Global.Role.CCSAdmin;
				case "r002"	: return Global.Role.CCSBackOffice;
				case "r003"	: return Global.Role.ClientAdmin;
				case "r004"	: return Global.Role.ClientManager;
				case "r005"	: return Global.Role.ClientTechnician;
				case "r006"	: return Global.Role.ClientBackOffice;
				default		: return Global.Role.Undefined;
			}
		}

		public static bool IsNullOrEmpty(this string value)
		{

			if(string.IsNullOrEmpty(value))
				return true;

			if(value == string.Empty)
				return true;

			return false;

		}

		public static bool IsNullOrEmpty(this double? value)
		{
			return value == null;
		}

		public static bool IsNullOrEmpty(this int? value)
		{
			return value == null;
		}

		public static string ToStringDateTime(this System.DateTime value)
		{
			return value.Year.ToString() + value.Month.ToString() + value.Date.ToString();
		}



	}
}
