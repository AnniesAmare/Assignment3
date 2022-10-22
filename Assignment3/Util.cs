using System.Net.Sockets;
using System.Text;
using System.Text.Json;
namespace Assignment3;

public static class Util
{
    public static string ToJson(this object data)
    {
        return JsonSerializer.Serialize(data);
    }

    public static T? FromJson<T>(this string element) => JsonSerializer.Deserialize<T>(element);
    
    public static void SendResponse(this TcpClient client, string response)
    {
        var strm = client.GetStream();
        var msg = Encoding.UTF8.GetBytes(response);
        strm.Write(msg, 0, msg.Length);
        strm.Close();
    }

    public static Request? ReadRequest(this TcpClient client)
    {
        var strm = client.GetStream();
        //strm.ReadTimeout = 250;
        byte[] reqst = new byte[2048];
        using (var memStream = new MemoryStream())
        {
            int bytesread = 0;
            do
            {
                bytesread = strm.Read(reqst, 0, reqst.Length);
                memStream.Write(reqst, 0, bytesread);

            } while (bytesread == 2048);

            var requestData = Encoding.UTF8.GetString(memStream.ToArray());
            Console.WriteLine("Received request: {0}", requestData); //used for testing
            return JsonSerializer.Deserialize<Request>(requestData);
        }
    }



}