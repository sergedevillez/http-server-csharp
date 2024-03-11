using System.Net;
using codecrafters_http_server_csharp.Models;


//Start server
Server s = new(args, 4221, IPAddress.Any);

s.Start();