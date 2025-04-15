
namespace Ecommerce1.Controllers;
using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net;

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
    
    public async Task<ActionResult<bool>> AddProduct([FromForm] DTOAddProductRequest obj)
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, "An unexpected server error occurred.");
            }

            else
            {
                clsUser? User =await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest("User Does not exsiste");

                else
                {
                    if (!((((User.Atherization&(byte)DTOUser.enAtherizations.AddProduct)==(byte)DTOUser.enAtherizations.AddProduct)&&User.Role==DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest("User is Athorized");

                    }
                }
                
            }

        }
        else
        {
            return BadRequest("You need to log in ");
        }

        if (obj.stProduct != null) obj.Product = JsonConvert.DeserializeObject<DTOProduct>(obj.stProduct);

        if (obj.stcatigories != null) obj.CatigoriesList = JsonConvert.DeserializeObject<List<DTOCatygory.enCatigories>?>(obj.stcatigories);


        bool result = false;
        //handle image and save it in the image file

        if (obj.Image == null || obj.Image.Length == 0)
        {
            return BadRequest("the Image is not valaid ");
        }

        if (obj.Product.Price <= 0)
        {
            return BadRequest("the ProductPrice must be more the (0) ");


        }

        if (obj.stcatigories.Length == 0)
        {
            return BadRequest("You must send aat lest one Catigory ");
        }


        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest("the Image is not valaid ");


        }


        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest("the Image is not valaid ");


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
    public async Task<ActionResult<bool>> UpdateProduct([FromForm] DTOAddProductRequest obj)
    {

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, "An unexpected server error occurred.");
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest("User Does not exsiste");

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.UpdateProduct) == (byte)DTOUser.enAtherizations.UpdateProduct) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest("User is Athorized");

                    }
                }

            }

        }
        else
        {
            return BadRequest("You need to log in ");
        }
        bool result;



        if (obj.stProduct != null) obj.Product = JsonConvert.DeserializeObject<DTOProduct>(obj.stProduct);

        if (obj.stcatigories != null) obj.CatigoriesList = JsonConvert.DeserializeObject<List<DTOCatygory.enCatigories>?>(obj.stcatigories);


        //handle image and save it in the image file

        

        if (obj.Product == null)
        {
            return BadRequest("You need to provied product information : price, name... ");
        }
        if (obj.Product.Price <= 0)
        {
            return BadRequest("the ProductPrice must be more the (0) ");


        }

        if (obj.CatigoriesList == null||obj.CatigoriesList.Count==0) return BadRequest("you need to provied Catigories");

 

        clsProduct? product = await clsProduct.Find(obj.Product.ID);

        if (product == null)
        {
            return BadRequest("thier is no product found ");
        }

        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest("the Image is not valaid ");


        }


        if (string.IsNullOrEmpty(obj.Product.Name))
        {
            return BadRequest("the Image is not valaid ");


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

    public async Task<ActionResult<bool>> DeleteProduct([FromBody] int ProductID)

    {

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, "An unexpected server error occurred.");
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return BadRequest("User Does not exsiste");

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.DeleteProduct) == (byte)DTOUser.enAtherizations.DeleteProduct) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest("User is Athorized");

                    }
                }

            }

        }
        else
        {
            return BadRequest("You need to log in ");
        }

        clsProduct? product = await clsProduct.Find(ProductID);
        if (product==null)
        {
            return BadRequest("the user did not be found ");
        }
        string path = clsGlobale.GetLoadDiractory()+product.ImageName;
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    

       bool result= await product.ClearCatigories();
        if (!result) {

            return StatusCode(500, "An unexpected server error occurred.");

        }
        product = null;
      result=  await clsProduct.Delete(ProductID);

        if (!result)
        {

            return StatusCode(500, "An unexpected server error occurred.");


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
        else return BadRequest("thier is no user with this ID");

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
                return StatusCode(500, "An unexpected server error occurred.");
            }

            else
            {
                clsUser? User = await clsUser.Find(UserID.Value);

                if (User == null) return StatusCode(500, "User Does not exsiste");

                else
                {
                    if (!((((User.Atherization & (byte)DTOUser.enAtherizations.ShowProductList) == (byte)DTOUser.enAtherizations.ShowProductList) && User.Role == DTOUser.enRole.User) || (User.Role == DTOUser.enRole.Admine)))
                    {

                        return BadRequest("User is an Athorized");

                    }
                }

            }

        }
        else
        {
            return BadRequest("You need to log in ");
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return StatusCode(500, "An unexpected server error occurred.");
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

