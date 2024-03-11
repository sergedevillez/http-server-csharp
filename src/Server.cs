using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_http_server_csharp;



var argsList = args.ToList();
string filesDirectory;
if (argsList.Contains("--directory"))
{
    int directoryIndex = argsList.IndexOf("--directory");
    var filesDirectoryArgument = argsList[directoryIndex + 1];
    filesDirectory = !string.IsNullOrWhiteSpace(filesDirectoryArgument) ? filesDirectoryArgument : "wwwroot";
}

//Start server
Console.WriteLine("Starting server...");
TcpListener server = new(IPAddress.Any, 4221);
server.Start();
Console.WriteLine("Server started on port 4221");

while (true)
{
    //Wait for client request
    Socket socket = await server.AcceptSocketAsync();

    Task.Run(async () => await HandleClient(socket));
}

static async Task HandleClient(Socket socket)
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

    //Handle the request
    HttpResponseMessage response = socket.HandleRequest(req, filesDirectory);

    await socket.SendAsync(response);
    socket.Close();
}