using System;
using System.Net.Sockets;
using System.Net;
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

            Cliente cliente = null;

            try
            {
                Console.WriteLine("[Servidor] Managing new vehicle...");

                lock (idLock)
                {
                    vehicleId = nextId++;
                    bearing = (new Random().Next(0, 2) == 0) ? "North" : "South";
                }

                Console.WriteLine($"[Servidor] Vehicle with ID {vehicleId} is assigned {bearing} direction");

                NetworkStream ns = client.GetStream();

                // Crear objeto Cliente y añadirlo a la lista global
                cliente = new Cliente
                {
                    Id = vehicleId,
                    TcpClient = client,
                    Stream = ns
                };

                lock (clientesLock)
                {
                    connectedClients.Add(cliente);
                }

                // Handshake
                string init = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake received: {init}");

                string handshakeData = $"ID:{vehicleId}, Bearing:{bearing}";
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, handshakeData);
                Console.WriteLine($"[Servidor] Sent handshake data: {handshakeData}");

                string confirm = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Servidor] Handshake confirmation: {confirm}");

                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "HANDSHAKE_COMPLETED");
                Console.WriteLine("[Servidor] Handshake completed");

                // Recibir vehículo
                Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                Console.WriteLine($"[Servidor] Received Vehiculo with ID {vehiculo.Id}, Bearing {vehiculo.Direccion}");

                carretera.AñadirVehiculo(vehiculo);
                Console.WriteLine("[Servidor] Vehicle added to the carretera.");

                // Ciclo de actualización
                while (!vehiculo.Acabado)
                {
                    vehiculo = NetworkStreamClass.LeerDatosVehiculoNS(ns);
                    Console.WriteLine($"[Servidor] Vehicle updated: Pos = {vehiculo.Pos}, Vel = {vehiculo.Velocidad}");

                    carretera.ActualizarVehiculo(vehiculo);
                    EnviarDatosACadaCliente();
                    carretera.MostrarBicicletas();
                }

                // Esperar desconexión
                var socket = client.Client;
                try
                {
                    while (true)
                    {
                        if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0)
                            break;
                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    // Quitar cliente de la lista
                    lock (clientesLock)
                    {
                        connectedClients.Remove(cliente);
                    }

                    ns.Close();
                    client.Close();
                    Console.WriteLine("[Servidor] Client disconnected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Error: {ex.Message}");

                // En caso de error, asegurar eliminación del cliente
                if (cliente != null)
                {
                    lock (clientesLock)
                    {
                        connectedClients.Remove(cliente);
                    }
                }
            }
        }

        static void EnviarDatosACadaCliente()
        {
            lock (clientesLock)
            {
                foreach (var cliente in connectedClients)
                {
                    try
                    {
                        // Enviar los datos de la carretera a cada cliente
                        NetworkStreamClass.EscribirDatosCarreteraNS(cliente.Stream, carretera);
                        Console.WriteLine("[Servidor] Sending data to a client.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Servidor] Error sending data: {ex.Message}");
                    }
                }
            }
        }
    }
}
