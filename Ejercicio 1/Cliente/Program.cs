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

                NetworkStream ns = client.GetStream();

                // Send message to server
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "Hello Server, ack");

                // Receive response
                string respuesta = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Response from server: {respuesta}");

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