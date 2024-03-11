using System.Net;
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

    public static HttpResponseMessage HandleRequest(this Socket socket, Request request, string? filesDirectory)
    {
        //Prepare response
        HttpResponseMessage response;
        //PATH "/"
        if (request.Path.Equals("/"))
        {
            Console.WriteLine("Root path called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent("You called the root path"),
                StatusCode = HttpStatusCode.OK
            };
        }
        //PATH "/index.html"
        else if (request.Path.Equals("/index.html"))
        {
            Console.WriteLine("Index page called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent("This is the index page")
            };
        }
        //PATH "/echo/{message}"
        else if (request.Path.Contains("/echo/"))
        {
            Console.WriteLine("Echo called. Response was: " + request.Path.Replace("/echo/", ""));
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent(request.Path.Replace("/echo/", ""), Encoding.UTF8, "text/plain"),
            };
        }
        //PATH /user-agent
        else if (request.Path.Equals("/user-agent"))
        {
            Console.WriteLine("User agent called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent(request.Headers["User-Agent"]),
            };
        }
        //PATH /files/<filename>
        else if (request.Path.Contains("/files/"))
        {
            if (!File.Exists(filesDirectory + "/" + request.Path.Replace("/files/", "")))
            {
                response = new(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("File not found")
                };
            }
            else
            {
                //Get the files content into content and the content-type as application/octet-stream
                response = new(HttpStatusCode.OK)
                {
                    Content = new StreamContent(File.OpenRead(filesDirectory + "/" + request.Path.Replace("/files/", "")))
                };
            }
        }
        //PATH others
        else
        {
            Console.WriteLine("Path not found");
            response = new(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Path not found")
            };
        }

        return response;
    }


    public static async Task RespondAsync(this Socket s, HttpResponseMessage response)
    {
        await s.SendAsync(response);
    }
}
