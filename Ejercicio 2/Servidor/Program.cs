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
        static List<Cliente> connectedClients = new List<Cliente>();
        static readonly object clientesLock = new object();
        static Carretera carretera = new Carretera();  // Global Carretera object

        static void Main(string[] args)
        {
            Console.Clear();
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
            int vehicleId = 0;
            string bearing = "";

            try
            {
                Console.WriteLine("[Servidor] Managing new vehicle...");

                lock (idLock)
                {
                    vehicleId = nextId++;
                    bearing = (new Random().Next(0, 2) == 0) ? "North" : "South";
                }

                Console.WriteLine($"[Servidor] Vehicle with ID {vehicleId} is assigned {bearing} direction");

                // 1) Read INIT from client
                NetworkStream ns = client.GetStream();
                string init = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake received: {init}");

                // 2) Send ID and bearing
                string handshakeData = $"ID:{vehicleId}, Bearing:{bearing}";
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, handshakeData);
                Console.WriteLine($"[Servidor] Sent handshake data: {handshakeData}");

                // 3) Read ACK from client (message readback)
                string confirm = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake confirmation: {confirm}");

                // 4) Send final ACK
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "HANDSHAKE_COMPLETED");
                Console.WriteLine("[Servidor] Handshake completed");

                // 5) Receive the new vehicle from client
                Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                Console.WriteLine($"[Servidor] Received Vehiculo with ID {vehiculo.Id}, Bearing {vehiculo.Direccion}");

                // Add the received vehicle to the carretera
                carretera.AñadirVehiculo(vehiculo);
                Console.WriteLine("[Servidor] Vehicle added to the carretera.");

                // Display all vehicles in the carretera
                carretera.MostrarBicicletas();

                // Clean up
                var socket = client.Client;
                try
                {
                    // Detect disconnection
                    while (true)
                    {
                        if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    ns.Close();
                    client.Close();
                    Console.WriteLine("[Servidor] Client disconnected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error: {ex.Message}");
            }
        }
    }
}
