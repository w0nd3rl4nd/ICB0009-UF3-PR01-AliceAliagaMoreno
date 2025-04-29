using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;
using System.Text.Json;
using System.Collections.Generic;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        
        //Método para escribir en un NetworkStream los datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            // Use JsonSerializer to serialize the object to a JSON string
            string json = JsonSerializer.Serialize(C);
            byte[] datos = Encoding.UTF8.GetBytes(json); // Convert the JSON string to a byte array
            int longitud = datos.Length; // Get the length of the byte array

            EscribirMensajeNetworkStream(NS, longitud.ToString()); // Write the length to the NetworkStream first
            NS.Write(datos, 0, datos.Length); // Write the actual serialized data to the NetworkStream
        }

        //Método para leer de un NetworkStream los datos que de un objeto Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            string longitudStr = LeerMensajeNetworkStream(NS); // Read the length of the incoming data
            int longitud = int.Parse(longitudStr); // Parse the length into an integer
            byte[] datos = new byte[longitud]; // Create a byte array of the expected length
            int totalLeido = 0; // Track how many bytes have been read so far

            while (totalLeido < longitud)
            {
                int leido = NS.Read(datos, totalLeido, longitud - totalLeido); // Read bytes from the stream
                if (leido == 0) break; // If no bytes are read, exit the loop
                totalLeido += leido; // Update the total number of bytes read
            }

            // Ensure that the bytes read are complete and valid
            string json = Encoding.UTF8.GetString(datos);
            try
            {
                Carretera carretera = JsonSerializer.Deserialize<Carretera>(json); // Deserialize the JSON string into a Carretera object
                return carretera;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Servidor] Deserialization error: {ex.Message}");
                return null; // Return null or handle error as needed
            }
        }

        //Método para enviar datos de tipo Vehiculo en un NetworkStream
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            // Use JsonSerializer to serialize the object to a JSON string
            string json = JsonSerializer.Serialize(V);
            byte[] datos = Encoding.UTF8.GetBytes(json); // Convert the JSON string to a byte array
            int longitud = datos.Length; // Get the length of the byte array

            EscribirMensajeNetworkStream(NS, longitud.ToString()); // Write the length to the NetworkStream first
            NS.Write(datos, 0, datos.Length); // Write the actual serialized data to the NetworkStream
        }

        //Método para leer de un NetworkStream los datos que de un objeto Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            string longitudStr = LeerMensajeNetworkStream(NS); // Read the length of the incoming data
            int longitud = int.Parse(longitudStr); // Parse the length into an integer
            byte[] datos = new byte[longitud]; // Create a byte array of the expected length
            int totalLeido = 0; // Track how many bytes have been read so far

            while (totalLeido < longitud)
            {
                int leido = NS.Read(datos, totalLeido, longitud - totalLeido); // Read bytes from the stream
                if (leido == 0) break; // If no bytes are read, exit the loop
                totalLeido += leido; // Update the total number of bytes read
            }

            // Ensure that the bytes read are complete and valid
            string json = Encoding.UTF8.GetString(datos);
            try
            {
                Vehiculo vehiculo = JsonSerializer.Deserialize<Vehiculo>(json); // Deserialize the JSON string into a Vehiculo object
                return vehiculo;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Servidor] Deserialization error: {ex.Message}");
                return null; // Return null or handle error as needed
            }
        }

        //Método que permite leer un mensaje de tipo texto (string) de un NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            // Modify this entire class because the method was unreliable
            byte[] buffer = new byte[1]; // read byte per byte
            List<byte> datos = new List<byte>();

            while (true)
            {
                int bytesRead = NS.Read(buffer, 0, 1);
                if (bytesRead == 0)
                    break; // closed connection

                if (buffer[0] == '\n')
                    break; // end of message

                datos.Add(buffer[0]);
            }

            return Encoding.UTF8.GetString(datos.ToArray()).Trim();
        }

        //Método que permite escribir un mensaje de tipo texto (string) al NetworkStream
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] MensajeBytes = Encoding.UTF8.GetBytes(Str + "\n"); // Encode the string into a byte array using UTF8
            NS.Write(MensajeBytes, 0, MensajeBytes.Length); // Write the byte array to the NetworkStream
        }                      

    }
}