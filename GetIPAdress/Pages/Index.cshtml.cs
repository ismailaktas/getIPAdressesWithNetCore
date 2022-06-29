using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;

namespace GetIPAdress.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

            ViewData["serverLocalIPAdress"] = getServerLocalIPAdress();
            ViewData["serverPublicIPAdress"] = getServerPublicIPAdress();
            //
            ViewData["clientIPAdress1"] = getClientIPAddress();
            ViewData["clientIPAdress2"] = GetClientIPAddress1(HttpContext);
        }


        //Sunucuya ait Local (İç Network) IP Adresi
        public string getServerLocalIPAdress()
        {
            string strIpAdress = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIpAdress = ip.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                strIpAdress = string.Concat("IP adresi bulunamadi. Hata: ", ex.Message);
            }
            return strIpAdress;
        }


        //Sunucuya ait Public (İnternet) IP Adresi
        public string getServerPublicIPAdress()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }


        //Ziyaretçi IP Adresi - Yöntem 1
        public string getClientIPAddress()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }


        //Ziyaretçi IP Adresi - Yöntem 2
        public static string GetClientIPAddress1(HttpContext context)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            {
                ip = context.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
            }
            return ip;
        }


    }
}
