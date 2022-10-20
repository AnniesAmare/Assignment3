using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("Server is started...");

//DATA MODEL
List<Category> category = new List<Category>();
category.Add(new Category(1, "Beverages"));
category.Add(new Category(2, "Condiments"));
category.Add(new Category(3, "Confections"));

var DataModel = new DataModel(category);

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
            if (!response.Status.Contains('4')) //Making sure the request is not a bad request
            {
                Console.WriteLine("Request approved");
                var method = requestFromJson.Method;

                // HANDLING ECHO METHOD
                if (method == "echo")
                {
                    response.Status = "1 Ok";
                    response.Body = requestFromJson.Body;
                }

                // HANDLING READ METHOD
                if (method == "read")
                {
                    var requestedCid = requestFromJson.getPathCid();
                    response = DataModel.read(requestedCid);
                }

                //HANDLING UPDATE METHOD
                if (method == "update")
                {
                    var requestedCid = requestFromJson.getPathCid();
                    var updatedCategoryFromJson = JsonSerializer.Deserialize<Category>(requestFromJson.Body);
                    response = DataModel.update(requestedCid, updatedCategoryFromJson);
                }

                //HANDLING CREATE METHOD
                if (method == "create")
                {
                    var createdCategoryFromJson = JsonSerializer.Deserialize<Category>(requestFromJson.Body);
                    var createdCategoryName = createdCategoryFromJson.Name;
                    response = DataModel.create(createdCategoryName);
                }

                //HANDLING DELETE METHOD
                if (method == "delete")
                {
                    var requestedCid = requestFromJson.getPathCid();
                    response = DataModel.delete(requestedCid);
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