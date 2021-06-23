using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApiCore.Core.Class;

namespace EmployeeApiCore.Service
{
    public class LoggerService
    {
        public void WriteToLog(string messsage, string logFileName)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Path.Combine(Global.EnvironmentPath, logFileName), true))
            {
                file.WriteLine(DateTime.UtcNow.ToString() + " " + messsage);
            }
        }

        public void LogApplicationError(string messsage)
        {
            string path = Path.Combine(Global.EnvironmentPath, "errorLog.txt");

            if (IsLogFileTooLarge(path))
            {
                BackupLogFile(path);
            }

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(DateTime.UtcNow.ToString() + " " + messsage + "\r\n");
            }
        }

        public void LogReaderMessageError(string messsage)
        {
            string path = Path.Combine(Global.EnvironmentPath, "readerErrorMessageLog.txt");

            if (IsLogFileTooLarge(path))
            {
                BackupLogFile(path);
            }

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(DateTime.UtcNow.ToString() + " " + messsage + "\r\n");
            }
        }

        public bool IsLogFileTooLarge(string logFile)
        {
            bool isTooLarge = false;

            long bytes = 0;
            double kilobytes = 0;
            double megabytes = 0;

            if (File.Exists(logFile))
            {
                bytes = new FileInfo(logFile).Length;
                kilobytes = (double)bytes / 1024;
                megabytes = kilobytes / 1024;

                if (megabytes > 10)
                {
                    isTooLarge = true;
                }
            }

            return isTooLarge;
        }
        
        public void BackupLogFile(string logFilePath)
        {
            string backupLogFilePath = logFilePath.Replace(".txt", ".bak.txt");

            FileInfo backuplogfile = new FileInfo(backupLogFilePath);
            FileInfo logfile = new FileInfo(logFilePath);

            if (File.Exists(backupLogFilePath))
            {
                // An existing log file is present. Delete it first.
                File.Delete(backupLogFilePath);
            }

            File.Move(logfile.Name, backuplogfile.Name);

        }
    }
}
