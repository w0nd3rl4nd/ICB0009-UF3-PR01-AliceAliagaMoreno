using System.Net.Sockets;

namespace Servidor
{
    public class Cliente
    {
        public int Id { get; set; }
        public NetworkStream Stream { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}