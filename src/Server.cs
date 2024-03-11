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

    //PATH "/"
    if (req.Path.Equals("/"))
    {
        Console.WriteLine("Root path called");
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("You called the root path"),
            StatusCode = HttpStatusCode.OK
        };
        await socket.SendAsync(response);
    }
    //PATH "/index.html"
    else if (req.Path.Equals("/index.html"))
    {
        Console.WriteLine("Index page called");
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("This is the index page")
        };
        await socket.SendAsync(response);
    }
    //PATH "/echo/{message}"
    else if (req.Path.Contains("/echo/"))
    {
        Console.WriteLine("Echo called. Response was: " + req.Path.Replace("/echo/", ""));
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(req.Path.Replace("/echo/", ""), Encoding.UTF8, "text/plain"),
        };
        await socket.SendAsync(response);
    }
    //PATH /user-agent
    else if (req.Path.Equals("/user-agent"))
    {
        Console.WriteLine("User agent called");
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(req.Headers["User-Agent"]),
        };
        await socket.SendAsync(response);
    }
    //PATH others
    else
    {
        Console.WriteLine("Path not found");
        HttpResponseMessage response = new(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Path not found")
        };
        await socket.SendAsync(response);
    }

    socket.Close();
}