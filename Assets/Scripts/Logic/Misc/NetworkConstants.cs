using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Logic.Misc
{
	public class NetworkConstants 
	{
		public static System.Net.IPAddress GetIPAddress(System.Net.IPAddress ip)
		{
#if UNITY_IPHONE
			bool isIPV4Format = ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
			bool isIPV6Environment = ZYZ.SystemEx.NetEx.SocketsEx.NetworkUtils.IsIPV6();
			if (isIPV4Format && isIPV6Environment)
			{
				ip = System.Net.IPAddress.Parse(IPV6Access.ConvertIPv4ToIPv6(ip.ToString()));
			}
#endif
			return ip;
		}

		public static string GetIPAddress(string ipAdr)
		{
			string ipRet = ipAdr;

#if UNITY_IPHONE
			System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(ipAdr);
			if (ips.Length > 0)
			{
				System.Net.IPAddress ip = GetIPAddress(ips[0]);
				ipRet = ip.ToString();
			}
#endif
			return ipRet;
		}
	}
}