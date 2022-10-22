using System.Text.Json;
namespace Assignment3;

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
            var categoriesToJson = Categories.ToJson();
            response.Body = categoriesToJson;
        }
        //CID SPECIFIED
        else
        {
            var requestedCategory = Categories.Find(x => x.Id == requestedCid);
            if (requestedCategory != null)
            {
                response.Status = "1 Ok";
                var categoryToJson = requestedCategory.ToJson();
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
        var deletedCategory = Categories.Find(x => x.Id == requestedCid);
        if (deletedCategory != null)
        {
            //deleting the Category matching the input cid
            var deletedCategoryIndex = Categories.FindIndex(x => x.Id == requestedCid);
            Categories.RemoveAt(deletedCategoryIndex);

            response.Status = "1 Ok";
            //option for returning deleted Category as body. Used for testing.
            //var categoryToJson = deletedCategory.ToJson();
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

        var requestedCategory = Categories.Find(x => x.Id == requestedCid);
        if (requestedCategory != null)
        {
            //updating the Category matching the input cid
            var requestedCategoryIndex = Categories.FindIndex(x => x.Id == requestedCid);
            Categories[requestedCategoryIndex] = updatedCategory;

            //returning the updated Category
            response.Status = "3 Updated";
            var categoryToJson = Categories[requestedCategoryIndex].ToJson();
            response.Body = categoryToJson;
        }
        else
        {
            response.Status = "5 Not Found";
        }
        return response;
    }

    private bool checkCid(int Cid)
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
            var newCategoryIndex = Categories.FindIndex(x => x.Name == newCategoryName);
            var categoryToJson = Categories[newCategoryIndex].ToJson();
            response.Body = categoryToJson;
        }
        return response;
    }

}