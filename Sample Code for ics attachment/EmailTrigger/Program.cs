using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace EmailTrigger
{

    public class Program
    {
        public static void Main(string[] args)
        {

            Common.GetExpiryDates();

        }

        public static void CreateCSV(DataTable dt)
        {

            var groups = dt.AsEnumerable().GroupBy(a => a.Field<string>("service_owner_email")).ToDictionary(g => g.Key, g => g.ToList());

            for (int i = 0; i < groups.Count; i++)
            {
                var email = groups.ElementAt(i);
                var serviceOwnerEmail = email.Key;
                var distributionEmail = email.Value[0].Field<string>("distribution_list_email");
                var dtfilter = "logins" + String.Join(",", email.Value.Select(a => a.Field<string>("login_name")));

                var rows = from row in dt.AsEnumerable()
                           where row.Field<string>("service_owner_email").Trim() == serviceOwnerEmail
                           select row;

                DataTable dt1 = rows.CopyToDataTable();
                DataTable dt2 = new DataTable();


                dt2.Columns.AddRange(new DataColumn[]
                {
                        new DataColumn("Server Name"),
                        new DataColumn("Database Name"),
                        new DataColumn("Login Name"),
                        new DataColumn("Expiry Date")
                });

                foreach (DataRow row in dt1.Rows)
                {
                    dt2.Rows.Add(row["server_name"].ToString(),
                                 row["database_name"].ToString(),
                                 row["login_name"].ToString(),
                                 row["password_expiry_date"].ToString());
                }


                //string logFilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["logPath"], DateTime.Today.ToString("yyyy-MM-dd").ToString() + ".csv");

                //using (StreamWriter writer = new StreamWriter(logFilePath, false))
                //{
                //    for (int j = 0; j < dt2.Columns.Count; j++)
                //    {
                //        writer.Write(dt2.Columns[j]);
                //        if (j < dt2.Columns.Count - 1)
                //        {

                //            writer.Write(",");
                //        }
                //    }
                //    writer.WriteLine();


                //    foreach (DataRow row in dt2.Rows)
                //    {
                //        foreach (DataColumn column in dt2.Columns)
                //        {
                //            string value = row[column].ToString();
                //            if (value.Contains(","))
                //            {
                //                value = $"\"{value}\"";
                //            }
                //            writer.Write($"{value},");
                //        }
                //        writer.WriteLine();
                //    }
                //}


                int portNumber = int.Parse(ConfigurationManager.AppSettings["PortNumber"]);
                string smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                string emailId = ConfigurationManager.AppSettings["MailId"];
                string mailBody = ConfigurationManager.AppSettings["MailBody"].Replace("\\n",Environment.NewLine);

                MailMessage emailMessage = new MailMessage();

                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+330)));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+660)));
                str.AppendLine("LOCATION: " + "abcd");
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", emailMessage.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", emailMessage.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", emailMessage.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", emailId));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", distributionEmail, serviceOwnerEmail));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");


                byte[] byteArray = Encoding.ASCII.GetBytes(str.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                Attachment attach = new Attachment(stream, "test.ics");

               
                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                //  contype.Parameters.Add("name", "Meeting.ics");
               AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
               emailMessage.AlternateViews.Add(avCal);



                SmtpClient SMTPServer = new SmtpClient(smtpServer, portNumber);
                emailMessage = new MailMessage(emailId, serviceOwnerEmail, " ", mailBody);
                emailMessage.To.Add(distributionEmail);
                emailMessage.Subject = "Please reset your password";
                emailMessage.Priority = MailPriority.High;
                emailMessage.IsBodyHtml = false;
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(attach);
                //emailMessage.Attachments.Add(attachment);
                emailMessage.Attachments.Add(attach);

                SMTPServer.Send(emailMessage);


                ////attach.Dispose();
                //dt2.Clear();
            }

        }

    }
}




