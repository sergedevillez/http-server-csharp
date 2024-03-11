using System.Net.Sockets;
using System.Text;

namespace codecrafters_http_server_csharp;

public static class SocketExtensions
{
    public static async Task SendAsync(this Socket s, HttpResponseMessage res)
    {
        string content = await res.Content.ReadAsStringAsync();
        StringBuilder response = new();
        if (res.Content is not null && !string.IsNullOrEmpty(content))
        {
            response.Append($"HTTP/1.1 {(int)res.StatusCode} {res.StatusCode}" +
                             $"\r\nContent-Type: text/plain" +
                             $"\r\nContent-Length: {content.Length}" +
                             "\r\n" +
                             $"\r\n{content}");
        }
        else
        {
            response.Append($"HTTP/1.1 {(int)res.StatusCode} {res.StatusCode}\r\n\r\n");
        }
        byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
        await s.SendAsync(responseBytes, SocketFlags.None);
    }
}
