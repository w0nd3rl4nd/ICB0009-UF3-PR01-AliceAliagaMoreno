using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;
using System.Text.Json;

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

            string json = Encoding.UTF8.GetString(datos); // Convert the byte array to a JSON string
            Carretera carretera = JsonSerializer.Deserialize<Carretera>(json); // Deserialize the JSON string into a Carretera object

            return carretera; // Return the deserialized Carretera object
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

            string json = Encoding.UTF8.GetString(datos); // Convert the byte array to a JSON string
            Vehiculo vehiculo = JsonSerializer.Deserialize<Vehiculo>(json); // Deserialize the JSON string into a Vehiculo object

            return vehiculo; // Return the deserialized Vehiculo object
        }

        //Método que permite leer un mensaje de tipo texto (string) de un NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] bufferLectura = new byte[1024]; // Buffer to temporarily store data read from the stream
            int bytesLeidos = 0; // Number of bytes read

            using (MemoryStream tmpStream = new MemoryStream()) // Create a temporary MemoryStream to accumulate the read data
            {
                int bytes = NS.Read(bufferLectura, 0, bufferLectura.Length); // Read bytes from the stream
                tmpStream.Write(bufferLectura, 0, bytes); // Write them to the MemoryStream
                bytesLeidos += bytes; // Update the number of bytes read

                // Continue reading if more data is available
                while (NS.DataAvailable)
                {
                    bytes = NS.Read(bufferLectura, 0, bufferLectura.Length); // Read additional bytes
                    tmpStream.Write(bufferLectura, 0, bytes); // Append them to the MemoryStream
                    bytesLeidos += bytes; // Update the number of bytes read
                }

                return Encoding.Unicode.GetString(tmpStream.ToArray(), 0, bytesLeidos); // Convert the accumulated bytes to a Unicode string
            }
        }

        //Método que permite escribir un mensaje de tipo texto (string) al NetworkStream
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] MensajeBytes = Encoding.Unicode.GetBytes(Str); // Encode the string into a byte array using Unicode
            NS.Write(MensajeBytes, 0, MensajeBytes.Length); // Write the byte array to the NetworkStream
        }                      

    }
}