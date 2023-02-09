//using System.Dynamic;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using UdpGroupChat.Common;
//using UdpGroupChat.Extensions;

//namespace UdpGroupChat.Udp
//{
//    internal sealed class Client
//    {
//        private static readonly IPAddress _startIpAddressForBroadcast = IPAddress.Parse("224.0.0.0");
//        private static readonly IPAddress _endIpAddressForBroadcast = IPAddress.Parse("239.255.255.255");
//        private int _portForGiveAccess;
//        private int _portForChat;
//        private IPAddress? _chatIp;
//        private IPAddress? ChatIp
//        {
//            get { return _chatIp; }
//            set 
//            {
//                if (ReferenceEquals(value, null))
//                {
//                    throw new ArgumentNullException(nameof(value));
//                }

//                if (!value.IsIpInRange(_startIpAddressForBroadcast, _endIpAddressForBroadcast))
//                {
//                    throw new SocketException((int)SocketError.OperationNotSupported);
//                }

//                _chatIp = value;
//            }
//        }
//        public User User { get; set; } = new User();
//        public void Connect(int portForChat = 0, int portForGiveAccess = 0, IPAddress? chatIp = null)
//        {
//            _portForChat = portForChat;
//            _portForGiveAccess = portForGiveAccess;
//            ChatIp = chatIp;
//        }

//        public async Task SendToBroadcastAsync()
//        {
//            if (ReferenceEquals(ChatIp, null))
//            {
//                throw new NullReferenceException("Undefined value of GhatIp");
//            }

//            // create group
//            using var udpSender = new UdpClient(new IPEndPoint(ChatIp, _portForChat));
//            Console.WriteLine($"Group created by {User?.Name}");

//            // creator send messages
//            StringBuilder message = new StringBuilder();
//            byte[] data;
//            while (true)
//            {
//                message.Clear();
//                message.Append(Console.ReadLine()!);

//                if (string.IsNullOrWhiteSpace(message.ToString()))
//                    break;

//                message.Append($": {User?.Name}");
//                data = Encoding.UTF8.GetBytes(message.ToString());
//                await udpSender.SendAsync(data);
//            }
//        }
//        public async Task ReceiveBroadcastFromGroup(IPAddress broadcastIp, int broadcastPort)
//        {
//            using var receiver = new UdpClient(broadcastPort);
//            receiver.JoinMulticastGroup(broadcastIp);
//            while (true)
//            {
//                var result = await receiver.ReceiveAsync();
//                string message = Encoding.UTF8.GetString(result.Buffer);
//                Console.WriteLine(message);
//            }
//        }

//        public async Task ReceiveRequests()
//        {
//            using var receiver = new UdpClient(_portForGiveAccess);
//            while (true)
//            {
//                var result = await receiver.ReceiveAsync();
//                string message = Encoding.UTF8.GetString(result.Buffer);
//                Console.WriteLine(message);
//                if (string.IsNullOrWhiteSpace(message))
//                {
//                    continue;
//                }
//                await SendResponse(result.RemoteEndPoint);
//            }
//        }
//        public async Task SendResponse(IPEndPoint endPoint)
//        {
//            using UdpClient sender = new UdpClient(endPoint);
//            IPEndPoint? receiverEndPoint = null;

//            while (true)
//            {
//                var message = Console.ReadLine();
//                if (string.IsNullOrWhiteSpace(message))
//                {
//                    Console.WriteLine("Give response in format: [y/n]");
//                    continue;
//                }
//                bool isParseSuccessed = ParseResponse(message, out receiverEndPoint);
//                if (isParseSuccessed)
//                {
//                    string response = "";
//                    if (ReferenceEquals(receiverEndPoint, null))
//                    {
//                        response = $"Access denied: {User.Name}";
//                    }
//                    else
//                    {
//                        response = $"{receiverEndPoint}";
//                    }
//                    await sender.SendAsync(Encoding.UTF8.GetBytes(response));
//                }
//                else
//                {
//                    continue;
//                }
//            }
//        }

//        public async Task RequestAccessAsync(int portToRequest)
//        {
//            using UdpClient sender = new UdpClient(portToRequest);
//            var message = $"User {User.Name} want to get access to broadcast: [y/n]";
//            byte[] data = Encoding.UTF8.GetBytes(message);
//            await sender.SendAsync(data);
//        }

//        public async Task WaitResponse(int portToWait)
//        {
//            using UdpClient receiver = new UdpClient(portToWait);

//            string response = "";
//            while (true)
//            {
//                var result = await receiver.ReceiveAsync();
//                response = Encoding.UTF8.GetString(result.Buffer);
//                if (string.IsNullOrWhiteSpace(response))
//                {
//                    continue;
//                }
//                Console.WriteLine(response);
//                break;
//            }
//        }

//        private bool ParseResponse(string response, out IPEndPoint? endPoint)
//        {
//            endPoint = null;
//            if (response.Equals("n"))
//            {
//                return false;
//            }
//            else if (response.Equals("y"))
//            {
//                endPoint = new IPEndPoint(ChatIp!, _portForChat);
//                return true;
//            }
//            else
//            {
//                Console.WriteLine("Give response in format: [y/n]");
//            }
//            return false;
//        }
//    }
//}
