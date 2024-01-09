using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public class ApplicationLog
    {
        static string AppDataFolder = @"Симплекс\СиМед - Клиника\СиМед - Чат агрегатор";
        public static void SaveExceptionToLog(Exception ex, Clinic.DataModel.ClinicChatAggregatorApplicationType appType)
        {
            string LogFileName = "";
            string dateTime = DateTime.Now.ToString("yyyy_MM_dd");
            string ExceptionMessage = DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss_fff")+"\r\n" +ex.Message + " " + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source;
            if (appType == Clinic.DataModel.ClinicChatAggregatorApplicationType.Program)
            {
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder)))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder));
                }

                DirectoryInfo directories = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder));
                foreach (var directory in directories.GetDirectories())
                {
                    DateTime datetaTime = Convert.ToDateTime(directory.Name.Replace('_', '.'));
                    int differenceDay = DateTime.Now.Subtract(datetaTime).Days;
                    if (differenceDay >= 30)
                    {
                        directory.Delete(true);
                    }
                }

                LogFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder, dateTime + ".log") ;
            }          
            else if (appType == Clinic.DataModel.ClinicChatAggregatorApplicationType.Service)
            {
                if (!Directory.Exists(Path.Combine(System.Windows.Forms.Application.StartupPath, "Logs")))
                {
                    Directory.CreateDirectory(Path.Combine(System.Windows.Forms.Application.StartupPath, "Logs"));
                }

                DirectoryInfo directories = new DirectoryInfo(Path.Combine(System.Windows.Forms.Application.StartupPath, "Logs"));
                foreach (var directory in directories.GetDirectories())
                {
                    DateTime datetaTime = Convert.ToDateTime(directory.Name.Replace('_', '.'));
                    int differenceDay = DateTime.Now.Subtract(datetaTime).Days;
                    if (differenceDay >= 30)
                    {
                        directory.Delete(true);
                    }
                }

                LogFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "Logs", dateTime + ".log");               
            }

           

                using (StreamWriter sw = new StreamWriter(LogFileName,true))
                {
                    sw.WriteLine(ExceptionMessage);
                }
                //byte[] buffer = Encoding.Default.GetBytes(ExceptionMessage);
                //fstream.Write(buffer, 0, buffer.Length);
            
        } 
    }
}
