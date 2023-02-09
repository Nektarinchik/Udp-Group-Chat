using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UdpGroupChat.Common;

namespace UdpGroupChat.Udp
{

    // TODO: instead of _localHost make GetCurrentIpAddress and try to make it on two computers
    internal sealed class ClientConnecting
    {
        private static readonly IPAddress _localHost = IPAddress.Parse("127.0.0.1");
        private async Task<string> WaitResponseAsync(UdpClient receiver)
        {
            string response = "";
            while (true)
            {
                var result = await receiver.ReceiveAsync();
                response = Encoding.UTF8.GetString(result.Buffer);
                if (string.IsNullOrWhiteSpace(response))
                {
                    continue;
                }
                Console.WriteLine(response);
                return response;
            }
        }
        public User User { get; set; } = new User();
        public ClientConnecting(string name)
        {
            User.Name = name;
        }
        public async Task<string> RequestAccessAsync(int portToRequest)
        {
            using UdpClient sender = new UdpClient();
            var message = $"User {User.Name} want to get access to broadcast: [y/n]";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await sender.SendAsync(data, new IPEndPoint(_localHost, portToRequest));
            return await WaitResponseAsync(sender);
        }

        public async Task ReceiveBroadcastFromGroupAsync(IPAddress broadcastIp, int broadcastPort)
        {
            using var receiver = new UdpClient(broadcastPort);
            receiver.JoinMulticastGroup(broadcastIp);
            while (true)
            {
                var result = await receiver.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine(message);
            }
        }

    }
}
