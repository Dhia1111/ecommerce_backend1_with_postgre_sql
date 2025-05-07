
namespace Ecommerce1.Controllers;
using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using static DTOCatygory;

public class DTOAddProductRequest
{
    public string stProduct { get; set; }
    public IFormFile? Image { get; set; }
    public string stcatigories { get; set; }
    // Properties to deserialize the JSON strings
    public DTOProduct? Product { get;set; }
    public  List<DTOCatygory.enCatigories>? CatigoriesList { get; set; }

    public DTOAddProductRequest(string stProduct, IFormFile image,string stcatigories)
    {
        this.stProduct = stProduct;
        Image = image;
        this.stcatigories = stcatigories;


    }
    public DTOAddProductRequest( )
    {
        this.stProduct = "";
        
        this.stcatigories ="";



    }

}
 

[Route("api/Ecommerce/ProductMangment")]
[ApiController]
public class clsProductMangentAPIs : ControllerBase
{

    [HttpPost("AddProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<bool>> AddProduct([FromForm] DTOAddProductRequest obj)
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,""));
            }

            else
            {
                clsUser? User =await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest( new DTOGeneralResponse("User Does not exsiste",400,"Serch faileur"));

                else
                {
                    if (!((((User.Atherization&(byte)DTOUser.enAtherizations.AddProduct)==(byte)DTOUser.enAtherizations.AddProduct)&&User.Role==DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest(new DTOGeneralResponse("User is UnAthorized",400,"Atherization"));

                    }
                }
                
            }

        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ",400, "Authentication"));
        }

        if (obj.stProduct != null) obj.Product = JsonConvert.DeserializeObject<DTOProduct>(obj.stProduct);

        if (obj.stcatigories != null) obj.CatigoriesList = JsonConvert.DeserializeObject<List<DTOCatygory.enCatigories>?>(obj.stcatigories);

        if(obj.Product == null)
        {
            return BadRequest("Provied a valiad data the  Product   is empty");

        }
        if (obj.CatigoriesList == null )
        {
            return BadRequest("Provied a valiad data the  catigories list is empty");
        }
        if (obj.CatigoriesList.Count == 0)
        {
            return BadRequest(new DTOGeneralResponse("Provied a valiad data the  catigories list is empty",400,"Saving faileur"));
        }
        bool result = false;
        //handle image and save it in the image file
        if (obj.Image == null || obj.Image.Length == 0)
        {
            return BadRequest(new DTOGeneralResponse("the Image is not valaid ",400,"null image "));
        }

        var extension = Path.GetExtension(obj.Image.FileName).ToLowerInvariant();

        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        if (!permittedExtensions.Contains(extension))
            return BadRequest(new DTOGeneralResponse("Invalid file extension,pleas provied an image file",400, "Bad file extention use( \".jpg\", \".jpeg\", \".png\", \".gif\", \".bmp\", \".webp\")"));

        if (obj.Product.Price <= 0)
        {
            return BadRequest(new DTOGeneralResponse("the ProductPrice must be more the (0) ",400, "Saving failure"));


        }

        if (!clsValidation.IsValidDecimal(obj.Product.Price) )
        {
            return BadRequest(new DTOGeneralResponse("the ProductPrice decimal part should be less then tow Numbers (2.22) not 2(.221) ", 400, "Saving failure"));


        }


        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest(new DTOGeneralResponse("the Product name is  anvalaid ",400,"Saving failure"));


        }


        string extention = clsGlobale.GetTheBestImageExtention();


        obj.Product.ImageName = Guid.NewGuid().ToString() + extention;
        var path = clsGlobale.GetLoadDiractory();

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path += obj.Product.ImageName ;
        using (var stramer = new FileStream(path, FileMode.Create))
        {
            await obj.Image.CopyToAsync(stramer);
        }


        clsProduct product = new clsProduct(obj.Product);
        result = await product.Save();

        //handle Catigories
        if (result)
        {


            foreach (DTOCatygory.enCatigories c in obj.CatigoriesList)
            {




               


                    await product.AddNewCatigory(c);




            

            }
        }

        return Ok(true);
    }


    [HttpPost("UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<bool>> UpdateProduct([FromForm] DTOAddProductRequest obj)
    {

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,"Serch failure"));
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest(new DTOGeneralResponse("User Does not exsiste",400,"Serch failure"));

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.UpdateProduct) == (byte)DTOUser.enAtherizations.UpdateProduct) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest(new DTOGeneralResponse("User is UnAthorized",400,"Athorization"));

                    }
                }

            }

        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ",400,"Athentication"));
        }
        bool result;



        if (obj.stProduct != null) obj.Product = JsonConvert.DeserializeObject<DTOProduct>(obj.stProduct);

        if (obj.stcatigories != null) obj.CatigoriesList = JsonConvert.DeserializeObject<List<DTOCatygory.enCatigories>?>(obj.stcatigories);

        if (obj.CatigoriesList == null || obj.CatigoriesList.Count == 0) return BadRequest(new DTOGeneralResponse("you need to provied Catigories",400,"Saving failure"));
    
        if (obj.Product == null)
        {
            return BadRequest(new DTOGeneralResponse("You need to provied product information : price, name... ",400,"Saving failure"));
        }
        //handle image and save it in the image file



        if (obj.Image != null && obj.Image.Length == 0)
        {
            var extension = Path.GetExtension(obj.Image.FileName).ToLowerInvariant();

            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            if (!permittedExtensions.Contains(extension))
                return BadRequest("Invalid file extension,pleas provied an image file");
        }

     
        if (obj.Product.Price <= 0)
        {
            return BadRequest(new  DTOGeneralResponse("the ProductPrice must be more the (0) ",400,"Saving failure"));


        }

        if (!clsValidation.IsValidDecimal(obj.Product.Price))
        {
            return BadRequest(new DTOGeneralResponse("the ProductPrice decimal part should be less then tow Numbers (2.22) not 2(.221) ", 400, "Saving failure"));


        }





        clsProduct? product = await clsProduct.Find(obj.Product.ID);

        if (product == null)
        {
            return BadRequest(new DTOGeneralResponse("thier is no product found ",400,"Saving failure"));
        }

        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest(new DTOGeneralResponse("the Product Name is not valaid ",400,"Saving failure"));


        }


        if (obj.Image != null && obj.Image.Length != 0)
        {
            string extention = clsGlobale.GetTheBestImageExtention();


            obj.Product.ImageName = Guid.NewGuid().ToString() + extention;
        }
        else
        {
            obj.Product.ImageName = product.ImageName;
        }
        var path = clsGlobale.GetLoadDiractory();



        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //delete Old Image from folder

        if (obj.Image != null && obj.Image.Length != 0)
        {
            if  (System.IO.File.Exists(path + product.ImageName))
            {

                System.IO.File.Delete(path + product.ImageName);
            }
            path += obj.Product.ImageName;
         
            
            using (var stramer = new FileStream(path, FileMode.Create))
            {
                await obj.Image.CopyToAsync(stramer);
            }
        }
       


       
        product.Name = obj.Product.Name;
        product.ImageName = obj.Product.ImageName;
        product.Price = obj.Product.Price;

        result = await product.Save();

        //handle Catigories
        if (result&&obj.CatigoriesList!=null)
        {

         await   product.ClearCatigories();

            foreach (DTOCatygory.enCatigories c in obj.CatigoriesList)
            {







                await product.AddNewCatigory(c);






            }
        }

        return Ok(result);
    }


    [HttpPost("DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<bool>> DeleteProduct([FromBody] int ProductID)
    {

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return BadRequest( new DTOGeneralResponse("LogIn again ",400,"Parsing failure"));
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest(new DTOGeneralResponse("User Does not exsiste",400,"Serch failure"));

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.DeleteProduct) == (byte)DTOUser.enAtherizations.DeleteProduct) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest(new DTOGeneralResponse("User is UnAthorized",400,"Atherization "));

                    }
                }

            }

        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ",400, "Authentication"));
        }

        clsProduct? product = await clsProduct.Find(ProductID);
        if (product==null)
        {
            return BadRequest(new DTOGeneralResponse("the user did not be found ",400,"Saving failure"));
        }
        string path = clsGlobale.GetLoadDiractory()+product.ImageName;
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    

       bool result= await product.ClearCatigories();

        product = null;
      result=  await clsProduct.Delete(ProductID);

        if (!result)
        {

            return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,"Deleting data failuer"));


        }
        return Ok(true);
    }


    [HttpGet("GetProduct/{ID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<DTOProduct>?>> GetProduct(int ID)
    {
        clsProduct? p = await clsProduct.Find(ID);
        if (p != null)
        {
            p.ImageUrl = clsGlobale.SetImageURL(p.ImageName.ToString());
            await p.LoadProductCatigories();
            return Ok(p.DTOProduct);
        }
        else return BadRequest(new DTOGeneralResponse("thier is no user with this ID",400,"Serch failure"));

    }



    [HttpGet("GetAllProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<DTOProduct>?>> GetAllProducts()
    
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,"Parsing failure"));
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return StatusCode(500, "User Does not exsiste");

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.ShowProductList) == (byte)DTOUser.enAtherizations.ShowProductList) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest(new DTOGeneralResponse("User is an Athorized",400,"Atherization"));

                    }
                }

            }

        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ",400,"Authentication"));
        }

        List<DTOProduct>? list = await clsProduct.GetAll();

        if (list != null)
        {
            foreach (DTOProduct p in list)
            {

                p.ImageUrl = clsGlobale.SetImageURL(p.ImageName);

            }
        }

        
        return Ok(list);

    }



    [HttpGet("GetAllProductsForCatigory/{CatigoryID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DTOProduct>?>> GetAllProductsForCatigory(DTOCatygory.enCatigories CatigoryID)

    {

        List<DTOProduct>? list = await clsProduct.GetAllProductForCatigory(CatigoryID);

        if (list != null)
        {
            foreach (DTOProduct p in list)
            {

                p.ImageUrl = clsGlobale.SetImageURL(p.ImageName);

            }
        }


        return Ok(list);

    }


    [HttpGet("IsUserAtherized", Name = "IsUserAtherized")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsUserAtherized()
    {
        //in this stage  i only let the type of operation is binary all or none 
        //in the  futer i can update it to more roles and Atherization types 

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,"Parsing failure"));
            }

            else
            {
                clsUser? user = await clsUser.Find(UserID.Value);
                if (user != null)
                {
                    if (user.Role==DTOUser.enRole.Admine)
                    {
                        return Ok(true);

                    }
                }
                else
                {
                    

                    return Ok(false);
                }
            }
        }

        return Ok(false);


    }




}

