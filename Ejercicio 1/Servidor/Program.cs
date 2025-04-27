using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Servidor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Servidor] Loading...");

            // Listen on 5000
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("[Servidor] Awaiting connections...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[Servidor] Client connected");

                // Start a new thread for each client
                Thread clienteThread = new(() => ManejarCliente(client));
                clienteThread.Start();
            }
        }

        static void ManejarCliente(TcpClient client)
        {
            try
            {
                Console.WriteLine("[Servidor] Managing new vehicle...");

                NetworkStream ns = client.GetStream();

                // Get a message
                string message = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Received message: {message}");

                // Send a response
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "Hello client, connection ack");

                // Close the connection when done.
                ns.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error: {ex.Message}");
            }
        }
    }
}