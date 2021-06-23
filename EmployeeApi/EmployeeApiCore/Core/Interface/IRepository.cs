using System;
using EmployeeApiCore.Core.BaseClass;
using EmployeeApiCore.Core.Object;

namespace EmployeeApiCore.Core.Interface
{
	public delegate void ErrorOccuredEventHandler(dynamic sender, EventArgs e);

	public interface IRepository<T> where T: Core.BaseClass.BaseModel
	{

		//event ErrorOccuredEventHandler ErrorOccured;

		System.Guid Guid { get; set; }
		string TableName { get; set; }

		T FindById(string id);

		BaseModelCollection<T> Find(QueryArg queryArgs);

		bool Add(T entity);

		bool Delete(T entity);

		bool Update(T entity);

	}
}
