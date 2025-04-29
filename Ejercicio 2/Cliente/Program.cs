using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("[Cliente] Initializing connection...");

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 5000); // Connect to local server
                Console.WriteLine("[Cliente] Connecting to the server...");

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

                // Now create a new Vehiculo with the received ID and bearing
                var vehiculo = new Vehiculo
                {
                    Id = int.Parse(serverData.Split(",")[0].Split(":")[1].Trim()),
                    Direccion = serverData.Split(",")[1].Split(":")[1].Trim()
                };

                // Send the Vehiculo back to the server
                NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                Console.WriteLine("[Cliente] Sent new Vehiculo to the server.");

                // Start moving the vehicle
                for (int i = 0; i <= 100; i++)
                {
                    // Update vehicle position
                    vehiculo.Pos = i;
                    // Simulate movement speed with Thread.Sleep based on Velocidad
                    Thread.Sleep(vehiculo.Velocidad);

                    // Send updated Vehiculo to server
                    NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                    Console.WriteLine($"[Cliente] Vehicle updated: Pos = {vehiculo.Pos}, Vel = {vehiculo.Velocidad}");

                    if (vehiculo.Pos == 100)
                    {
                        vehiculo.Acabado = true;
                        NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo); // Send vehicle data one last time so server exits the while !Acabado loop
                        Console.WriteLine("[Cliente] Vehicle has finished its journey.");
                        break;
                    }
                }

                // Wait for user input before disconnecting
                Console.WriteLine("Press enter to disconnect...");
                Console.ReadLine();

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
