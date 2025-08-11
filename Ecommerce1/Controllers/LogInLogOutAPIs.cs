using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce1.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[Route("api/Ecommerce/LogInLogOut")]
[ApiController]
public class EcommerceController : ControllerBase
{
  
    [HttpPost("SignUp", Name = "SignUp")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> SignUp([FromBody] DTOUser User)
    {
        Response.Cookies.Delete("Authentication");
        

        if (User == null) { 
            return BadRequest(new DTOGeneralResponse("Invalaid User information",400,"null data"));
        
        }
        if (string.IsNullOrEmpty(User.Person.Email)) {  

            return BadRequest(new DTOGeneralResponse("nvalaid User information(Email)", 400, "null data"));
        }
        if (string.IsNullOrEmpty(User.UserName)) {  
            return BadRequest(new DTOGeneralResponse("Invalaid User information(UserName)", 400, "null data"));
        }
        if (string.IsNullOrEmpty(User.UserPassword)) { 
            return BadRequest(new DTOGeneralResponse("Invalaid User information(PassWord)", 400, "null data"));
        }
        {
            // Check if password length is exactly 9
            if (User.UserPassword.Length < 9)
             return BadRequest(new DTOGeneralResponse("Weak PassWord length should be more then 9", 400, "null data"));

            // Count letters and digits
            int letterCount = 0;
            int digitCount = 0;

            foreach (char c in User.UserPassword)
            {
                if (char.IsLetter(c))
                    letterCount++;
                else if (char.IsDigit(c))
                    digitCount++;
            }

            // Must have at least 2 letters and at least 2 digits
            bool LetterOrDigetsLessThenTow= letterCount < 2 || digitCount < 2;

            if (LetterOrDigetsLessThenTow)
            {
                 return BadRequest(new DTOGeneralResponse("Weak PassWord password should have 9 and above characters and 2 letters at lest  and 2 numbers at lest ", 400, "null data"));

            }

        }


        clsUser? user = await clsUser.Find(User.UserName);
 
        if (user != null)
        {
             return BadRequest(new DTOGeneralResponse("Select another user name ", 400, "Unvalid data Serch error"));

        }


        var hasher = new PasswordHasher<object>();

        string hashedPassword = hasher.HashPassword(null, User.UserPassword);



        if (string.IsNullOrEmpty(hashedPassword))
        {
            return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "Creating Incription for pass word"));

        }

        User.UserPassword = hashedPassword;

        clsUser NewCustomer = new clsUser(User);

        bool Responce = await NewCustomer.SaveCustomer();

