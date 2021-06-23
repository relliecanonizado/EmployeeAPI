using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using EmployeeApiCore.Repository;
using EmployeeApi.Properties;
using EmployeeApiCore.Model.Collection;
using EmployeeApiCore.Core.Object;
using EmployeeApiCore.Model.Model;
using EmployeeApiCore.Core.Class;

namespace EmployeeApi.Controllers
{
	[RoutePrefix("api/employee")]
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class EmployeeController : ApiController
    {
		#region Variables

		private EmployeRepository _employeeRepository;
		private HttpResponseMessage _responseMessage;

		#endregion

		#region Constructor

		public EmployeeController()
		{
			_employeeRepository = new EmployeRepository(Settings.Default.DatabaseName);
		}

		#endregion

		#region GET Method

		// GET employee/
		[HttpGet]
		[Route("")]
		public IHttpActionResult Index()
		{
			return GetAllEmployees();
		}

		// GET employee/list
		[HttpGet]
		[Route("list")]
		public IHttpActionResult GetAllEmployees()
		{
			EmployeeCollection employeeCollection = _employeeRepository.GetAllEmployees();
			var response = new ListResponseObject("", "");

			if(employeeCollection.Count > 0)
			{
				response.Data = employeeCollection;
			}

			_responseMessage = base.Request.CreateResponse(HttpStatusCode.OK, response);

			return base.ResponseMessage(_responseMessage);
		}

		#endregion

		#region POST Method

		// POST : api/employee/create
		[HttpPost]
		[Route("create")]
		public IHttpActionResult Create([FromBody] CreateMessage createMessage)
		{
			Response response = new Response();

			try
			{
				EmployeeCollection employeeCollection = _employeeRepository.FindEmployeeByWholeName(
					createMessage.FirstName,
					createMessage.MiddleName,
					createMessage.LastName
				);
				
				if(employeeCollection.Count > 0)
				{
					response.MessageCode = (int)Global.MessageCodes.EmployeeAlreadyExists;
					response.Message = "Employee already exists";
					_responseMessage = base.Request.CreateResponse(HttpStatusCode.BadRequest, response);
					return base.ResponseMessage(_responseMessage);
				}

				Employee employee = new Employee();
				employee.FirstName = createMessage.FirstName;
				employee.MiddleName = createMessage.MiddleName;
				employee.LastName = createMessage.LastName;

				if(_employeeRepository.AddEmployee(createMessage.FirstName, createMessage.MiddleName, createMessage.LastName))
				{
					response.MessageCode = (int)Global.MessageCodes.Successful;
					response.Message = "Successfully created employee " + employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
					_responseMessage = base.Request.CreateResponse(HttpStatusCode.OK, response);
				}
				else
				{
					response.MessageCode = (int)Global.MessageCodes.SaveEmployeeError;
					response.Message = "An error has occured";
					_responseMessage = base.Request.CreateResponse(HttpStatusCode.BadRequest, response);
				}
			}
			catch(Exception e)
			{
				
			}

			return base.ResponseMessage(_responseMessage);
		}

		#endregion

		#region PUT method

		// PUT : api/employee/update/{employeeId}
		[HttpPut]
		[Route("update/{employeeId}")]
		public IHttpActionResult Update([FromBody] UpdateMessage updateMessage, string employeeId)
		{
			Response response = new Response();

			try
			{
				Employee employee = _employeeRepository.FindById(employeeId);

				if(employee != null)
				{
					employee.FirstName = updateMessage.FirstName;
					employee.MiddleName = updateMessage.MiddleName;
					employee.LastName = updateMessage.LastName;

					if(_employeeRepository.Update(employee))
					{
						response.Message = "Successfully updated employee data";
						_responseMessage = base.Request.CreateResponse(HttpStatusCode.OK, response);
					}
					else
					{
						_responseMessage = base.Request.CreateResponse(HttpStatusCode.InternalServerError);
					}
				}
			}
			catch(Exception e)
			{

			}

			return base.ResponseMessage(_responseMessage);
		}

		#endregion

		#region DELETE Method

		// DELETE: api/employee/delete/{employeeId}
		[HttpDelete]
		[Route("delete")]
		public IHttpActionResult Delete(int id)
		{
			Response response = new Response();
			Employee employee = _employeeRepository.FindById(Convert.ToString(id));

			if(employee != null)
			{
				if(_employeeRepository.DeleteById(Convert.ToInt32(id)))
				{
					response.Message = "Deleted";
				}
			}
			else
			{
				response.Message = "Not found";
			}

			_responseMessage = base.Request.CreateResponse(HttpStatusCode.OK, response);

			return ResponseMessage(_responseMessage);
		}

		#endregion
	}
}
