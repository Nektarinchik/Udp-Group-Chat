using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UdpGroupChat.Extensions;
using UdpGroupChat.Udp;

namespace UdpGroupChat.Common
{
    internal static class Init
    {
        private static readonly IPAddress _startIpAddressForBroadcast = IPAddress.Parse("224.0.0.0");
        private static readonly IPAddress _endIpAddressForBroadcast = IPAddress.Parse("239.255.255.255");
        public async static Task Start()
        {
            Console.WriteLine("Choose option:\n\t1. Create broadcast\n\t2. Request access to broadcast");
            string? option = "";
            while (true)
            {
                option = Console.ReadLine();
                if (!ReferenceEquals(option, null))
                    if (option.Equals("1") || option.Equals("2"))
                    {
                        break;
                    }
                Console.WriteLine("Choose options only from list");
            }

            switch (option)
            {
                case "1":
                    await CreateBroadcastAsync(); 
                    break;
                case "2":
                    await JoinToBroadcastAsync();
                    break;
            }
        }

        private static async Task JoinToBroadcastAsync()
        {
            Console.Write("Enter your name: ");
            var username = Console.ReadLine()!;

            int portToSendRequest = GetPortNumberFromConsole("Enter port number to send request: ");

            ClientConnecting client = new ClientConnecting(username);

            var response = client.RequestAccessAsync(portToSendRequest);

            _ = IPEndPoint.TryParse(await response, out var ipEndPoint);

            if (!ReferenceEquals(ipEndPoint, null))
            {
                await client.ReceiveBroadcastFromGroupAsync(ipEndPoint.Address, ipEndPoint.Port);
            }
        }

        private static async Task CreateBroadcastAsync()
        {
            Console.Write("Enter your name: ");
            var username = Console.ReadLine()!;

            int requestsPort = GetPortNumberFromConsole("Enter port to receive requests: ");

            int broadcastPort = GetPortNumberFromConsole("Enter port of broadcast: ");

            IPAddress broadcastAddress = GetBroadcastIpAdressFromConsole("Enter IP-address of broadcast: ");

            ClientCreator creator = new ClientCreator(username, requestsPort, broadcastPort, broadcastAddress);

            Task.Run(creator.ReceiveRequests);
            await creator.SendToBroadcastAsync();
        }

        private static IPAddress GetBroadcastIpAdressFromConsole(string? inputMessage=null)
        {
            inputMessage ??= "Enter IP-address of group: ";
            Console.Write(inputMessage);
            
            while (true)
            {
                try
                {
                    string address = Console.ReadLine()!;
                    var result = IPAddress.Parse(address);
                    if (!result.IsIpInRange(_startIpAddressForBroadcast, _endIpAddressForBroadcast))
                    {
                        Console.WriteLine("Enter IP-address in range [224.0.0.0]-[239.255.255.255]");
                    }
                    else
                        return result;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Enter IP-address in correct format xxx.xxx.xxx.xxx where xxx = [0, 255]");
                }
            }
        }

        private static int GetPortNumberFromConsole(string? inputMessage=null)
        {
            inputMessage ??= "Enter port to request: ";
            Console.Write(inputMessage);

            while (true)
            {
                try
                {
                    return Convert.ToUInt16(Console.ReadLine()!);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Enter number from range [0, 65535]");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Enter number from range [0, 65535]");
                }
            }
        }
    }
}
