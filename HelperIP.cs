using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MedLab
{
    public static class HelperIP
    {
        public static string GetIp()
        {
            string ipAddress = string.Empty;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            return ipAddress;
        }
    }
}
