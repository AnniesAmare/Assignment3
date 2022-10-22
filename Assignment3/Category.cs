using System.Text.Json.Serialization;
namespace Assignment3;

public class Category
{
    [JsonPropertyName("cid")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public Category (int id, string name)
    {
        Id = id;
        Name = name;
    }
}