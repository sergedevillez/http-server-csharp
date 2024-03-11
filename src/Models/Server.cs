using System.Net;
using System.Net.Sockets;

namespace codecrafters_http_server_csharp.Models;

public class Server
{
    private readonly string[] _args;
    private TcpListener _server { get; set; }

    public Server(string[] args, int port, IPAddress ipAddress)
    {
        _args = args;
        _server = new TcpListener(ipAddress, port);
    }

    //Need to be called to start the server
    public async Task Start()
    {
        _server.Start();
        while (true)
        {
            Socket socket = await _server.AcceptSocketAsync();
            Task.Run(async () => await HandleClientConnection(socket));
        }
    }

    private async Task HandleClientConnection(Socket socket)
    {
        //Prepare buffer for request
        byte[] buffer = new byte[1024];

        //Add timeout to socket
        socket.ReceiveTimeout = 3000;

        //Read request. Socket flag is ued to specify how the socket should behave
        await socket.ReceiveAsync(buffer, SocketFlags.None);

        //Parse request
        Request req = Request.Parse(buffer);

        Console.WriteLine($"\nMethod: {req.Method}\nPath: {req.Path}\nVersion {req.Version}");

        //Get necessary arg
        string? filesDirectory = _args.Contains("--directory") ? _args[Array.IndexOf(_args, "--directory") + 1] : null;

        //Handle the request
        HttpResponseMessage response = socket.HandleRequest(req, filesDirectory);

        await socket.SendAsync(response);
        socket.Close();
    }

    public void Stop()
    {
        _server.Stop();
    }
}
