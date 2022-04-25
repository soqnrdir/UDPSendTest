using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            // (1) UdpClient 객체 성성
            UdpClient udp = new UdpClient();

            // (2) Multicast 종단점 설정 (목적지인 IP:PORT 설정)          
            //IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("229.1.1.229"), 5500);
            IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("10.128.101.145"), 5500);

            //int ifIndex = 4;
            //udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(ifIndex));
            //Console.WriteLine("ifIndex: " + ifIndex);
            //var multicastAddress = IPAddress.Parse("<group IP>");
            //var multOpt = new MulticastOption(multicastAddress, ifaceIndex);
            //udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multOpt);


            for (int i = 1; i <= 60; i++)
            {
                byte[] dgram = Encoding.ASCII.GetBytes("Msg#" + i);

                // (3) Multicast 그룹에 데이타그램 전송      
                udp.Send(dgram, dgram.Length, multicastEP);

                Console.WriteLine("Msg#" + i + "송신중...");
                Thread.Sleep(1000);
            }
        }
    }
}
