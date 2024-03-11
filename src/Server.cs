using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_http_server_csharp;

//Start server
Console.WriteLine("Starting server...");
TcpListener server = new(IPAddress.Any, 4221);
server.Start();
Console.WriteLine("Server started on port 4221");

while (true)
{
    //Wait for client request
    Socket socket = await server.AcceptSocketAsync();
    Console.WriteLine("Client connected");

    //Prepare buffer for request
    byte[] buffer = new byte[1024];

    //Read request. Socket flag is ued to specify how the socket should behave
    await socket.ReceiveAsync(buffer, SocketFlags.None);

    //Parse request
    Request req = Request.Parse(buffer);

    Console.WriteLine($"Request:\nMethod: {req.Method}\nPath: {req.Path}\nVersion {req.Version}");

    switch (req.Path)
    {
        case "/":
            await socket.SendAsync("HTTP/1.1 200OK\r\n\r\nYou called the root path");
            break;
        case "/index.html":
            await socket.SendAsync("HTTP/1.1 200OK\r\n\r\nYou called the index.html path");
            break;
        default:
            await socket.SendAsync("HTTP/1.1 404NOTFOUND\r\n\r\nPath not found");
            break;
    }
}

class Request
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string Version { get; set; } = "";
    public Dictionary<string, string> Headers { get; set; } = new();

    public static Request Parse(byte[] requestBytes)
    {
        //Transform request to string
        string requestString = Encoding.UTF8.GetString(requestBytes);
        //Split request into lines
        string[] lines = requestString.Split("\r\n");
        //Split first line () into parts
        string[] startLine = lines[0].Split(" ");
        //Create new request object
        Request request = new()
        {
            Method = startLine[0],
            Path = startLine[1],
            Version = startLine[2]
        };

        foreach (string line in lines.Skip(1))
        {
            string[] header = line.Split(": ");
            if (header.Length == 2)
            {
                request.Headers.Add(header[0], header[1]);
            }
        }
        return request;
    }

}

//Unused, to be used later when reponse are more complex
class Response
{
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; } = "";
}