using EmployeeApiCore.Model;
using EmployeeApiCore.Core.BaseClass;
using System.Collections;
using EmployeeApiCore.Model.Model;
using EmployeeApiCore.Model.Collection;

namespace EmployeeApiCore.Repository
{
    public class EmployeRepository : BaseRepository<Employee>
    {
        public EmployeRepository(string databaseName) : base(databaseName)
        {
            this.TableName = "Employee";

            this.ErrorEncountered += (ex) => { throw ex; };

        }

		#region Select queries

		public EmployeeCollection GetAllEmployees()
		{
			string query;
			IEnumerable result = new EmployeeCollection();

			query = "SELECT * FROM " + this.TableName;

			base.Search(ref result, query);

			return (EmployeeCollection)result;
		}

		public Employee FindEmployeeById(int employeeId)
        {
            string query;
            IEnumerable result = new EmployeeCollection();

            query = "SELECT * FROM " + this.TableName +
               " WHERE Id = '" + employeeId + "'";

            base.Search(ref result, query);

            if (result != null && ((EmployeeCollection)result).Count > 0)
                return ((EmployeeCollection)result)[0];

            return null;
        }

		public Employee FindEmployeeByFirstName(string firstName)
		{
			string query;
			IEnumerable result = new EmployeeCollection();

			query = "SELECT * FROM " + this.TableName +
			   " WHERE FirstName = '" + firstName + "'";

			base.Search(ref result, query);

			if(result != null && ((EmployeeCollection)result).Count > 0)
				return ((EmployeeCollection)result)[0];

			return null;
		}

		public Employee FindEmployeeByLastName(string lastName)
		{
			string query;
			IEnumerable result = new EmployeeCollection();

			query = "SELECT * FROM " + this.TableName +
			   " WHERE LastName = '" + lastName + "'";

			base.Search(ref result, query);

			if(result != null && ((EmployeeCollection)result).Count > 0)
				return ((EmployeeCollection)result)[0];

			return null;
		}

		public EmployeeCollection FindEmployeeByWholeName(string firstName, string middleName, string lastName)
		{
			string query;
			IEnumerable result = new EmployeeCollection();

			query = "SELECT * FROM " + this.TableName +
			   " WHERE FirstName = '" + firstName + "'" +
			   " AND MiddleName = '" + middleName + "'" +
			   " AND LastName = '" + lastName + "'";

			base.Search(ref result, query);

			return (EmployeeCollection)result;
		}

		#endregion

		#region Insert Operations

		public bool AddEmployee(string firstName, string middleName, string lastName)
		{
			Employee employee = new Employee();
			employee.FirstName = firstName;
			employee.MiddleName = middleName;
			employee.LastName = lastName;

			return base.Add(employee);
		}

		#endregion

		#region Update Operations

		public bool UpdateEmployeeDetails(string firstName, string middleName, string lastName)
		{
			//string query;

			//query = "UPDATE [Employee] SET ReaderConfigId = " + readerConfigId.ToSQLString() + " WHERE Id IN (" + readerIds + ")";

			//return this.DoUpdate(query);

			Employee employee = new Employee();
			employee.FirstName = firstName;
			employee.MiddleName = middleName;
			employee.LastName = lastName;

			return base.Update(employee);
		}

		#endregion

		#region Delete Operations

		public bool DeleteById(int id)
		{
			return base.DoDelete("DELETE FROM " + this.TableName + " WHERE Id = " + id);
		}

		#endregion

	}
}
