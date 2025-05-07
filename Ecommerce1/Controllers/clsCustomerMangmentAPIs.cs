using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;

using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe.V2;
using Microsoft.Extensions.Logging;
using Stripe.Events;
public class DTOPayment
{

    public List<DTOCartItem>? InCludedProductList { get; set; }

    public string PaymentMethodID { get; set; }

    public DTOPerson? PersonInf { get; set; }

    public DTOPayment()
    {
        this.PaymentMethodID = "";
    }

}



[Route("api/Ecommerce/CustomerMangment")]
[ApiController]


public class clsCustomerMangmentAPIs : ControllerBase
{



    [HttpGet("CartItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCartItems()
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "UnAtherized"));
            }

            else
            {
                List<DTOCartItem>? Cart = await clsCartItem.GetCart(UserID.Value);

                if (Cart != null)
                    foreach (DTOCartItem item in Cart)
                    {
                        clsProduct? p=await clsProduct.Find(item.ProductID);
                     if(p!=null)   item.ImageURL = clsGlobale.SetImageURL(p.ImageName);

                    }
                return Ok(Cart);
            }

        }
        else
        {
            return BadRequest( new DTOGeneralResponse("You need to log in ",400, "UnAutherized"));
        }
    }

    [HttpGet("GetPerson")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOPerson?>> GetPerson()
    {
        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                Response.Cookies.Delete("Authentication");
                return BadRequest(new DTOGeneralResponse("You need to log in ", 400, "UnAutherized"));
            }

            else
            {
                clsUser? user = await clsUser.Find(UserID.Value);
                if (user != null)
                {
                    clsPerson? person = await clsPerson.Find(user.PersonID);

                    if (person != null) { return Ok(person.DTOperson); }

                    else return Ok(null);
                }
                else
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "UnAtherized"));

                }
            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("You need to log in ", 400, "UnAutherized"));
        }

    }

    [HttpPost("Payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    public async Task<ActionResult<object>> Payment([FromBody] DTOPayment PaymentInf)

    {
 
        clsUser? User = null;

        Guid? GuidID = Guid.Empty;



        if (PaymentInf == null || PaymentInf.InCludedProductList == null || PaymentInf.PersonInf == null)
        {
            return BadRequest(new DTOGeneralResponse("You need to send a valiad data ,please check your data and try again",400,"null request"));
        }

        if (PaymentInf.InCludedProductList.Count == 0)
        {
            return BadRequest(new DTOGeneralResponse("thier are no products", 400, "empty request"));


        }

        else if (string.IsNullOrEmpty(PaymentInf.PaymentMethodID))
        {
            return BadRequest(new DTOGeneralResponse("null request,make sure to provied the  data", 400, "Athorization and  Validation Error"));

        }

        string? CountryCoude = await clsLocation.GetCountryCode(PaymentInf.PersonInf.Country);
        if (CountryCoude == null)
        {


            return BadRequest(new DTOGeneralResponse("unvalid data request :the Country Name is Uncorect", 400, "Validation Error"));

        }
        if (!await clsValidation.ValidateLocationAsync(PaymentInf.PersonInf.PostCodeAndLocation, PaymentInf.PersonInf.City, CountryCoude))
        {


            return BadRequest(new DTOGeneralResponse("You need to send a valiad data ,please Check the location (post code or location are wrogn", 400, "Validation Error"));

        }

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

        if (Request.Cookies.TryGetValue("GuidID", out string token2))
        {

            GuidID = clsGlobale.ExtractGuidIDFromToken(token2);

            if (!GuidID.HasValue)
            {




                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred,please log in again", 500, "Athorization abd  Validation Error"));

            }

            else
            {
                clsTransaction? Transaction = await clsTransaction.Find(GuidID.Value);

                if (Transaction != null&&Transaction.State==DTOTransaction.enState.Pending)
                {
                    await clsGlobale.SendEmail(User, "the transaction is in process please wait", "Processing the transaction", false);

                     return BadRequest(new DTOGeneralResponse("the transaction is in process please wait", 400, "Validation Error"));

                }
                else if(Transaction!=null)
                {
                    await clsGlobale.SendEmail(User, "the transaction is in Complited  please LogIn Again", "Processing the transaction", false);

                    return BadRequest(new DTOGeneralResponse($"the transaction is hans Complited with State {Transaction?.State.ToString()},please logIn again", 400, "none"));

                }

            }
        }

        else
        {

            return BadRequest(new DTOGeneralResponse("the transaction is in process please wait", 400, "Validation Error"));

        }


        if (PaymentInf.PersonInf != null)
        {
            User.Person.PostCode = PaymentInf.PersonInf.PostCodeAndLocation;
            User.Person.FirstName = PaymentInf.PersonInf.FirstName;
            User.Person.LastName = PaymentInf.PersonInf.LastName;
            User.Person.City = PaymentInf.PersonInf.City;
            User.Person.Country = PaymentInf.PersonInf.Country;
            User.Person.Phone = PaymentInf.PersonInf.Phone;

            if (!await User.Person.Save())
            {
                 
                return StatusCode(500, new DTOGeneralResponse("Field to save Person Information", 500, "Saving Person inf failed"));

            }

        }

        decimal TotolePrice = 0;

        if (PaymentInf.InCludedProductList != null)
        {
            foreach (DTOCartItem Cartitem in PaymentInf.InCludedProductList)
            {
                clsProduct? p = await (clsProduct.Find(Cartitem.ProductID));
                if (p != null)
                {
                    TotolePrice += (p.Price * Cartitem.NumberOfItems);
                }
            }
        }

        //Create The Product List IDs



        clsTransaction NewTransaction = new clsTransaction(PaymentInf.PaymentMethodID, DTOTransaction.enState.Pending, TotolePrice, User.UserID, GuidID.Value.ToString(), PaymentInf.InCludedProductList);



        if (!await NewTransaction.Save())
        {

            await clsGlobale.SendEmail(User, "We Could not Create a new transaction for you please try again later", "Processing the transaction", false);


            return StatusCode(500, new DTOGeneralResponse("the server is experiencing an internal problem wich can't store the transaction pleas try again!!!", 500, "Saving Transaction  failed"));


        }



        string SecrtKey = clsGlobale.GetStripSecret();

        PaymentIntent _PaymentIntent = new PaymentIntent();
        try
        {
            StripeConfiguration.ApiKey = SecrtKey;

            var option = new PaymentIntentCreateOptions
            {
                Amount = (long)(TotolePrice * 100),
                Currency = "usd",
                PaymentMethod = PaymentInf.PaymentMethodID,
                Confirm = false,
                ReceiptEmail = User.Person.Email,
                Description = "Purchase from MyStore",
                Metadata = new Dictionary<string, string>
            {
                { "TransactionGUID", clsGlobale.GenerateJwtToken(NewTransaction.TransactionGUID) }
            },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {

                    Enabled = true,
                    AllowRedirects = "always" // Disables redirect-based payments


                }
                ,
                Shipping = new ChargeShippingOptions
                {
                    Name = User.UserName,
                    Address = new Stripe.AddressOptions
                    {


                        Country = PaymentInf.PersonInf.Country,
                        City = PaymentInf.PersonInf.City,
                        PostalCode = clsLocation.ExtractPostCodeFromPostCodeAndLocation(PaymentInf.PersonInf.PostCodeAndLocation),

                    }


                }




            };
            var requestOptions = new RequestOptions
            {

                StripeAccount = clsGlobale.GetStripCLIAcountNumber()
            };


            var service = new PaymentIntentService();
        var  PaymentIntent=  await service.CreateAsync(option);

            _PaymentIntent = PaymentIntent;
        }

        catch (StripeException ex)
        {


            clsTransaction? PendingPaymentfailed = await clsTransaction.Find(NewTransaction.TransactionGUID);

            if (PendingPaymentfailed != null)
            {
                PendingPaymentfailed.State = DTOTransaction.enState.Failed;
            bool  Results  = await PendingPaymentfailed.Save();
                var deleteOptions1 = new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/" // Make sure to match the path
                };

                //delete old Cookies 

                Response.Cookies.Delete("GuidID", deleteOptions1);
                Response.Cookies.Delete("Authentication", deleteOptions1);

                if (Results)
                {
                    var cookieOptions1 = new CookieOptions
                    {
                        HttpOnly = true,   // Prevent JavaScript access
                        Secure = true,     // Only send over HTTPS
                        SameSite = SameSiteMode.None, // Prevent CSRF attacks
                        Expires = DateTime.UtcNow.AddHours(1) // Set expiration
                    };
                    //seting the Pending transaction Id to prevent fouble pay
                    var GuidIDToken1 = clsGlobale.GenerateJwtToken(Guid.NewGuid());
                    var AtherizationToken1 = clsGlobale.GenerateJwtToken(User.DTOUser);


                    if (!string.IsNullOrEmpty(GuidIDToken1) && !string.IsNullOrEmpty(AtherizationToken1))
                    {
                        Response.Cookies.Append("GuidID", GuidIDToken1, cookieOptions1);
                        Response.Cookies.Append("Authentication", AtherizationToken1, cookieOptions1);



                    }
                }

            }

            if (ex != null)
            {

                //this validation is just for pructic you need to create a make sure to determe if the problem is the user request
                //or the server error
                return StatusCode(500, new DTOGeneralResponse($"Stripe Pervent the payment :{ex.Message} at {ex.Data}", 500, "Outer Service failure"));
                
            }

            else
            {
                return StatusCode(500, new DTOGeneralResponse($"Stripe Pervent the payment  at {DateTime.Now} unknown reason", 500, "Outer Service failure"));

            }
            

        }



        var deleteOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/" // Make sure to match the path
        };



        //Finsh payment

        // this will move to stripe hooks

        
        /*  clsTransaction? PendingPayment = await clsTransaction.Find(Guid.Parse(NewTransaction.TransactionGUID.ToString()));

          if (PendingPayment != null)
          {
              PendingPayment.State = DTOTransaction.enState.Succeeded;


              await PendingPayment.Save();


          }*/


        //delete old Cookies 
        
        Response.Cookies.Delete("GuidID", deleteOptions);
        Response.Cookies.Delete("Authentication", deleteOptions);


        //Create a new Cookies


        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   // Prevent JavaScript access
            Secure = true,     // Only send over HTTPS
            SameSite = SameSiteMode.None, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration
        };
        //seting the Pending transaction Id to prevent fouble pay
        var GuidIDToken = clsGlobale.GenerateJwtToken(NewTransaction.TransactionGUID);
        var AtherizationToken = clsGlobale.GenerateJwtToken(User.DTOUser);


        if (!string.IsNullOrEmpty(GuidIDToken) && !string.IsNullOrEmpty(AtherizationToken))
        {
            Response.Cookies.Append("GuidID", GuidIDToken, cookieOptions);
            Response.Cookies.Append("Authentication", AtherizationToken, cookieOptions);



        }

        else
        {
 
            return StatusCode(500, new DTOGeneralResponse("LogIn Again", 500, "Cookie Genrating error"));


        }






        await clsGlobale.SendEmail(User, "the Payment is Prosessing...", "Processing the Payment", false);

        return Ok(new
        {
            clientSecret = _PaymentIntent.ClientSecret
        });




    }





}





