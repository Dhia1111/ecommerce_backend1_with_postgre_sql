using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce1.Controllers;

using BusinessLayer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
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
        

        if (User == null) { return BadRequest("Invalaid User information"); }
        if (string.IsNullOrEmpty(User.Person.Email)) { return BadRequest("Invalaid User information(Email)"); }
        if (string.IsNullOrEmpty(User.UserName)) { return BadRequest("Invalaid User information(UserName)"); }
        if (string.IsNullOrEmpty(User.UserPassword)) { return BadRequest("Invalaid User information(PassWord) "); }
        {
            // Check if password length is exactly 9
            if (User.UserPassword.Length < 9)
                return BadRequest("Weak PassWord length should be more then 9");

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
                return BadRequest("Weak PassWord password should have 9 and above characters and 2 letters at lest  and 2 numbers at lest ");

            }

        }


        clsUser? user = await clsUser.Find(User.UserName);
 
        if (user != null)
        {
            return BadRequest("Select another user name ");
        }


        var hasher = new PasswordHasher<object>();

        string hashedPassword = hasher.HashPassword(null, User.UserPassword);

        if (string.IsNullOrEmpty(hashedPassword))
        {
            return StatusCode(500, "An unexpected server error occurred.");

        }

        User.UserPassword = hashedPassword;

        clsUser NewCustomer = new clsUser(User);

        bool Responce = await NewCustomer.SaveCustomer();

        if (!Responce)
        {

            return StatusCode(500, "An unexpected server error occurred.");

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
    <body>
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
                Console.WriteLine("\n\nError :\n" + ex);
                return StatusCode(500, "We could not send an email verfication link pleas reSign.");


            }

            return Ok("Please Verify you email,\n...if thier is no verfication message please sign up again");


        }
        else
        {
            await clsUser.Delete(NewCustomer.UserID);
            await clsPerson.Delete(NewCustomer.PersonID);
            NewCustomer = null;

            return StatusCode(500, "An unexpected server error occurred.");



        }


    }


    [HttpPost("LogIn", Name = "LogIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> LogIn([FromBody] DTOUser User)
    {
        if (User == null) { return BadRequest("Invalaid User information"); }
        if (string.IsNullOrEmpty(User.UserName)) { return BadRequest("Invalaid User information(Name)"); }
        if (string.IsNullOrEmpty(User.UserPassword)) { return BadRequest("Invalaid User information(PassWord) "); }

        clsUser? ExsistedCustomer = await clsUser.Find(User.UserName);

        if (ExsistedCustomer == null)
        {

            return BadRequest("User name or PassWord Are incorect");

        }
      
        var hasher = new PasswordHasher<object>();

        var VerifyhashedPassword = hasher.VerifyHashedPassword(null, ExsistedCustomer.PassWord, User.UserPassword);

        if (VerifyhashedPassword == PasswordVerificationResult.Failed)
        {
            return BadRequest("User name or PassWord Are incorect");

        }

        if (await clsValidatingEmail.Find(ExsistedCustomer.PersonID) != null)
        {
            return BadRequest("An Account Without a verfied email, please check your email for a verfication link then log in again");

        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,   // Prevent JavaScript access
            Secure = true,     // Only send over HTTPS
            SameSite = SameSiteMode.None, // Prevent CSRF attacks
            Expires = DateTime.UtcNow.AddHours(1) // Set expiration

        };

        var AthorizationToken = clsGlobale.GenerateJwtToken(ExsistedCustomer.DTOUser);
        var GuidIDToken=clsGlobale.GenerateJwtToken(Guid.NewGuid());
   
        
        if (!string.IsNullOrEmpty(AthorizationToken)&&!string.IsNullOrEmpty(GuidIDToken))
        {
            Response.Cookies.Append("Authentication", AthorizationToken, cookieOptions);
            Response.Cookies.Append("GuidID",GuidIDToken, cookieOptions);

            return Ok("LogIn seccessfuly");

        }

        else
        {
            return StatusCode(500, "An unexpected server error occurred.");

        }




    }



    [HttpPost("IsUserNameAvalibale", Name = "IsUserNameAvalibale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> IsUserNameFreeToUse([FromBody] string UserName)
    {
        if (string.IsNullOrEmpty(UserName)) { return BadRequest("Invalaid User information(User Name)"); }
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

     

           return Ok( new { Status = "Succeeded", Message = "LogOut done secsesfuly", ErrorType = "" });









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
                return StatusCode(500, "An unexpected server error occurred.");
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


    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> VerifyEmail([FromBody] string GUID_ID)
    {
        clsValidatingEmail? validatingEmail = await clsValidatingEmail.Find(GUID_ID);

        if (validatingEmail == null)
        {

            return BadRequest("Pleas Check if your account is active or  Sign up again ");

        }

          clsUser? User = await clsUser.FindByPersonID(validatingEmail.PersonID);

          if (User == null) { return StatusCode(500, "Un internale server error"); }

        bool result = await clsValidatingEmail.Delete(validatingEmail.PersonID);

        if (!result)
        {
            return StatusCode(500, "Unable to Acive the account and confirme the email");
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
        if (!string.IsNullOrEmpty(AuthenticationToken)&&!string.IsNullOrEmpty(GuidIdToken))
        {
            Response.Cookies.Append("Authentication", AuthenticationToken, cookieOptions);
            Response.Cookies.Append("GuidID", GuidIdToken, cookieOptions);

            return Ok("Email verfied seccessfuly");

        }

        else
        {
            return StatusCode(500, "An unexpected server error occurred.");

        }
        //
    }


  



}
