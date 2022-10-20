using System.Text.Json;
using System.Xml.Linq;

public class DataModel
{
    public List<Category> Categories { get; set; }

    public DataModel (List<Category> defaultList)
    {
        Categories = defaultList;
    }

    //READ METHOD
    public Response read(int requestedCid)
    {
        var response = new Response();

        //CID NOT SPECIFIED
        if (requestedCid == 0) //0 means that no specific Cid is requested
        {
            response.Status = "1 Ok";
            var categoriesToJson = JsonSerializer.Serialize(Categories);
            response.Body = categoriesToJson;
        }
        //CID SPECIFIED
        else
        {
            var requestedCategory = Categories.Find(category1 => category1.Id == requestedCid);
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
        
        return response;
    }

    //READ METHOD
    public Response delete(int requestedCid)
    {
        var response = new Response();
        var deletedCategory = Categories.Find(category1 => category1.Id == requestedCid);
        if (deletedCategory != null)
        {
            //deleting the Category matching the requested Cid in Categories
            var deletedCategoryIndex = Categories.FindIndex(category1 => category1.Id == requestedCid);
            Categories.RemoveAt(deletedCategoryIndex);

            //returning status 
            response.Status = "1 Ok";
            //option for returning deleted Category as body. Used for testing.
            //var categoryToJson = JsonSerializer.Serialize<Category>(deletedCategory);
            //response.Body = categoryToJson;
        }
        else
        {
            response.Status = "5 Not Found";
        }

        return response;
    }



    //UPDATE METHOD
    public Response update(int requestedCid, Category updatedCategory)
    {
        var response = new Response();

        var requestedCategory = Categories.Find(category1 => category1.Id == requestedCid);
        if (requestedCategory != null)
        {
            //updating the Category matching the requested Cid in Categories
            var requestedCategoryIndex = Categories.FindIndex(category1 => category1.Id == requestedCid);
            Categories[requestedCategoryIndex] = updatedCategory;

            //returning the updated Category
            response.Status = "3 Updated";
            var categoryToJson = JsonSerializer.Serialize<Category>(Categories[requestedCategoryIndex]);
            response.Body = categoryToJson;
        }
        else
        {
            response.Status = "5 Not Found";
        }


        return response;

    }

    private Boolean checkCid(int Cid)
    {
        foreach (var category in Categories)
        {
            if (category.Id == Cid) return false;
        }
        return true;
    }

    private int getUniqueCid()
    {
        for (int i = 1; i < Categories.Capacity + 1; i++)
        {
            if (checkCid(i))
            {
                return i;
            }
        }
        return 0;
    }

    //CREATE METHOD
    public Response create(string newCategoryName)
    {
        var response = new Response();

        //Create a unique Cid
        var newCid = getUniqueCid();

        //Checking that a unique Cid is created, otherwise returns an error
        if (newCid == 0)
        {
            response.Status = "6 Error";
        }
        else
        {
            //adding the new Category to the DataModel
            var newCategory = new Category(newCid, newCategoryName);
            Categories.Add(newCategory);

            //returning the new Category from our Categories list
            response.Status = "2 Created";
            var newCategoryIndex = Categories.FindIndex(category1 => category1.Id == newCid);
            var categoryToJson = JsonSerializer.Serialize<Category>(Categories[newCategoryIndex]);
            response.Body = categoryToJson;
        }
        
        return response;

    }

}