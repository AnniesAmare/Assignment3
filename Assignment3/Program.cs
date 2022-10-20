using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("Server is started...");

while (true)
{
    // get client connection
    var newClient = server.AcceptTcpClient();

    // once connected, create new thread to handle communication
    Thread thread = new Thread(HandleClient);
    thread.Start(newClient);

    // the handle method used on all connected clients
    void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        Console.WriteLine("Connected to new client");

        String data = null;
        var stream = client.GetStream();
        var buffer = new byte[1024];
        var response = new Response();

        try
        {
            var readCount = stream.Read(buffer, 0, buffer.Length);

            data = System.Text.Encoding.UTF8.GetString(buffer, 0, readCount);
            Console.WriteLine("Received request: {0}", data);
            var requestFromJson = JsonSerializer.Deserialize<Request>(data);
            
            //REQUEST HANDLING
            response = requestFromJson.checkForBadRequest();
            if (!response.Status.Contains('4')) //Making sure the request is valid
            {
                Console.WriteLine("Request approved");
                var method = requestFromJson.Method;
                if (method == "echo")
                {
                    response.Status = "1 Ok";
                    response.Body = requestFromJson.Body;
                }

            }

            //SEND JSON RESPONSE 
            var responseToJson = JsonSerializer.Serialize<Response>(response);
            Console.WriteLine("Created response: {0}", responseToJson);

            var responseBuffer = Encoding.UTF8.GetBytes(responseToJson);
            stream.Write(responseBuffer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("No request recieved"); //not needed
        }

        stream.Close();
    }


}