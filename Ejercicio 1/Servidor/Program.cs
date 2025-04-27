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
        static int nextId = 1; // ID for each vehicle
        static readonly object idLock = new object();  // Object to lock ID access
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

                int vehicleId;
                string bearing;

                lock (idLock)
                {
                    vehicleId = nextId++;
                    bearing = (new Random().Next(0, 2) == 0) ? "North" : "South";
                }

                Console.WriteLine($"[Servidor] Vehicle with ID {vehicleId} is assigned {bearing} direction");

                Vehiculo vehiculo = new Vehiculo
                {
                    Id = vehicleId,
                    Direccion = bearing
                };

                NetworkStream ns = client.GetStream();

                // Send the ID to the client as a handshake
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, $"ID: {vehicleId}, Direction: {bearing}");

                // Get a message
                string message = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Received message: {message}");

                // Send a response
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "ID and direction assigned succesfully");

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