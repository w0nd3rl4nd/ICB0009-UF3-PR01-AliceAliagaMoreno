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
            int vehicleId = 0;

            try
            {
                Console.WriteLine("[Servidor] Managing new vehicle...");

                
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

                // I am already getting the network stream lol
                NetworkStream ns = client.GetStream();

                // 1) Read INIT from client
                string init = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake received: {init}");

                // 2) Send ID and bearing
                string handshakeData = $"ID:{vehicleId}, Bearing:{bearing}";
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, handshakeData);
                Console.WriteLine($"[Servidor] Sent handshake data: {handshakeData}");

                // 3) Reac ACK from client (message readback)
                string confirm = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake confirmation: {confirm}");

                // 4) Send final ACK
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "HANDSHAKE_COMPLETED");
                Console.WriteLine("[Servidor] Handshake completed");

                var newCLient = new Cliente {
                    Id       = vehicleId,
                    Stream   = ns,
                    TcpClient= client
                };

                lock (clientesLock)
                {
                    connectedClients.Add(new Cliente { Id = vehicleId, Stream = ns, TcpClient = client });
                    Console.WriteLine($"[Servidor] Connected clients: {connectedClients.Count}");
                }

                // Detectar desconexión
                var socket = client.Client;
                try
                {
                    // Bucle que comprueba cada segundo si el peer ha cerrado
                    while (true)
                    {
                        // SelectRead + Available==0 => FIN recibido
                        if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    // Al salir del bucle, realmente se desconectó
                    lock (clientesLock)
                    {
                        connectedClients.RemoveAll(c => c.Id == vehicleId);
                        Console.WriteLine($"[Servidor] Client {vehicleId} disconnected. Remaining {connectedClients.Count}");
                    }
                    ns.Close();
                    client.Close();
                }
            
            } catch (Exception ex) {
                Console.WriteLine($"[Servidor] Error in client {vehicleId}: {ex.Message}");
            }
        }
    }
}