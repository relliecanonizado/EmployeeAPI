using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml.Serialization;
using EmployeeApiCore.Core.Class;
using EmployeeApiCore.Core.Interface;
using EmployeeApiCore.Core.Object;
using static EmployeeApiCore.Core.Object.ColumnModel;

namespace EmployeeApiCore.Core.BaseClass
{
	public abstract class BaseRepository<T> : Connection, IRepository<T> where T : BaseClass.BaseModel
	{

		#region Variables

		public SqlConnection		_connection;

		private PropertyInfo[]		_properties;
		private ColumnCollection	_columns;
		private Type				_type;
		private Guid				_guid;

		private string				_databaseName;
		private string				_tableName;

		#endregion

		#region Constructor

		public BaseRepository(string databaseName) : base()
		{

			_columns		= null;
			_guid			= Guid.NewGuid();
			_type			= typeof(T);
			_databaseName	= databaseName;
			_properties		= _type.GetProperties();

		}

		#endregion

		#region Properties

		public ColumnCollection Columns
		{
			get { return _columns; }
		}

		public Type Type
		{
			get { return _type; }
		}

		public string DatabaseName
		{
			get { return _databaseName; }
		}

		#endregion

		#region Functions

		private Dictionary<string, string> GetColumnAndValuesDictionary(T entity)
		{

			Dictionary<string, string> result = new Dictionary<string, string>();

			dynamic value;
			Type type;

			foreach(var item in _columns)
			{
				if (item.IsIdentity == 0)
				{
					value = this.GetPropertyValue(item.ColumnName, entity);
					type = this.GetPropertyType(item.ColumnName, entity);

					result.Add(item.ColumnName, Extension.ToSQLString(value, type));
				}
				
			}

			return result;

		}

		private string GenerateConditions(T entity)
		{

			string[] primaryKeys;
			string[] primaryKeyValues;
			string result;

			result = " WHERE 1 = 1";

			if(_columns.IsMultiPrimaryKey())
			{
				primaryKeys = _columns.GetPrimaryKey().Split('|');
				primaryKeyValues = entity.PrimaryKey.Value.Split('|');

				for(int i = 0 ; i <= (primaryKeys.Length - 1) ; i++)
				{
					result += " AND " + primaryKeys[i] + " = " + primaryKeyValues[i].ToSQLString();
				}
			}
			else
			{
				result += " AND " + _columns.GetPrimaryKey() + " = " + entity.PrimaryKey.Value.ToSQLString();
			}

			return result;

		}

		private long OnCount(QueryArg queryArg)
		{
			
			string query;

			if(!this.UseDatabase(this.DatabaseName))
				return 0;

			if(queryArg != null)
				query = "SELECT COUNT(*) as count FROM " + this.TableName + queryArg.Filter.ToSQLCondition(_type);
			else
				query = "SELECT COUNT(*) FROM " + this.TableName; 

			return this.DoCount(query);

		}

		private bool OnAdd(T entity)
		{

			string columns = string.Empty;
			string values = string.Empty;

			string query;

			if (!this.UseDatabase(this.DatabaseName))
				return false;

			foreach (var item in this.GetColumnAndValuesDictionary(entity))
			{
				columns += ",[" + item.Key + "]";
				values += "," + item.Value;
			}

			if (columns.Length > 0)
				columns = columns.Remove(0, 1);

			if (values.Length > 0)
				values = values.Remove(0, 1);

			query = "INSERT INTO @TableName (@Columns) SELECT @Values WHERE NOT EXISTS (SELECT 1 FROM @TableName @Condition)";

			query = query.Replace("@TableName", this.TableName);
			query = query.Replace("@Columns", columns);
			query = query.Replace("@Values", values);
			query = query.Replace("@Condition", this.GenerateConditions(entity));

			return this.DoInsert(query);

		}

		private bool OnDelete(T entity)
		{

			string query;
			string[] primaryKeys;
			string[] primaryKeyValues;
			string condition;

			if(!this.UseDatabase(this.DatabaseName))
				return false;

			if(_columns.IsMultiPrimaryKey())
			{
				primaryKeys = _columns.GetPrimaryKey().Split('|');
				primaryKeyValues = entity.PrimaryKey.Value.Split('|');

				condition = " WHERE ";

				for(int i = 0 ; i <= (primaryKeys.Length - 1) ; i++)
				{
					condition += primaryKeys[i] + " = " + primaryKeyValues[i].ToSQLString() + " AND ";
				}

				condition = condition.Remove(condition.Length - 5, 5);

				query = "DELETE TOP (1) FROM " + this.TableName + " " + condition;

			}
			else
			{
				query = "DELETE TOP (1) FROM " + this.TableName + " WHERE " + _columns.GetPrimaryKey() + " = " + entity.PrimaryKey.Value.ToSQLString();
			}

			return this.DoDelete(query);

		}

		private dynamic GetPropertyValue(string field, T entity)
		{

			Attribute[] attributes;
			dynamic value;

			foreach(var item in _properties)
			{
				attributes = Attribute.GetCustomAttributes(item);

				if(attributes != null)
				{
					foreach(var attribute in attributes)
					{
						if(attribute is XmlElementAttribute)
						{							
							if(field == ((XmlElementAttribute)attribute).ElementName)
							{
								value = _type.GetProperty(item.Name).GetValue(entity, null);
								return value;
							}
						}

					}
				}
			}

			return null;

		}