        if (!Responce)
        {

             return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "Creating Incription for pass word"));

        }

        Guid NewGUID_ID = Guid.NewGuid();

        clsValidatingEmail UnValaidEmail = new clsValidatingEmail(NewCustomer.PersonID, NewGUID_ID);

        if (await UnValaidEmail.Add())
        {
            try
            {
                string emailbody = @$"<html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .email-container {{
                max-width: 600px;
                margin: auto;
                background: #ffffff;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                text-align: center;
            }}
            h2 {{
                color: #333;
            }}
            p {{
                color: #555;
                font-size: 16px;
            }}
            .btn {{                
                display: inline-block;
                background-color: rgb(25, 52, 52);
                padding: 10px 20px;
                text-decoration: none;
                font-size: 18px;
                border-radius: 15px;
                margin-top: 20px;
                color: white;
            }}
            .btn:hover {{
                color: white;
                background-color: rgb(76, 94, 94);
                transition-duration: 1s;
            }}
            .footer {{
                margin-top: 20px;
                font-size: 14px;
                color: #777;
            }}
            .footer a {{
                color: blue; /* Default link color */
                text-decoration: underline;
            }}
            .email-link {{
                color: white !important; /* Ensuring email link stays white */
                text-decoration: none;
            }}
        </style>
    </head>
    <body
        <div class='email-container'>
            <h2>Welcome to DEPhone</h2>
            <p>Thank you for signing up! Please verify your email by clicking the button below:</p>
            <a class='btn email-link' href='{clsGlobale.GetVerifyEmailLink()}?token={NewGUID_ID.ToString()}'>Verify Email</a>
          
        </div>
    </body>
    </html>";


                using SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(clsGlobale.GetEmail(), clsGlobale.GetEmailPassWord()),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(clsGlobale.GetEmail()),
                    Subject = "Verify Your Email",

                    // make sure to create a valiad VerificationLink based on yor front end 

                    Body = emailbody,


                    IsBodyHtml = true,
                };

                mailMessage.To.Add(User.Person.Email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
  
                return StatusCode(500, new DTOGeneralResponse("We could not send an email verfication link pleas reSign.", 500, "Email integration"));

            }

            return Ok(new DTOGeneralResponse("Please Verify you email,\n...if thier is no verfication message please sign up again", 200, "none"));


        }
        else
        {
            await clsUser.Delete(NewCustomer.UserID);
            await clsPerson.Delete(NewCustomer.PersonID);
            NewCustomer = null;

            return StatusCode(500, new DTOGeneralResponse("We could not send an email verfication link pleas reSign.", 500, "Internal email handling"));



        }


    }


    [HttpPost("LogIn", Name = "LogIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> LogIn([FromBody] DTOUser User)
    {
        if (User == null) {
            return BadRequest( new DTOGeneralResponse("Invalaid User information", 400, "null data"));

        }
        if (string.IsNullOrEmpty(User.UserName))
        {

            return BadRequest(new DTOGeneralResponse("Invalaid User information(Name)", 400, "null data"));
        }
         if (string.IsNullOrEmpty(User.UserPassword)) {


            return BadRequest(new DTOGeneralResponse("Invalaid User information(PassWord) ", 400, "null data")); }

        clsUser? ExsistedCustomer = await clsUser.Find(User.UserName);

        if (ExsistedCustomer == null)
        {

            return BadRequest(new DTOGeneralResponse("User name or PassWord Are incorect", 400, "null data"));

        }
      
        var hasher = new PasswordHasher<object>();

        var VerifyhashedPassword = hasher.VerifyHashedPassword(null, ExsistedCustomer.PassWord, User.UserPassword);

        if (VerifyhashedPassword == PasswordVerificationResult.Failed)
        {
            return BadRequest(new DTOGeneralResponse("User name or PassWord Are incorect", 400, "null data"));

        }

        if (await clsValidatingEmail.Find(ExsistedCustomer.PersonID) != null)
        {
            return BadRequest(new DTOGeneralResponse("An Account Without a verfied email, please check your email for a verfication link then log in again", 400, "null data"));

        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   // Prevent JavaScript access
            Secure = true,     // Only send over HTTPS
            SameSite = SameSiteMode.None, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration

        };

        var AthorizationToken = clsGlobale.GenerateJwtToken(ExsistedCustomer.DTOUser);
        string? PaymentGUIDToken = "";
        //check if the use does not have unfinshed payment 
        if (await ExsistedCustomer.HasUnfinshedPayment())
        {
            //this is to make sure to prevent double payment  in any case 

            Guid? UnfinshedPaymentGuid = await clsTransaction.GetUnfinshedPayment(ExsistedCustomer.UserID);
            PaymentGUIDToken = clsGlobale.GenerateJwtToken(UnfinshedPaymentGuid.Value);
        }
        else
        {
            PaymentGUIDToken= clsGlobale.GenerateJwtToken(Guid.NewGuid());

        }

        if (!string.IsNullOrEmpty(AthorizationToken)&&!string.IsNullOrEmpty(PaymentGUIDToken))
        {
            Response.Cookies.Append("Authentication", AthorizationToken, cookieOptions);
            Response.Cookies.Append("GuidID",PaymentGUIDToken, cookieOptions);

            return Ok(new DTOGeneralResponse("LogIn seccessfuly",200,"None"));

        }

        else
        {
            return StatusCode(500,new DTOGeneralResponse("An unexpected server error occurred.",500,"Create JWT"));

        }




    }


    [HttpPost("IsUserNameAvalibale", Name = "IsUserNameAvalibale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> IsUserNameFreeToUse([FromBody] string UserName)
    {
        if (string.IsNullOrEmpty(UserName)) { return BadRequest(new DTOGeneralResponse("Invalaid User information(User Name)", 400, "null data")); }
        clsUser? IsUserIxsiste = await clsUser.Find(UserName);
        return Ok(IsUserIxsiste == null);



    }


    [HttpPost("LogOut", Name = "LogOut")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public  ActionResult<object>  LogOut()
     {


        var deleteOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/" // Make sure to match the path
        };

        Response.Cookies.Delete("Authentication", deleteOptions);
        Response.Cookies.Delete("GuidID", deleteOptions);

     

           return Ok( new DTOGeneralResponse("LogOut done secsesfully" ,200,"None"));









    }


    [HttpGet("IsLogedIn", Name = "IsLogedIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsLogedIn()
    {

        if (Request.Cookies.TryGetValue("Authentication", out string token))
        {
            int? UserID = clsGlobale.ExtractUserIdFromToken(token);

            if (UserID == null)
            {
                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.",500,"Serch faileur"));
            }

            else
            {
                clsUser? user = await clsUser.Find(UserID.Value);
                if (user!= null)
                {
                    return Ok(true);
                }
                else
                {
                     Response.Cookies.Delete("Authentication");
                    Response.Cookies.Delete("GuidID");

                    return Ok(false);
                }
            }
        }

        return Ok(false);


    }




    [HttpPost("VERIFYEMAIL", Name = "VERIFYEMAIL")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DTOGeneralResponse>> VerifyEmail([FromBody] string GUID_ID)
    {
        clsValidatingEmail? validatingEmail = await clsValidatingEmail.Find(GUID_ID);

        if (validatingEmail == null)
        {

            return BadRequest(new DTOGeneralResponse("Pleas Check if your account is active or  Sign up again ", 400, "Serch faileur"));

        }

        clsUser? User = await clsUser.FindByPersonID(validatingEmail.PersonID);

        if (User == null) { return StatusCode(500, new DTOGeneralResponse("Un internale server error", 500, "Serch faileur")); }

        bool result = await clsValidatingEmail.Delete(validatingEmail.PersonID);

        if (!result)
        {
            return StatusCode(500, new DTOGeneralResponse("Unable to Acive the account and confirme the email", 500, "Deleting data Faileur"));
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   // Prevent JavaScript access
            Secure = true,     // Only send over HTTPS
            SameSite = SameSiteMode.None, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration
        };

        string? AuthenticationToken = clsGlobale.GenerateJwtToken(User.DTOUser);
        string? GuidIdToken = clsGlobale.GenerateJwtToken(Guid.NewGuid());
        if (!string.IsNullOrEmpty(AuthenticationToken) && !string.IsNullOrEmpty(GuidIdToken))
        {
            Response.Cookies.Append("Authentication", AuthenticationToken, cookieOptions);
            Response.Cookies.Append("GuidID", GuidIdToken, cookieOptions);

            return Ok(new DTOGeneralResponse("Email Verfied secsessfuly", 200, "none"));

        }

        else
        {
            return StatusCode(500, new DTOGeneralResponse("An unexpected server error accurred.", 500, "Creating JWT faileur"));

        }
    }




}
