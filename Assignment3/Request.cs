using System.Text.Json;
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

    private List<string> RequestErrors = new List<string>(10);

    //METHOD TO CHECK PATH IS VALID
    private Boolean checkPath()
    {
        //PATH IS IRRELEVANT ON ECHO
        if (this.Method == "echo") return true;

        //PATH NOT NULL RESTRAINT CHECK
        if (this.Path == null)
        {
            RequestErrors.Add("missing resource");
            return false;
        }
        
        //SPECIFIC PATH CHECK
        if (this.Path.Contains("testing")) return true; //always accept testing-paths
        if (!this.Path.Contains("/api/categories"))
        {
            RequestErrors.Add("4 Bad Request");
            return false;
        }

        //PATH ID CHECK
        var method = this.Method;
        var pathSplit = this.Path.Split("/");
        if (pathSplit.Length > 3) //if the path contains an id after categories
        {
            var id_string = pathSplit[3];
            if (!int.TryParse(id_string, out int id_number))
            {
                RequestErrors.Add("4 Bad Request");
                return false;
            }
            else if (method == "create") //specific id is not allowed on create-method
            {
                RequestErrors.Add("4 Bad Request");
                return false;
            }
        } else if (method is "update" or "delete") //specific id is required on update or delete
        {
            RequestErrors.Add("4 Bad Request"); 
            return false;
        }

        return true;
    }

    //METHOD TO CHECK DATE IS VALID
    private Boolean checkDate()
    {
        if (this.Date == null)
        {
            RequestErrors.Add("missing date");
            return false;
        }
        else if (!Int64.TryParse(this.Date, out long date))
        {
            RequestErrors.Add("illegal date");
            return false;
        }
        return true;
    }

    //METHOD TO CHECK METHOD IS VALID
    private Boolean checkMethod()
    {
        var method = this.Method;
        if (method == null)
        {
            RequestErrors.Add("missing method");
            return false;
        }
        else if (method is "create" or "read" or "update" or "delete" or "echo")
        {
            return true;
        } 
        else
        {
            RequestErrors.Add("illegal method");
            return false;
        }

    }

    private Boolean checkBody()
    {
        var body = this.Body;
        var method = this.Method;
        
        //BODY IS IRRELEVANT ON READ
        if (method == "read") return true;

        //BODY IS IRRELEVANT ON DELETE
        if (method == "delete") return true;


        //BODY NOT NULL RESTRAINT
        if (body == null)
        {
            RequestErrors.Add("missing body");
            return false;
        } 
        else if (method is "create" or "update" )
        {
            try
            {
                var bodyFromJson = JsonSerializer.Deserialize<Category>(body);
            }
            catch (Exception e)
            {
                Console.WriteLine("could not convert body to json: {0}",body);
                RequestErrors.Add("illegal body");
                return false;
            }
        }
        return true;

    }

    public Response checkForBadRequest()
    {
        Response outputResponse = new Response();
        this.checkMethod();
        this.checkDate();
        this.checkBody();
        this.checkPath();

        outputResponse.Status = "";

        if (RequestErrors.Count > 0)
        {
            outputResponse.Status = "4 Bad Request: ";
            bool isFirst = true;
            foreach (var error in RequestErrors)
            {
                if (error.Contains('4'))
                {
                    outputResponse.Status = "4 Bad Request"; //does this when there is no specific error-identification
                    break;
                }

                if (isFirst)
                {
                    outputResponse.Status += error;
                    isFirst = false;
                }
                else
                {
                    outputResponse.Status += ", ";
                    outputResponse.Status += error;
                }
            }
        }

        return outputResponse;
    }

    public int getPathCid()
    {
        var method = this.Method;
        var pathSplit = this.Path.Split("/");
        if (pathSplit.Length > 3) //if the path contains an id after categories
        {
            var id_string = pathSplit[3];
            if (int.TryParse(id_string, out int id_number))
            {
                return id_number;
            }
        }
        return 0;
    }

}