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
            string contentType = res.Content is StreamContent ? "application/octet-stream" : "text/plain";
            response.Append($"HTTP/1.1 {(int)res.StatusCode} {res.StatusCode}");
            response.Append($"\r\nContent-Type: {contentType}");
            response.Append($"\r\nContent-Length: {content.Length}");
            response.Append("\r\n");
            response.Append($"\r\n{content}"); ;
        }
        else
        {
            response.Append($"HTTP/1.1 {(int)res.StatusCode} {res.StatusCode}\r\n\r\n");
        }
        byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
        await s.SendAsync(responseBytes, SocketFlags.None);
    }

    public static HttpResponseMessage HandleRequest(this Socket _, Request request, string? filesDirectory)
    {
        //Prepare response
        HttpResponseMessage response = new();

        //Check method
        response = request.Method.ToLower() switch
        {
            "get" => HandleGetRequest(request, response, filesDirectory),
            "post" => HandlePostRequest(request, response, filesDirectory),
            _ => new(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method not allowed")
            },
        };
        return response;
    }

    private static HttpResponseMessage HandlePostRequest(Request request, HttpResponseMessage response, string? filesDirectory)
    {
        //POST /files/<filename>
        if (request.Method.ToLower().Equals("post") && request.Path.Contains("/files/"))
        {
            //Get the file name from the path and the file from the request content
            string fileName = request.Path.Replace("/files/", "");
            byte[] fileContent = Encoding.UTF8.GetBytes(request.Content);

            //Save file
            string filePath = filesDirectory + "/" + fileName;
            File.WriteAllBytes(filePath, fileContent);

            response = new(HttpStatusCode.Created)
            {
                Content = new StringContent($"File created at {filePath}")
            };
        }
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

    private static HttpResponseMessage HandleGetRequest(Request request, HttpResponseMessage response, string? filesDirectory)
    {
        //GET "/"
        if (request.Path.Equals("/"))
        {
            Console.WriteLine("Root path called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent("You called the root path"),
                StatusCode = HttpStatusCode.OK
            };
            return response;
        }

        //GET "/index.html"
        if (request.Path.Equals("/index.html"))
        {
            Console.WriteLine("Index page called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent("This is the index page")
            };
            return response;
        }

        //GET "/echo/{message}"
        if (request.Path.Contains("/echo/"))
        {
            Console.WriteLine("Echo called. Response was: " + request.Path.Replace("/echo/", ""));
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent(request.Path.Replace("/echo/", ""), Encoding.UTF8, "text/plain"),
            };
            return response;
        }

        //GET /user-agent
        if (request.Path.Equals("/user-agent"))
        {
            Console.WriteLine("Get user agent called");
            response = new(HttpStatusCode.OK)
            {
                Content = new StringContent(request.Headers["User-Agent"]),
            };
            return response;
        }

        //GET /files/<filename>
        if (request.Path.Contains("/files/"))
        {
            Console.WriteLine("Get file called");

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
            return response;
        }

        //others
        Console.WriteLine("Path not found");
        response = new(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Path not found")
        };
        return response;
    }

    public static async Task RespondAsync(this Socket s, HttpResponseMessage response)
    {
        await s.SendAsync(response);
    }
}
