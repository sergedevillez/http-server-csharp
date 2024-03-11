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
    await Task.Run(() => HandleClient(socket));
}

static async Task HandleClient(Socket socket)
{
    //Prepare buffer for request
    byte[] buffer = new byte[1024];

    //Read request. Socket flag is ued to specify how the socket should behave
    await socket.ReceiveAsync(buffer, SocketFlags.None);

    //Parse request
    Request req = Request.Parse(buffer);

    Console.WriteLine($"Request:\nMethod: {req.Method}\nPath: {req.Path}\nVersion {req.Version}");

    //Handle the request
    var response = socket.HandleRequest(req);

    await socket.SendAsync(response);
    await Task.Delay(1000);
    socket.Close();
}