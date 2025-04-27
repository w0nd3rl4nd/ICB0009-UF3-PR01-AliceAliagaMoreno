using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Cliente] Initialising connection...");

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 5000); // Connect to local server
                Console.WriteLine("[Cliente] Connecting to the server...");

                // I am already getting the network stream lol
                NetworkStream ns = client.GetStream();

                // 1) Send "INIT" to begin connection
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INIT");
                Console.WriteLine("[Cliente] Sent handshake initiation");

                // 2) Read answer (ID and Bearing)
                string serverData = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Received handshake data: {serverData}");

                // 3) Resend confirmation (readback)
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, serverData);
                Console.WriteLine($"[Cliente] Sent handshake confirmation: {serverData}");

                // 4) Read final ACK
                string finalAck = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Handshake completed: {finalAck}");

                ns.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cliente] Error: {ex.Message}");
            }
        }
    }
}