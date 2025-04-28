﻿using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;


namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        
        //Método para escribir en un NetworkStream los datos de tipo Carretera
        public static void  EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {            
                            
        }

        //Metódo para leer de un NetworkStream los datos que de un objeto Carretera
        /*public static Carretera LeerDatosCarreteraNS (NetworkStream NS)
        {
            

        }*/

        //Método para enviar datos de tipo Vehiculo en un NetworkStream
        public static void  EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {            
                              
        }

        //Metódo para leer de un NetworkStream los datos que de un objeto Vehiculo
        /*public static Vehiculo LeerDatosVehiculoNS (NetworkStream NS)
        {

        }*/

        //Método que permite leer un mensaje de tipo texto (string) de un NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] bufferLectura = new byte[1024];
            int bytesLeidos = 0;

            using (MemoryStream tmpStream = new MemoryStream())
            {
                int bytes = NS.Read(bufferLectura, 0, bufferLectura.Length);
                tmpStream.Write(bufferLectura, 0, bytes);
                bytesLeidos += bytes;

                while (NS.DataAvailable)
                {
                    bytes = NS.Read(bufferLectura, 0, bufferLectura.Length);
                    tmpStream.Write(bufferLectura, 0, bytes);
                    bytesLeidos += bytes;
                }

                return Encoding.Unicode.GetString(tmpStream.ToArray(), 0, bytesLeidos);
            }
        }

        //Método que permite escribir un mensaje de tipo texto (string) al NetworkStream
        public static void  EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {            
            byte[] MensajeBytes = Encoding.Unicode.GetBytes(Str);
            NS.Write(MensajeBytes,0,MensajeBytes.Length);                        
        }                          

    }
}
