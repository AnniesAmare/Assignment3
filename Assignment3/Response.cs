using System.Text.Json.Serialization;

public class Response
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }
}