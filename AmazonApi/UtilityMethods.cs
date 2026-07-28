using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using Serilog;
using System.Net.Mail;
using System.Net;

namespace AmazonAPI
{
    public class UtilityMethods
    {
      private static readonly string logFilePath = @"C:\Logs\Amazon\WindowsServiceLog.txt";
      private static readonly ILogger log = new LoggerConfiguration()
        .WriteTo.File(logFilePath,rollingInterval: RollingInterval.Day,retainedFileCountLimit: 30,
                      outputTemplate: "{Timestamp:dd/MM/yyyy HH:mm:ss} {Level}: {Message}{NewLine}")
        .CreateLogger();
//get dateTime for Israel
        public static DateTime IsraelDateTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
        }
       public static DateTime PDTDateTime(DateTime date)
       {
          return TimeZoneInfo.ConvertTime(date, 
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        } 
public static DateTime CurrentPDTDateTime()
       {
          return TimeZoneInfo.ConvertTime(DateTime.Now, 
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        } 

//write to log table
public static void WriteToLog(string msg)
        {
            SqlConnection con2 = new SqlConnection(SD.connectionStr);
            try
            {
                con2.Open();
                string sql = "INSERT INTO [dbo].LogsDatas " +
                             "(Msg1, CreatedDate) VALUES " +
                             "(@LOGMSG,@CREATEDAT)";
                SqlCommand cmd = new SqlCommand(sql, con2);
                cmd.Parameters.Add("@LOGMSG", SqlDbType.VarChar, 4000).Value = msg;
                cmd.Parameters.Add("@CREATEDAT", SqlDbType.DateTime, 100).Value = IsraelDateTime();
                cmd.ExecuteNonQuery();
                con2.Close();
            }
            catch (Exception ex)
            {
                if (con2.State == ConnectionState.Open)
                    con2.Close();
                Console.WriteLine("error in InsertToNotificationTable of CheckStatusAddNotifications " + ex.Message);
            }
        }
public static void WriteToTextLog(string message, string type)
        {
          try
          {
             if (type == "INF")
               log.Information(message);
             else if (type == "ERR")
               log.Error(message);
          }
           catch (Exception ex)
            {
              log.Error("Error writing log: {ErrorMessage}", ex.Message);
            }
        }
public static void WriteToLogSuccess(string msg)
        {
            SqlConnection con2 = new SqlConnection(SD.connectionStr);
            try
            {
                con2.Open();
                string sql = "INSERT INTO [dbo].LogsDatas " +
                             "(Msg2, CreatedDate) VALUES " +
                             "(@LOGMSG,@CREATEDAT)";
                SqlCommand cmd = new SqlCommand(sql, con2);
                cmd.Parameters.Add("@LOGMSG", SqlDbType.VarChar, 4000).Value = msg;
                cmd.Parameters.Add("@CREATEDAT", SqlDbType.DateTime, 100).Value = IsraelDateTime();
                cmd.ExecuteNonQuery();
                con2.Close();
            }
            catch (Exception ex)
            {
                if (con2.State == ConnectionState.Open)
                    con2.Close();
                Console.WriteLine("error in InsertToNotificationTable of CheckStatusAddNotifications " + ex.Message);
            }
        }
        public static string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }
        public static DateTime? SafeGetDateTime(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDateTime(colIndex);
            return null;
        }
public static void SendErrMail(string excep)
        {
            try
            {
                // Configure your SMTP client settings.
                // For example, using Gmail's SMTP server.
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ktonlinemarketing1@gmail.com", SD.gmailPassSMTP),
                    EnableSsl = true
                };

                // Create the email message.
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("ktonlinemarketing1@gmail.com"),
                    Subject = "Error on API-SP",
                    Body = excep
                };

                // Send the email to yourself.
                mailMessage.To.Add("ktonlinemarketing1@gmail.com");

                // Send the email.
                smtpClient.Send(mailMessage);
            }
            catch (Exception emailEx)
            {
                // If the email fails, write to the console (or log appropriately).
                Console.WriteLine("Failed to send exception email: " + emailEx.Message);
            }
        }
    }
}
