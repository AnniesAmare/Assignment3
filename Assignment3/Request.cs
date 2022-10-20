using System.Text.Json.Serialization;

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


    //METHOD TO CHECK PATH IS VALID
    public Boolean checkPath()
    {
        if (this.Path == null && this.Method != "echo")
        {
            return false;
        }
        return true;
    }

    //METHOD TO CHECK METHOD IS VALID
    public Boolean checkMethod()
    {
        var method = this.Method;
        if (method is "create" or "read" or "update" or "delete" or "echo")
        {
            return true;
        }
        else
        {
            return false;
        }

    }


}