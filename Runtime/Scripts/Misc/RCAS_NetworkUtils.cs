using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;

namespace Edia.Rcas
{
    public static class RCAS_NetworkUtils
    {
        public static string CheckOrGetLocalIPAddress(string ipRange)
        {
            if (ipRange == "") ipRange = "no-valid-ip";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> IPv4CandidatesList = new List<IPAddress>();

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString().Contains(ipRange))
                    {
                        return ip.ToString();
                    } 
                    else
                    {
                        IPv4CandidatesList.Add(ip);
                    }
                }
            }

            if (IPv4CandidatesList.Count > 0)
            {
                return IPv4CandidatesList[0].ToString();
            }

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
