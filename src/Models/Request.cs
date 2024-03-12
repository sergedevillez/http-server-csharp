using System.Text;

namespace codecrafters_http_server_csharp;

public class Request
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";
    public string Version { get; set; } = "";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Content { get; set; } = "";

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


        //Get the empty line index
        int emptyLineIndex = Array.IndexOf(lines, "");
        //Anything above the empty line is the header
        for (int i = 1; i < emptyLineIndex; i++)
        {
            string[] header = lines[i].Split(": ");
            if (header.Length == 2)
            {
                request.Headers.Add(header[0], header[1]);
            }
        }
        //Anything below the empty line is the content
        if (emptyLineIndex < lines.Length - 1)
        {
            request.Content = lines[emptyLineIndex + 1];
        }
        return request;
    }
}
