using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using OnlineMarketApi.Models;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Helpers
{
    public class Mail
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static void Send(List<string> receivers,string subject, string messageContent, List<Setting> setting)
        {
            string mailAddress = setting.SingleOrDefault(s => s.Setting1 == "MailAddress").Value;
            string mailHost = setting.SingleOrDefault(s => s.Setting1 == "MailHost").Value;
            string mailPassword = setting.SingleOrDefault(s => s.Setting1 == "MailPassword").Value;
            string mailPort = setting.SingleOrDefault(s => s.Setting1 == "MailPort").Value;
            Task.Factory.StartNew(() =>
            {
                if (receivers != null && receivers.Count > 0)
                {
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(mailAddress, "Online Market App");
                    foreach (string to in receivers)
                    {
                        message.To.Add(new MailAddress(to.Trim()));
                    }
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = messageContent;
                    SmtpClient smtp = new SmtpClient(mailHost);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(mailAddress, mailPassword);
                    smtp.EnableSsl = true;
                    smtp.Port = int.Parse(mailPort);
                    smtp.Send(message);


                    //string host = "smtp.gmail.com";
                    //string fromMail = "essameldin.adil@gmail.com";
                    //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(host, 587);
                    //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = true;
                    //client.Credentials = new System.Net.NetworkCredential(fromMail, "_Malaz09133");
                    //client.EnableSsl = true;
                    //System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                    //mailMessage.From = new System.Net.Mail.MailAddress(fromMail);
                    //mailMessage.IsBodyHtml = true;
                    ////mailMessage.To.Add("mahmoud.ali@kenana.com");
                    //foreach (string to in receivers)
                    //{
                    //    mailMessage.To.Add(to);
                    //}
                    //if (copyto != null && copyto.Count > 0)
                    //{
                    //    foreach (string copy in copyto)
                    //    {
                    //        mailMessage.CC.Add(copy);
                    //    }
                    //}

                    //if (Attachments != null && Attachments.Count > 0)
                    //{
                    //    foreach (Attachment attachment in Attachments)
                    //    {
                    //        mailMessage.Attachments.Add(attachment);
                    //    }
                    //}
                    //mailMessage.Subject = subject;
                    //mailMessage.Body = message;
                    //client.Send(mailMessage);
                }
            });
        }
    }

}
