using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeApiCore.Core.Class
{
	public abstract class Connection
	{

		#region Variables

		private SqlConnection _connection = new SqlConnection();
		private SqlDataAdapter _dataAdapter = new SqlDataAdapter();
		private SqlTransaction _transaction;

		private SqlDataReader _dataReader;
		private string _connectionId = string.Empty;
		private string _query;
		private string _database;

		#endregion

		#region Delegates

		public delegate void ErrorEncounteredEventHandler(System.Exception ex);

		#endregion

		#region Events

		internal event ErrorEncounteredEventHandler ErrorEncountered;

		#endregion

		#region Constructor

		public Connection()
		{
			//if(connection != null)
			//{
			//	_connection = connection;
			//}
			//else
			//{
			//	if(ErrorEncountered != null)
			//		this.ErrorEncountered(new System.Exception("Connection not defined."));
			//}
		}

		#endregion

		#region Functions

		public bool Connect()
		{

			try
			{
				if(_connection.State != ConnectionState.Open)
				{
					string connectionString =  ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

					try
					{
						_connection = new SqlConnection();						
						_connection.ConnectionString = connectionString;
						_connection.Open();
					}
					catch(SqlException ex)
					{
						if(this.ErrorEncountered != null)
							this.ErrorEncountered(ex);

						//return false;

						throw;
					}

				}

				return true;
			}
			catch(Exception ex)
			{
				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);

				//return false;

				throw;
			}

		}

		public bool UseDatabase(string databaseName)
		{

			var command = new SqlCommand();

			try
			{
				if(!this.Connect())
					return false;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = "USE " + "[" + databaseName + "]";
				command.ExecuteNonQuery();
				command.Dispose();

				_database = databaseName;

				return true;
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);

				return false;
			}

		}


		public bool DoInsert(string query)
		{

			var command = new SqlCommand();

			_query = query;

			try
			{
				if(!this.Connect())
					return false;

				command.Connection = _connection;				
				command.CommandType = CommandType.Text;
				command.CommandText = query;
				command.ExecuteNonQuery();				
				command.Dispose();

				return true;
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);

				return false;
			}
			finally
			{
				_connection.Close();
			}

		}

		public long DoCount(string query)
		{
			var command = new SqlCommand();		

			_query = query;

			try
			{
				if(!this.Connect())
					return 0;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = query;

				var result = (int)command.ExecuteScalar();
				
				command.Dispose();
				return result;
			}
			catch(Exception ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);

				return 0;
			}
			finally
			{
				_connection.Close();
			}
		}

		public bool DoDelete(string query)
		{

			var command = new SqlCommand();

			_query = query;

			try
			{
				if(!this.Connect())
					return false;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = query;
				command.ExecuteNonQuery();
				command.Dispose();

				return true;
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);
			}
			finally
			{
				_connection.Close();
			}

			return false;

		}

		public bool DoUpdate(string query)
		{

			var command = new SqlCommand();

			_query = query;

			try
			{
				if(!this.Connect())
					return false;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = query;
				command.ExecuteNonQuery();
				command.Dispose();

				return true;
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);
			}
			finally
			{
				_connection.Close();
			}

			return false;

		}

		public DataTable DoSelect(string tableName, string query)
		{

			var table = new DataTable(tableName);
			var command = new SqlCommand();

			_query = query;

			try
			{
				if(!this.Connect())
					return null;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = query;

				_dataAdapter.SelectCommand = command;
				table.Clear();
				_dataAdapter.Fill(table);
				command.Dispose();
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);
			}
			finally
			{
				_connection.Close();
			}

			return table;

		}

		public DataTable GetColumns(string database, string tableName)
		{

			var table = new DataTable("ColumnModel");
			var command = new SqlCommand();

			_query =
					"SELECT C.*, ISNULL(TC.CONSTRAINT_TYPE,'') AS CONSTRAINT_TYPE, COLUMNPROPERTY(object_id('"+ tableName + "'), C.COLUMN_NAME, 'IsIdentity') AS 'IS_IDENTITY'" +
					"  FROM INFORMATION_SCHEMA.COLUMNS C" +
					"       LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CCU ON (C.TABLE_NAME = CCU.TABLE_NAME AND C.COLUMN_NAME = CCU.COLUMN_NAME)" +
					"       LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON (CCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME)" +
					"WHERE C.TABLE_CATALOG = @Database AND C.TABLE_NAME = @TableName";

			try
			{
				if(!this.Connect())
					return null;

				command.Connection = _connection;
				command.CommandType = CommandType.Text;
				command.CommandText = _query;

				command.Parameters.Add(new SqlParameter("@Database", database));
				command.Parameters.Add(new SqlParameter("@TableName", tableName));

				_dataAdapter.SelectCommand = command;
				table.Clear();
				_dataAdapter.Fill(table);
				command.Dispose();
			}
			catch(SqlException ex)
			{
				command.Dispose();

				if(this.ErrorEncountered != null)
					this.ErrorEncountered(ex);
			}
			finally
			{
				_connection.Close();
			}

			return table;

		}

		#endregion

	}
}
