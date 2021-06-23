using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApiCore.Core.Object
{
    public class Response
    {
        public int? MessageCode { get; set; }

        public string Message { get; set; }
		
        public bool ShouldSerializeMessageCode()
        {
            return (MessageCode != null);
        }

        public bool ShouldSerializeMessage()
        {
            return (Message != null);
        }
	}

    public class CreateMessage : Response
	{
        public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }

	}

	public class UpdateMessage : Response
	{
		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }

	}
}
