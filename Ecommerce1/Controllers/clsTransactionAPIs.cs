

using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce1.Controllers;

using BusinessLayer;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
 
[Route("api/Ecommerce/clsTransactionAPIs")]
[ApiController]
public class clsTransactionAPIs : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    public clsTransactionAPIs(IHttpClientFactory factory)
    {

        _httpClientFactory = factory;

    }



    //Get all Transactions


    [HttpGet("GetAllTransactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<List<DTOTransaction>?>> GetAll()
    {
        clsUser? User;


        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {

            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }



            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }

        if ((User.Role != DTOUser.enRole.Admine &&!(User.IsAthorizedUser(DTOUser.enAtherizations.ShowTransactionList))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }



        return await clsTransaction.GetAll();

    }


    //Get Transaction ByID return a {transactionID ,totolPrice ,[Icluded Products]

    [HttpGet("GetInCludedProductsInTransaction/{TransactionID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<object>?>> GetTrasactionDetails(int TransactionID)
    {
        clsUser? User;


        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {

            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }



            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }

        if ((User.Role != DTOUser.enRole.Admine && !(User.IsAthorizedUser(DTOUser.enAtherizations.ShowTransactionDetails))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }



        List<object>? list = new List<object>();
        List<DTOIncludedProducts>? IncludedProductslist = new List<DTOIncludedProducts>();
        if (!int.TryParse(TransactionID.ToString(),out int ID))
        {
            return BadRequest(new DTOGeneralResponse("Send a valid Params", 400, "Validation and Parsing data"));

        }


          IncludedProductslist= await clsIncludedProduct.GetAllIncludedProducts(ID);


        if (IncludedProductslist != null)
        {
            foreach (DTOIncludedProducts e in IncludedProductslist)
            {
                clsProduct? p = await clsProduct.Find(e.ProductID);

                if (p == null)
                {
                    return null;
                }
                else
                {
                  list.Add(new {id=e.ID,name=p.Name,imageUrl=clsGlobale.SetImageURL(p.ImageName),price=p.Price, numberOfProduct=e.NumberOfProduct });
                }
            }

        }

        else
        {
            return null;
        }
       
        return list;
    }




    [HttpDelete("DeleteTransaction/{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOGeneralResponse?>> Delete(int Id)
    {

        clsUser? User;

        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {


            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }

            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }

        bool IsUserAnAdmine = User.Role == DTOUser.enRole.Admine;

        if ((User.Role != DTOUser.enRole.Admine && !(User.IsAthorizedUser(DTOUser.enAtherizations.DeleteTransaction))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }

        if (!int.TryParse(Id.ToString(), out int ID))
        {
            return BadRequest(new DTOGeneralResponse("Send a valid Params", 400, "Validation and Parsing data"));

        }

        clsTransaction? transaction = await clsTransaction.Find(ID);


        if (transaction==null)
        {
            return BadRequest(new DTOGeneralResponse("Send a valid Params", 400, "serch failuer"));

        }
        else
        {
            transaction = null;

        }

        //Dlete all Included Products 

        bool DeletingResult = await clsIncludedProduct.DeleteAll(ID);

        if (!DeletingResult)
        {
            return StatusCode(500,new DTOGeneralResponse("Could Not Delete the Trasaction dependencies", 400, "Delete failuer"));

        }

        DeletingResult = await clsTransaction.Delete(ID);
        if (!DeletingResult)
        {
            return StatusCode(500, new DTOGeneralResponse("Could Not Delete the Trasaction dependencies", 400, "Delete failuer"));


        }
 
      
        
        return Ok(new DTOGeneralResponse("the transaction deleted and all its dependencies ", 200, "None"));

    }




    [HttpGet("GetCurrencies/{ID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>?>> GetAllCurrencies(int ID)
    {
        List<string>? Currencies = await clsCurrency.getCurrecies(ID);
        if (Currencies == null)
        {
            return StatusCode(500, new DTOGeneralResponse("We could not respond due to server error", 500, "NotSet"));
        }

        return Ok(Currencies);
    }

    [HttpGet("GetCurrenciesUsingCountryName/{CountryName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>?>> GetAllCurrencies(string CountryName)
    {
        List<string>? Currencies = await clsCurrency.getCurrecies(CountryName);
        if (Currencies == null)
        {
            return StatusCode(500, new DTOGeneralResponse("We could not respond due to server error", 500, "NotSet"));
        }

    
        return Ok(Currencies);
  


    }



    [HttpGet("GetExchangeRatesToUsd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<KeyValuePair<string, float>>>>GetExchangeRates()
    {
        string? objCurrencyExchange;
        try
        {

            var Client = _httpClientFactory.CreateClient("CurrencyExchange");


            using var response = await Client.GetAsync($"?base=USD");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new DTOGeneralResponse(
                 "Currency Exchange API returned an error status.",
                (uint)response.StatusCode,
                 "HttpError"
            ));
            }

            objCurrencyExchange = (await response.Content.ReadFromJsonAsync<object>())?.ToString();

        }
        catch (Exception e)
        {

            return StatusCode(500, new DTOGeneralResponse("Currency Exchange inf not found ", 500, "serchFilaier"));
        }

        List<KeyValuePair<string, float>> ListOfCurrencies = clsCurrency.GetCurrencyRates(objCurrencyExchange);




        return Ok(ListOfCurrencies);
    }







}