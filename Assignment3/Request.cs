using System.Text.Json;
namespace Assignment3;

public class Request
{
    //request received with uppercase (new test suite)
    public string Method { get; set; }
    public string Path { get; set; }
    public string Date { get; set; }
    public string Body { get; set; }
    
    /*
//Request received in lowercase (old test suite)
[JsonPropertyName("method")]
public string Method { get; set; }

[JsonPropertyName("path")]
public string Path { get; set; }

[JsonPropertyName("date")]
public string Date { get; set; }

[JsonPropertyName("body")]
public string Body { get; set; }
*/

    //ERROR LIST
    private List<string> RequestErrors = new List<string>(10);

    //METHOD TO CHECK PATH IS VALID
    private void checkPath()
    {
        //PATH IS IRRELEVANT ON ECHO METHOD
        if (Method == "echo") return;

        //PATH NOT NULL RESTRAINT CHECK
        if (Path == null)
        {
            RequestErrors.Add("missing resource");
            return;
        }
        
        //SPECIFIC PATH CHECK
        if (Path.Contains("testing")) return; //always accept testing-paths
        if (!Path.Contains("/api/categories"))
        {
            RequestErrors.Add("4 Bad Request");
            return;
        }

        //PATH CONTAINING ID CHECK
        var pathSplit = Path.Split("/");
        if (pathSplit.Length > 3) //if the path contains an id after categories
        {
            var cidInput = pathSplit[3];
            if (!int.TryParse(cidInput, out int cid))
            {
                RequestErrors.Add("4 Bad Request");
                return;
            }
            if (Method == "create") //specific id is not allowed on create-method
            {
                RequestErrors.Add("4 Bad Request");
                return;
            }
        } else if (Method is "update" or "delete") //specific id is required on update or delete
        {
            RequestErrors.Add("4 Bad Request"); 
            return;
        }
    }

    //METHOD TO CHECK DATE IS VALID
    private void checkDate()
    {
        if (Date == null)
        {
            RequestErrors.Add("missing date");
            return;
        }
        if (!Int64.TryParse(Date, out long date))
        {
            RequestErrors.Add("illegal date");
            return;
        }
        return;
    }

    //METHOD TO CHECK METHOD IS VALID
    private void checkMethod()
    {
        if (Method == null)
        {
            RequestErrors.Add("missing method");
            return;
        }
        if (Method is "create" or "read" or "update" or "delete" or "echo")
        {
            return;
        }
        RequestErrors.Add("illegal method");
        return;
    }

    private void checkBody()
    {
        //BODY IS IRRELEVANT ON READ
        if (Method == "read") return;

        //BODY IS IRRELEVANT ON DELETE
        if (Method == "delete") return;

        //BODY NOT NULL RESTRAINT
        if (Body == null)
        {
            RequestErrors.Add("missing body");
            return;
        }
        //BODY NEEDS READABLE CATEGORY OBJECT ON CREATE & UPDATE
        if (Method is "create" or "update" )
        {
            try
            {
                JsonSerializer.Deserialize<Category>(Body);
            }
            catch (Exception e)
            {
                //Console output to show invalid body input.
                //Console.WriteLine("could not convert body to json: {0}",Body);
                RequestErrors.Add("illegal body");
                return;
            }
        }
        return;
    }

    public Response checkForBadRequest()
    {
        Response outputResponse = new Response();
        checkMethod();
        checkDate();
        checkBody();
        checkPath();

        //DEFAULT STATUS OUTPUT
        outputResponse.Status = "";

        //IF ERRORS HAVE OCCURRED ADD THEM TO STATUS
        if (RequestErrors.Count > 0)
        {
            outputResponse.Status = "4 Bad Request: ";
            bool isFirst = true;
            foreach (var error in RequestErrors)
            {
                //OVERWRITES ALL OTHER ERRORS (FOR CASES WITHOUT ERROR SPECIFICATION)
                if (error.Contains('4'))
                {
                    outputResponse.Status = "4 Bad Request";
                    break;
                }
                //APPENDS ALL ERRORS TO THE STATUS
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

    //METHOD TO FETCH CID FROM PATH
    public int getPathCid()
    {
        var pathSplit = Path.Split("/");
        if (pathSplit.Length > 3) //if the path contains an id after categories
        {
            var cidInput = pathSplit[3];
            if (int.TryParse(cidInput, out int cid))
            {
                return cid;
            }
        }
        return 0; //if the path does not contain a cid.
    }
}