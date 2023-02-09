using System.Net;

namespace UdpGroupChat.Extensions
{
    internal static class IpAddressExtensions
    {
        public static bool IsIpInRange(
            this IPAddress address,
            IPAddress startIpAddress,
            IPAddress endIpAddress)
        {
            int startIpNumeric = BitConverter.ToInt32(startIpAddress.GetAddressBytes().Reverse().ToArray(), 0);
            int endIpNumeric = BitConverter.ToInt32(endIpAddress.GetAddressBytes().Reverse().ToArray(), 0);
            int currentIpNumeric = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

            return (currentIpNumeric >= startIpNumeric) && (currentIpNumeric <= endIpNumeric);
        }
    }
}