		private string GetColumnAndValues(T entity)
		{

			string result = string.Empty;
			dynamic value;
			Type type;
		
			foreach(var item in _columns)
			{

				if(item.IsIdentity == 0)
				{
					value = this.GetPropertyValue(item.ColumnName, entity);
					type = this.GetPropertyType(item.ColumnName, entity);

					result += ",[" + item.ColumnName + "]=" + Extension.ToSQLString(value, type);
				}
			
			}

			if(!string.IsNullOrEmpty(result))
				result = result.Remove(0, 1);

			return result;

		}
	
		private System.Type GetPropertyType(string field, T entity)
		{

			Attribute[] attributes;

			foreach(var item in _properties)
			{
				attributes = Attribute.GetCustomAttributes(item);

				if(attributes != null)
				{
					foreach(var attribute in attributes)
					{
						if(attribute is XmlElementAttribute)
						{
							if(field == ((XmlElementAttribute)attribute).ElementName)
							{
								return item.PropertyType;
							}
						}
					}
				}
			}

			return null;

		}

		#endregion

		#region Voids

		private void InitializeColumns()
		{

			var columns = new ColumnCollection();
			var collection = (IEnumerable)columns;

			this.DeserializeResult(this.GetColumns(this.DatabaseName, _tableName), ref collection);

			_columns = (ColumnCollection)collection;

		}

		private void DeserializeResult(DataTable dataTable, ref IEnumerable model)
		{		

			using(var data = new DataSet("ArrayOf" + dataTable.TableName))
			{
				data.Tables.Add(dataTable);
				model = Serializer.Deserialize(data.GetXml(), model.GetType());			
			}
			
		}

		private T OnFindById(string id)
		{

			var result = new BaseModelCollection<T>();
			var collection = (IEnumerable)result;

			string query;

			query = "select * from " + this.TableName + " WHERE " + _columns.GetPrimaryKey() + " = " + id.ToSQLString();

			this.Search(ref collection, query);

			result = (BaseModelCollection<T>)collection;

			if(result.Count > 0)
				return result[0];

			return null;

		}

		private BaseModelCollection<T> OnFind(QueryArg queryArgs)
		{

			var result = new BaseModelCollection<T>();
			var collection = (IEnumerable)result;

			this.Search(ref collection, queryArgs, this._tableName);

			return (BaseModelCollection<T>)collection;

		}

		private bool OnUpdate(T entity)
		{

			string query;
			string[] primaryKeys;
			string[] primaryKeyValues;
			string condition;

			if(!this.UseDatabase(this.DatabaseName))
				return false;

			if(_columns.IsMultiPrimaryKey())
			{
				primaryKeys = _columns.GetPrimaryKey().Split('|');
				primaryKeyValues = entity.PrimaryKey.Value.Split('|');

				condition = " WHERE ";

				for(int i = 0 ; i <= (primaryKeys.Length - 1) ; i++)
				{
					condition += primaryKeys[i] + " = " + primaryKeyValues[i].ToSQLString() + " AND ";
				}

				condition = condition.Remove(condition.Length - 5, 5);

				query = "UPDATE TOP (1) " + this.TableName + " SET " + this.GetColumnAndValues(entity) + condition;
			}
			else
			{
				query = "UPDATE TOP (1) " + this.TableName + " SET " + this.GetColumnAndValues(entity) + " WHERE " + _columns.GetPrimaryKey() + " = " + entity.PrimaryKey.Value.ToSQLString();
			}

			return this.DoUpdate(query);

		}

		protected void Search(ref IEnumerable results, string query)
		{

			DataTable dataTable;

			if(!this.UseDatabase(this.DatabaseName))
				return;

			dataTable = this.DoSelect(_type.Name, query);

			if(dataTable != null)
				this.DeserializeResult(dataTable, ref results);

		}

		protected void Search(ref IEnumerable results, QueryArg queryArgs, string tableName)
		{

			DataTable dataTable;
			string query = "SELECT * FROM [" + tableName + "]";

			if(!this.UseDatabase(this.DatabaseName))
				return;

			if (queryArgs != null)
			{
				query = "SELECT " + queryArgs.ToSQLField(_type) + " FROM " + TableName + "";
				query += queryArgs.Filter.ToSQLCondition(_type);
				query += queryArgs.ToSQLOrderBy(_type);
				query += " OFFSET " + queryArgs.Offset + " ROWS";

				if(queryArgs.Limit > 0)
					query += " FETCH NEXT " + queryArgs.Limit + " ROWS ONLY";
			}
			

			dataTable = this.DoSelect(_type.Name, query);

			if(dataTable != null)
				this.DeserializeResult(dataTable, ref results);

		}

		#endregion

		#region IRepository Members
	
		public Guid Guid
		{
			get { return _guid; }
			set { _guid = value; }
		}

		public string TableName
		{
			get { return  "[" + _tableName + "]"; }
			set
			{
				_tableName = value;
				this.InitializeColumns();
			}
		}

		public bool Add(T entity)
		{
			if(entity == null)
				return false;

			return this.OnAdd(entity);

		}

		public T FindById(string id)
		{
			return this.OnFindById(id);
		}

		public BaseModelCollection<T> Find(QueryArg queryArgs)
		{
			return this.OnFind(queryArgs);
		}

		public bool Delete(T entity)
		{
			if(entity == null)
				return false;

			return this.OnDelete(entity);

		}

		public bool Update(T entity)
		{
			if(entity == null)
				return false;

			return this.OnUpdate(entity);
		}

		public long Count(QueryArg queryArg)
		{
			return this.OnCount(queryArg);
		}

		#endregion

	}
}
