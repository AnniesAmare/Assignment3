﻿using System.Text.Json.Serialization;

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
    public Boolean checkPath()
    {
        if (this.Path == null && this.Method != "echo")
        {
            RequestErrors.Add("missing resource");
            return false;
        }
        return true;
    }

    //METHOD TO CHECK DATE IS VALID
    public Boolean checkDate()
    {
        if (this.Date == null)
        {
            RequestErrors.Add("missing date");
            return false;
        }
        else if (!Int64.TryParse(this.Date, out long date))
        {
            RequestErrors.Add("illegal date");
            return true;
        }
        else
        {
            return false;
        }
    }

    //METHOD TO CHECK METHOD IS VALID
    public Boolean checkMethod()
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

    public Response checkForBadRequest()
    {
        Response outputResponse = new Response();
        this.checkMethod();
        this.checkPath();
        this.checkDate();

        outputResponse.Status = "";

        if (RequestErrors.Count > 0)
        {
            outputResponse.Status = "4 Bad Request: ";
            bool isFirst = true;
            foreach (var error in RequestErrors)
            {
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


    //public Response writeBadRequest()
    //{
    //    Response outputResponse = new Response();
    //    outputResponse.Status = "4 Bad Request:";
    //    if (!this.checkMethod()) outputResponse.Status += " illegal method";
    //    if (!this.checkPath()) outputResponse.Status += " missing resource";
    //    if (!this.checkDate()) outputResponse.Status += " missing date";

    //    return outputResponse;
    //}


}