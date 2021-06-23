#define USE_XML_DATA
using System;
using System.ComponentModel;
using System.Web;
using EmployeeApiCore.Core.Object.Collection;

namespace EmployeeApiCore.Core.Class
{
	public class Global
	{
		public const string PollMessageDelimiter = "<CR>";
        public const string ReaderMessageDelimiter = "\r\n";
        public const string ReaderMessagePrefix = "#TiToSystem";
        public const string ReaderMessageSuffix = "0";

        public const string ApiKey = "ticketInTicketOutApi";
		public const string SessionId = "3CB54559-46F0-4F4F-A7FB-C602CE3992AA";
		//public const string Token = "08FF46AE-F652-44B8-A39E-F6E424B76B89";

		// Set value to FALSE if you want to use database data instead of Data.xml
		public static bool UseXmlData = false;
				
		public enum Role
		{
			Undefined,
			CCSAdmin,
			CCSBackOffice,
			ClientAdmin,
			ClientManager,
			ClientTechnician,
			ClientBackOffice
		}

        public enum Status
        {
            VALID            = 0,
            EXPIRED          = -1,
            REDEEMED         = -2,
            ESCROWED         = -3,
            WAITING_PRINT_OK = -4,
            GENERAL_ERROR    = -100
        }

        public enum ReaderMethods
        {
            CreateTicket,
            AckTicketCreation,
            RedeemTicketValue,
            AckTicketRedemption
        }

        public enum MessageCodes
        {
            [Description("Successful")]
            Successful = 1,
            [Description("Invalid Json Format")]
            InvalidJsonFormat = 2,
			[Description("Unable to save employee")]
			SaveEmployeeError = 3,
			[Description("Employee already exists")]
			EmployeeAlreadyExists = 4
		}

        public enum LogTypes
        {
            Log,
            Failure
        }

        public static string DataXmlPath = System.IO.Path.Combine(ApplicationPath, "TiToData.xml");
        
        public static Object.ConfigObject Configuration;

        public static SettingObjectCollection Settings = new SettingObjectCollection();

        public static string EnvironmentPath;

        public static System.DateTime GenerateRandomDateTime(int dateOffset)
		{
			TimeSpan timeSpan = DateTime.Now - DateTime.UtcNow.AddDays(dateOffset);
			var randomTest = new Random();
			TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
			DateTime newDate = DateTime.UtcNow.AddDays(dateOffset) + newSpan;

			return newDate;
		}

		public static double GenerateRandomAmount(int maxValue, int minValue)
		{			
			return Math.Round((new Random().NextDouble() * (maxValue - minValue) + minValue), 2, MidpointRounding.AwayFromZero);
		}

		public static double GenerateRandomAmount(Random random, int maxValue, int minValue)
		{
			return Math.Round((random.NextDouble() * (maxValue - minValue) + minValue), 2, MidpointRounding.AwayFromZero);
		}

		public static string StripSpecialCharacters(string text)
		{
			string pattern = "[^a-zA-Z0-9]";
			var regex = new System.Text.RegularExpressions.Regex(pattern);
			string newText;
			
			newText = regex.Replace(text, "");

			return newText;
		}

		public static string ApplicationPath
		{
			get { return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName); }
		}

		public static bool CreateFile(string filename, string contents, bool isAppend = false)
		{
			try
			{
				using(var writer = new System.IO.StreamWriter(filename, isAppend))
				{
					writer.WriteLine(contents);
					writer.Flush();
					writer.Close();
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
