using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("Server is started...");

while (true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client accepted...");
    var stream = client.GetStream();
    var buffer = new byte[1024];
    String data = null;
    var response = new Response();

    try
    {
        var readCount = stream.Read(buffer, 0, buffer.Length);

        data = System.Text.Encoding.UTF8.GetString(buffer, 0, readCount);
        Console.WriteLine("Received: {0}", data);


        var requestFromJson = JsonSerializer.Deserialize<Request>(data);
        Console.WriteLine(requestFromJson.Method);

        //METHOD HANDLING
        if (requestFromJson.Method != null)
        {
            var method = requestFromJson.Method;
            if (method == "create")
            {
                
            } else if (method == "read")
            {
                
            } else if (method == "update")
            {
                
            } else if (method == "delete")
            {
                
            } else if (method == "echo")
            {
                
            }
            else
            {
                response.Status = "4 Bad Request ";
                response.Status += "illegal method";
            }
        }
        else
        {
            response.Status = "4 Bad Request ";
            response.Status += "missing method";
        }

        //CREATE AND SEND JSON RESPONSE 
        response.Body = " ";

        var responseToJson = JsonSerializer.Serialize<Response>(response);
        Console.WriteLine("Created response: {0}", responseToJson);

        var responseBuffer = Encoding.UTF8.GetBytes(responseToJson);
        stream.Write(responseBuffer);
    }
    catch (Exception e)
    {
        Console.WriteLine(e); //not needed
    }

    stream.Close();


    /*
    var readCount = stream.Read(buffer, 0, buffer.Length);
    var request = Encoding.UTF8.GetString(buffer, 0, readCount);

    var lines = request.Split("\n");

    Console.WriteLine(lines[0]);

    var response = "HTTP/1.1 200 Ok\nContent-Type: text/plain\n\n";

    if (lines[0].Contains("categories"))
    {
        response += "category(id=1, name=testing)";
    }
    else
    {
        response += "Hello from server: -)";
    }

    var responseBuffer = Encoding.UTF8.GetBytes(response);

    stream.Write(responseBuffer);

    stream.Close();
    */

}


public class Response
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }
}

public class Request
{
    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }
}
