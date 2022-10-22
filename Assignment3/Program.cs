using System.Net;
using System.Net.Sockets;
using Assignment3;

var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
server.Start();

Console.WriteLine("Server is started...");

//DATA MODEL
List<Category> defaultCategories = new List<Category>
{
    { new(1, "Beverages") },
    {new (2, "Condiments")},
    {new (3, "Confections")},

};
var DataModel = new DataModel(defaultCategories);

while (true)
{
    // get new client connection
    var newClient = server.AcceptTcpClient();

    // once connected, create new thread to handle communication
    Thread thread = new Thread(HandleClient);
    thread.Start(newClient);

    // the handle method used on all connected clients
    void HandleClient(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        Console.WriteLine("Connected to new client");

        var response = new Response();

        try
        {
            var request = client.ReadRequest();

            //REQUEST HANDLING
            response = request.checkForBadRequest();
            if (!response.Status.Contains('4')) //Making sure the request is not a bad request
            {
                Console.WriteLine("Request approved");
                var method = request.Method;

                // HANDLING ECHO METHOD
                if (method == "echo")
                {
                    response.Status = "1 Ok";
                    response.Body = request.Body;
                }

                // HANDLING READ METHOD
                if (method == "read")
                {
                    var requestedCid = request.getPathCid();
                    response = DataModel.read(requestedCid);
                }

                //HANDLING UPDATE METHOD
                if (method == "update")
                {
                    var requestedCid = request.getPathCid();
                    var updatedCategoryFromJson = request.Body.FromJson<Category>();
                    response = DataModel.update(requestedCid, updatedCategoryFromJson);
                }

                //HANDLING CREATE METHOD
                if (method == "create")
                {
                    var createdCategoryFromJson = request.Body.FromJson<Category>();
                    var createdCategoryName = createdCategoryFromJson.Name;
                    response = DataModel.create(createdCategoryName);
                }

                //HANDLING DELETE METHOD
                if (method == "delete")
                {
                    var requestedCid = request.getPathCid();
                    response = DataModel.delete(requestedCid);
                }

            }
            //SEND JSON RESPONSE 
            var responseToJson = response.ToJson();
            Console.WriteLine("Created response: {0}", responseToJson);
            client.SendResponse(responseToJson);
        }
        catch (Exception e)
        {
            //Console.WriteLine(e);
            Console.WriteLine("Connection closed. No request received");
        }
    }


}