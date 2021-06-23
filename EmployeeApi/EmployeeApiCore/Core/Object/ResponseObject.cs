using System.Reflection;

namespace EmployeeApiCore.Core.Object
{
	public class ResponseObject : BindableObject
	{

		public ResponseObject(string sessionId, string token)
		{
			//this.SessionId = sessionId;
			//this.Token = token;
		}

		#region Properties

		//public string SessionId
		//{
		//	get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
		//	set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		//}

		//public string Token
		//{
		//	get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
		//	set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		//}

		public dynamic Data
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		public string Message
		{
			get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
			set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		}

		#endregion
		
	}

	public class ListResponseObject : ResponseObject
	{
		public ListResponseObject(string sessionId, string token) : base(sessionId, token)
		{
		}

        public ListResponseObject() : base("", "")
        {
        }

  //      public long TotalCount
		//{
		//	get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
		//	set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
		//}
		
	}

    public class PlainResponseObject : BindableObject
    {
        public PlainResponseObject()
        {

        }

        #region Properties

        public int MessageCode
        {
            get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
            set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
        }

        public string Message
        {
            get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
            set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
        }

        public dynamic Data
        {
            get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
            set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
        }

        #endregion
    }
}
