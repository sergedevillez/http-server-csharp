using System.Net.Sockets;
using System.Text;

namespace codecrafters_http_server_csharp;

public static class SocketExtensions
{
    public static async Task SendAsync(this Socket s, string response)
    {
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await s.SendAsync(responseBytes, SocketFlags.None);
    }
}
