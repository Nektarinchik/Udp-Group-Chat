using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UdpGroupChat.Common;
using UdpGroupChat.Extensions;
using System.Data;
using System.Net.NetworkInformation;

namespace UdpGroupChat.Udp
{
    internal sealed class ClientCreator
    {
        //private static readonly IPAddress _localHost = IPAddress.Parse("127.0.0.1");

        private int _broadcastPort;
        private int _requestsPort;
        public IPAddress ChatIp { get; set; }
        public User User { get; set; } = new User();
        public ClientCreator(
            string name,
            int requestsPort,
            int broadcastPort,
            IPAddress broadcastIp)
        {
            User.Name = name;
            _requestsPort = requestsPort;
            _broadcastPort = broadcastPort;
            try
            {
                ChatIp = broadcastIp;
            }
            catch (SocketException)
            {
                throw;
            }
        }
        public async Task SendToBroadcastAsync()
        {
            if (ReferenceEquals(ChatIp, null))
            {
                throw new NullReferenceException("Undefined value of GhatIp");
            }

            // create group
            using var udpSender = new UdpClient();
            Console.WriteLine($"Group created by {User?.Name}");

            // creator send messages
            StringBuilder message = new StringBuilder();
            byte[] data;
            while (true)
            {
                message.Clear();
                message.Append(Console.ReadLine()!);

                if (string.IsNullOrWhiteSpace(message.ToString()))
                    break;

                message.Append($": {User?.Name}");
                data = Encoding.UTF8.GetBytes(message.ToString());
                await udpSender.SendAsync(data, new IPEndPoint(ChatIp, _broadcastPort));
            }
        }
        public async Task ReceiveRequests()
        {
            using var receiver = new UdpClient(
                new IPEndPoint(GetLocalIPAddress(), _requestsPort));
            while (true)
            {
                var result = await receiver.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }
                Console.WriteLine(message);
                await SendResponse(result.RemoteEndPoint);
            }
        }
        public async Task SendResponse(IPEndPoint endPoint)
        {
            using UdpClient sender = new UdpClient();
            IPEndPoint? receiverEndPoint = null;

            while (true)
            {
                var message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("Give response in format: [y/n]");
                    continue;
                }
                bool isParseSuccessed = ParseResponse(message, out receiverEndPoint);
                if (isParseSuccessed)
                {
                    string response = "";
                    if (ReferenceEquals(receiverEndPoint, null))
                    {
                        response = $"Access denied: {User.Name}";
                    }
                    else
                    {
                        response = $"{receiverEndPoint}";
                    }
                    await sender.SendAsync(Encoding.UTF8.GetBytes(response), endPoint);
                    return;
                }
                else
                {
                    continue;
                }
            }
        }
        private bool ParseResponse(string response, out IPEndPoint? endPoint)
        {
            endPoint = null;
            if (response.Equals("n"))
            {
                return true;
            }
            else if (response.Equals("y"))
            {
                endPoint = new IPEndPoint(ChatIp!, _broadcastPort);
                return true;
            }
            else
            {
                Console.WriteLine("Give response in format: [y/n]");
            }
            return false;
        }
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
