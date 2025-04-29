using System;
using System.Net.Sockets;
using System.Threading;
using NetworkStreamNS;
using VehiculoClass;
using System.Net;
using CarreteraClass;

namespace Client
{
    class Program
    {
        static volatile bool listening = true;

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("[Cliente] Initializing connection...");

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 5000);
                Console.WriteLine("[Cliente] Connecting to the server...");

                NetworkStream ns = client.GetStream();

                // Handshake
                NetworkStreamClass.EscribirMensajeNetworkStream(ns, "INIT");
                Console.WriteLine("[Cliente] Sent handshake initiation");

                string serverData = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Received handshake data: {serverData}");

                NetworkStreamClass.EscribirMensajeNetworkStream(ns, serverData);
                Console.WriteLine($"[Cliente] Sent handshake confirmation: {serverData}");

                string finalAck = NetworkStreamClass.LeerMensajeNetworkStream(ns);
                Console.WriteLine($"[Cliente] Handshake completed: {finalAck}");

                // Vehiculo setup
                var vehiculo = new Vehiculo
                {
                    Id = int.Parse(serverData.Split(",")[0].Split(":")[1].Trim()),
                    Direccion = serverData.Split(",")[1].Split(":")[1].Trim()
                };

                NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                Console.WriteLine("[Cliente] Sent new Vehiculo to the server.");

                // Thread to listen for Carretera updates
                Thread escuchaCarretera = new Thread(() => EscucharDatosCarretera(ns));
                escuchaCarretera.Start();

                // Simulate vehicle movement
                for (int i = 0; i <= 100; i++)
                {
                    vehiculo.Pos = i;
                    Thread.Sleep(vehiculo.Velocidad);

                    NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                    Console.WriteLine($"[Cliente] Vehicle updated: Pos = {vehiculo.Pos}, Vel = {vehiculo.Velocidad}");

                    if (vehiculo.Pos == 100)
                    {
                        vehiculo.Acabado = true;
                        NetworkStreamClass.EscribirDatosVehiculoNS(ns, vehiculo);
                        Console.WriteLine("[Cliente] Vehicle has finished its journey.");
                        break;
                    }
                }

                // Notify the listening thread to stop
                listening = false;

                // Wait for the listening thread to finish before proceeding
                escuchaCarretera.Join();

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

        static void EscucharDatosCarretera(NetworkStream ns)
        {
            try
            {
                while (listening)
                {
                    Carretera carretera = NetworkStreamClass.LeerDatosCarreteraNS(ns);
                    Console.WriteLine("[Cliente] Received new Carretera data:");
                    carretera.MostrarBicicletas();
                }
            }
            catch (Exception ex)
            {
                if (listening)
                {
                    Console.WriteLine($"[Cliente] Error receiving data from server: {ex.Message}");
                }
                else
                {
                    Console.WriteLine("[Cliente] Listening stopped.");
                }
            }
        }
    }
}
