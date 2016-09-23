using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                            Console.WriteLine("MyIpIs");
                            Console.WriteLine(aTag.InnerHtml);
                        }
                        
                        counter++;
                    }
                }
            }

            Console.WriteLine(url);
            //Console.WriteLine(stringhtml);


            //string url = "http://www.whatsmyip.org/";
            //HtmlWeb web = new HtmlWeb();
            //HtmlDocument doc = web.Load(url);
            //var request = (HttpWebRequest)WebRequest.Create(url);
            //request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";

            //string stringhtml = string.Empty;
            //var aTags = doc.DocumentNode.SelectNodes("//span");
            //int counter = 1;
            //if (aTags != null)
            //{
            //    foreach (var aTag in aTags)
            //    {
            //        stringhtml += counter + ". " + aTag.InnerHtml + " - " + aTag.Attributes["id"].Value + "\t" + "<br />"; //ip
            //        counter++;
            //    }
            //}

            //using (var client = new WebClient())
            //{
            //    string html = client.DownloadString("http://www.whatismyip.com");
            //    // TODO: do something with the downloaded result from the remote
            //    // web site

            //    var doc = new HtmlDocument();
            //    doc.LoadHtml(html);

            //    var text = doc.DocumentNode.Descendants()
            //                  .Where(x => x.NodeType == HtmlNodeType.Text && x.InnerText.Trim().Length > 0)
            //                  .Select(x => x.InnerText.Trim());

            //    //var web = new HtmlWeb();
            //    //var doc = web.Load(url);
            //}
        }
    }
}
