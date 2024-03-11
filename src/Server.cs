using System.Net;
using System.Net.Sockets;
using System.Text;

//Start server
Console.WriteLine("Starting server...");
TcpListener server = new(IPAddress.Any, 4221);
server.Start();
Console.WriteLine("Server started on port 4221");

while (true)
{
    //Open socket and wait for call from client
    var socket = server.AcceptSocket();

    //Create response
    var response = "HTTP/1.1 200 OK\r\n\r\n";

    //Prepare response as byte array
    var dataBuffer = Encoding.UTF8.GetBytes(response);

    //Send response via saved socket
    socket.Send(dataBuffer);

}