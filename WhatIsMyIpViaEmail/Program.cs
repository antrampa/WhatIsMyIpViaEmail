using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


namespace WhatIsMyIpViaEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://whatismyipaddress.com/";// "http://www.whatsmyip.org/";
            string stringhtml = string.Empty;
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionReadEncoding = false;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    htmlDoc.Load(stream, Encoding.UTF8);
                }

                var aTags = htmlDoc.DocumentNode.SelectNodes("//a");
                int counter = 1;
                
                if (aTags != null)
                {
                    foreach (var aTag in aTags)
                    {
                        stringhtml += counter + ". " + aTag.InnerHtml + " - " + aTag.Attributes["href"].Value + "\t" + "<br />"; //ip
                        if(aTag.Attributes["href"].Value.StartsWith("//whatismyipaddress.com/ip/"))
                        {
                            string currentIP = aTag.InnerHtml;// Attributes["href"].Value;
                            if(currentIP != Settings.Default.MyIp)
                            {
                                Settings.Default.MyIp = currentIP;
                                Settings.Default.Save();
                                //Send e-mail
                                SendEmail(currentIP);
                            }

                            Console.WriteLine("MyIpIs");
                            Console.WriteLine(aTag.InnerHtml);
                        }
                        
                        counter++;
                    }
                }
            }

            Console.WriteLine(url);
            //Console.WriteLine(stringhtml);
        }

        private static void SendEmail(string currentIpAddress)
        {
            try
            {
                string emailFrom = Settings.Default.EmailAddressFrom;
                string emailTo = Settings.Default.EmailAddressToSend;
                string fromName = Settings.Default.FromName;
                string toName = Settings.Default.ToName;
                string username = Settings.Default.SendEmailUsername; //"username";
                string password = Settings.Default.SendEmailPassword; //"password";
                string smtp = Settings.Default.SendEmailSMTP; //"smtp.gmail.com";
                int port = Settings.Default.SendEmailSMTPPort; //587;

                SmtpClient mySmtpClient = new SmtpClient(smtp, port);
                mySmtpClient.EnableSsl = true;
                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(username, password);
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(emailFrom, fromName);
                MailAddress to = new MailAddress(emailTo, toName);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                //// add ReplyTo
                //MailAddress replyto = new MailAddress(email);
                //myMail.ReplyToList.Add(replyto);

                // set subject and encoding
                myMail.Subject = "Your IP Address has changed";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = "<b>Your IP address now is:</b><br><b>"+ currentIpAddress + "</b>.";
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("SmtpException has occured: " + ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SendEmail: " + ex.Message);
            }

        }

    }
}
