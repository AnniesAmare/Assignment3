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

        //DATA MODEL
        List<Category> category = new List<Category>();
        category.Add(new Category(1, "Beverages"));
        category.Add(new Category(2, "Condiments"));
        category.Add(new Category(3, "Confections"));

        var stream = client.GetStream();
        var buffer = new byte[1024];
        var response = new Response();

        try
        {
            var readCount = stream.Read(buffer, 0, buffer.Length);

            var requestData = System.Text.Encoding.UTF8.GetString(buffer, 0, readCount);
            Console.WriteLine("Received request: {0}", requestData);
            var requestFromJson = JsonSerializer.Deserialize<Request>(requestData);
            
            //REQUEST HANDLING
            response = requestFromJson.checkForBadRequest();
            if (!response.Status.Contains('4')) //Making sure the request is valid
            {
                Console.WriteLine("Request approved");
                var method = requestFromJson.Method;

                // ECHO METHOD
                if (method == "echo")
                {
                    response.Status = "1 Ok";
                    response.Body = requestFromJson.Body;
                }

                // READ METHOD
                if (method == "read")
                {
                    var requestedCid = requestFromJson.getPathCid();
                    if (requestedCid == 0) //0 means that no specific Cid is requested
                    {
                        response.Status = "1 Ok";
                        var categoriesToJson = JsonSerializer.Serialize(category);
                        response.Body = categoriesToJson;
                    }
                    else
                    {
                        var requestedCategory = category.Find(category1 => category1.Id == requestedCid);
                        if (requestedCategory != null)
                        {
                            response.Status = "1 Ok";
                            var categoryToJson = JsonSerializer.Serialize<Category>(requestedCategory);
                            response.Body = categoryToJson;
                        }
                        else
                        {
                            response.Status = "5 Not Found";
                        }

                    }

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